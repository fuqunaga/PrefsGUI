using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PrefsGUi.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace PrefsGUI.Editor
{
    public class PrefsAssetUtility : AssetPostprocessor
    {
        #region Type Define

        public class PrefsHolder
        {
            public Object parent;
            public HashSet<PrefsParam> prefsSet;
        }

        public class ObjPrefs
        {
            public readonly Object obj;
            public readonly List<PrefsHolder> holders;

            public ObjPrefs(Object obj, IEnumerable<PrefsHolder> holders)
            {
                this.obj = obj;
                this.holders = holders.ToList();
            }

            public IEnumerable<PrefsParam> PrefsAll => holders.SelectMany(h => h.prefsSet);

            public Object GetPrefsParent(PrefsParam prefs)
            {
                return holders.FirstOrDefault(h => h.prefsSet.Contains(prefs))?.parent;
            }
        }

        readonly struct ObjPrefsComparer : IEqualityComparer<ObjPrefs>
        {
            public bool Equals(ObjPrefs x, ObjPrefs y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.obj, y.obj) && x.holders.SequenceEqual(y.holders, new PrefsHolderComparer());
            }

            public int GetHashCode(ObjPrefs obj)
            {
                return HashCode.Combine(obj.obj, obj.holders);
            }
        }

        readonly struct PrefsHolderComparer : IEqualityComparer<PrefsHolder>
        {
            public bool Equals(PrefsHolder x, PrefsHolder y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.parent, y.parent) && x.prefsSet.SequenceEqual(y.prefsSet);
            }

            public int GetHashCode(PrefsHolder obj)
            {
                return HashCode.Combine(obj.parent, obj.prefsSet);
            }
        }

        #endregion

        #region AssetPostprocessor
        
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            needUpdate = true;
        }
        
        #endregion
        
        
        private static readonly float interval = 3f;
        private static float lastUpdateCheckTime;

        public static Action onObjPrefsListChanged;
        
        private static List<ObjPrefs> objPrefsList = new();

        private static bool needUpdate = true;

        public static IReadOnlyList<ObjPrefs> ObjPrefsList 
        {
            get
            {
                UpdateObjPrefsIfNeed();
                return objPrefsList;
            }
        }

        public static IEnumerable<(PrefsParam, Object, Object)> PrefsObjComponentList
            => ObjPrefsList.SelectMany(op => op.holders.SelectMany(holder => holder.prefsSet.Select(prefs => (prefs, op.obj, holder.parent))));

          

        private static bool IsInScene(GameObject go) => go.scene.name != null;


        public static void UpdateObjPrefsIfNeed()
        {
            var time = (float) EditorApplication.timeSinceStartup;
            needUpdate |= time - lastUpdateCheckTime > interval;

            if (needUpdate)
            {
                if (DoUpdateGoPrefs())
                {
                    onObjPrefsListChanged?.Invoke();
                }

                needUpdate = false;
                lastUpdateCheckTime = time;
            }
        }


        static bool DoUpdateGoPrefs()
        {
            using var gameObjectsScope = ListPool<GameObject>.Get(out var gameObjects);
            gameObjects.AddRange(
                Resources.FindObjectsOfTypeAll<GameObject>()
                    .Where(go => PrefabStageUtility.GetPrefabStage(go) == null) // ignore GameObject in  PrefabStage
            );

            var prefabSources = new HashSet<GameObject>(gameObjects.Where(IsInScene).Select(PrefabUtility.GetCorrespondingObjectFromOriginalSource));


            var objPrefsOfGameObjects = gameObjects
                .Except(prefabSources)  // ignore prefabs that the child is in the scene
                .Select(go =>
                {
                    var holders = go.GetComponents<MonoBehaviour>()
                            .Where(mono => mono != null)
                            .Where(mono => !Assembly.GetAssembly(mono.GetType()).GetName().Name.StartsWith("UnityEngine.")) // skip unity classes
                            .Select(mono => new PrefsHolder() { parent = mono, prefsSet = PrefsTypeUtility.SearchChildPrefsParams(mono) })
                            .Where(holder => holder.prefsSet.Any());

                    return (go, holders);
                })
                .Where(pair => pair.holders.Any(holder => holder.prefsSet.Any()))
                .Select(pair => new ObjPrefs(
                    pair.go,
                    pair.holders
                ));

            var objPrefsOfScriptableObjects = Resources.FindObjectsOfTypeAll(typeof(ScriptableObject))
                .Where(so =>
                {
                    var asmName = Assembly.GetAssembly(so.GetType()).GetName().Name;
                    return !asmName.StartsWith("Unity.") && !asmName.StartsWith("UnityEditor") && !asmName.StartsWith("UnityEngine");
                })
                .Select(so => (so, prefsList: PrefsTypeUtility.SearchChildPrefsParams(so)))
                .Where(pair => pair.prefsList.Any())
                .Select(pair => new ObjPrefs(
                    pair.so,
                    new[] { new PrefsHolder() { parent = pair.so, prefsSet = pair.prefsList } }
                    )
                );

            using var newListScope = ListPool<ObjPrefs>.Get(out var newList);
            newList.AddRange(objPrefsOfGameObjects
                .Concat(objPrefsOfScriptableObjects)
                .OrderBy(op => op.obj.name)
            );

            var change = !objPrefsList.SequenceEqual(newList, new ObjPrefsComparer());
            if (change)
            {
                objPrefsList = new(newList);
            }

            return change;
        }
    }
}