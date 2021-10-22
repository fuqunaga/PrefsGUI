using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrefsGUI.Editor
{
    public static class ObjectPrefsUtility
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

            public IEnumerable<PrefsParam> prefsList => holders.SelectMany(h => h.prefsSet);

            public Object GetPrefsParent(PrefsParam prefs)
            {
                return holders.FirstOrDefault(h => h.prefsSet.Contains(prefs))?.parent;
            }
        }

        class ObjPrefsComparer : IEqualityComparer<ObjPrefs>
        {
            public static ObjPrefsComparer Instance = new ObjPrefsComparer();

            public bool Equals(ObjPrefs x, ObjPrefs y)
            {
                return x.obj.Equals(y.obj);
            }

            public int GetHashCode(ObjPrefs obj)
            {
                return obj.obj.GetHashCode();
            }
        }

        public static bool IsInScene(GameObject go) => go.scene.name != null;

        #endregion


        #region ObjPrefsList

        public static List<ObjPrefs> objPrefsList = new List<ObjPrefs>();

        static readonly float interaval = 3f;
        static float lastTime;

        public static Action onGoPrefsListChanged;


        public static void UpdateGoPrefs()
        {
            var time = (float)EditorApplication.timeSinceStartup;
            if (time - lastTime > interaval)
            {
                if (DoUpdateGoPrefs())
                {
                    onGoPrefsListChanged?.Invoke();
                }

                lastTime = (float)EditorApplication.timeSinceStartup; ;
            }
        }


        static bool DoUpdateGoPrefs()
        {
            var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => PrefabStageUtility.GetPrefabStage(go) == null); // ignore GameObject in  PrefabStage

            var prefabSources = new HashSet<GameObject>(gameObjects.Where(IsInScene).Select(PrefabUtility.GetCorrespondingObjectFromOriginalSource));


            var objPrefsOfGameObjects = gameObjects
                .Except(prefabSources)  // ignore prefabs that the child is in the scene
                .Select(go =>
                {
                    var holders = go.GetComponents<MonoBehaviour>()
                            .Where(mono => mono != null)
                            .Where(mono => !Assembly.GetAssembly(mono.GetType()).GetName().Name.StartsWith("UnityEngine.")) // skip unity classes
                            .Select(mono => new PrefsHolder() { parent = mono, prefsSet = SearchChildPrefsParams(mono) })
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
                .Select(so => (so, prefsList: SearchChildPrefsParams(so)))
                .Where(pair => pair.prefsList.Any())
                .Select(pair => new ObjPrefs(
                    pair.so,
                    new[] { new PrefsHolder() { parent = pair.so, prefsSet = pair.prefsList } }
                    )
                );


#if false
                .Select(go => new ObjPrefs(
                    go,
                    go.GetComponents<MonoBehaviour>()
                        .Where(mono => !Assembly.GetAssembly(mono.GetType()).GetName().Name.StartsWith("UnityEngine.")) // skip unity classes
                        .Select(mono => new ObjPrefsField() { obj = mono, prefsList = SearchChildPrefsParams(mono).ToList() })
                        .Where(mp => mp.prefsList.Any())
                        .ToList()
                        )
                )
                .Where(gp => gp.objPrefsList.Any())
                .ToList();
                */

            var inHierarchy = all
                .Where(gp => gp.go.scene.name != null)
                .ToList();

            var prefabSources = new HashSet<GameObject>(inHierarchy.Select(gp => PrefabUtility.GetCorrespondingObjectFromOriginalSource(gp.go)));

            var inProject = all
                .Except(inHierarchy)
                .Where(gp => !prefabSources.Contains(gp.go)) // ignore prefabs that the child is in the hierarchy
                ;

            var newList = inHierarchy.Concat(inProject)
                .OrderBy(gp => gp.go.name)
                .ToList();
#else
            var newList = objPrefsOfGameObjects
                .Concat(objPrefsOfScriptableObjects)
                .OrderBy(op => op.obj.name);
#endif

            var change = !objPrefsList.SequenceEqual(newList, ObjPrefsComparer.Instance);

            if (change)
            {
                objPrefsList = newList.ToList();
            }

            return change;
        }

        #endregion


        #region SearchChildPrefsParams() 

        static Dictionary<Type, FieldInfo[]> typeToFields = new Dictionary<Type, FieldInfo[]>();

        static FieldInfo[] GetFieldInfos(Type type)
        {
            if (!typeToFields.TryGetValue(type, out var ret))
            {
                typeToFields[type] = ret = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(field => !field.FieldType.IsPrimitive)
                    .ToArray();
            }

            return ret;
        }

        static Dictionary<Type, bool> hasContainPrefs = new Dictionary<Type, bool>();
        public static bool HasContainPrefs(Type type)
        {
            if (!hasContainPrefs.TryGetValue(type, out var ret))
            {
                hasContainPrefs[type] = false; // avoid self reference

                if (type.IsSubclassOf(typeof(PrefsParam)))
                {
                    ret = true;
                }
                else
                {
                    if (type.GetInterfaces()
                        .Any(iface =>
                            iface.IsGenericType
                            && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                            && iface.GetGenericArguments().Any(arg => HasContainPrefs(arg)))
                            )
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = GetFieldInfos(type).Any(field => HasContainPrefs(field.FieldType));
                    }
                }

                hasContainPrefs[type] = ret;
            }

            return ret;
        }


        static HashSet<object> circularReferenceGuard = new HashSet<object>();
        public static HashSet<PrefsParam> SearchChildPrefsParams(object obj)
        {
            var ret = new HashSet<PrefsParam>();

            if (obj == null) return ret;
            if (circularReferenceGuard.Contains(obj)) return ret;
            circularReferenceGuard.Add(obj);


            // is IEnumerable
            var enumerable = obj as IEnumerable;
            if (enumerable != null)
            {
                foreach (var elem in enumerable)
                {
                    CheckAndAddPrefs(ret, elem);
                }
            }

            var type = obj.GetType();
            if (HasContainPrefs(type))
            {
                var fields = GetFieldInfos(type);
                for (var fi = 0; fi < fields.Length; ++fi)
                {
                    var field = fields[fi];
                    var fieldType = field.FieldType;

                    if (HasContainPrefs(fieldType))
                    {
                        var fieldObj = field.GetValue(obj);
                        CheckAndAddPrefs(ret, fieldObj);
                        /*
                        if (fieldObj != null)
                        {
                            var prefs = fieldObj as PrefsParam;

                            if (prefs != null)
                            {
                                ret.Add(prefs);
                            }
                            else
                            {
                                AddChildPrefsParam(fieldObj);
                            }
                        }
                        */
                    }
                }
            }


            circularReferenceGuard.Remove(obj);

            return ret;


            void CheckAndAddPrefs(HashSet<PrefsParam> prefsSet, object o)
            {
                // ignore child MonoBehavior
                if ((o != null) && !(o is MonoBehaviour))
                {
                    if (o is PrefsParam prefs)
                    {
                        prefsSet.Add(prefs);
                    }
                    else
                    {
                        prefsSet.UnionWith(SearchChildPrefsParams(o));
                    }
                }
            }
        }

        #endregion
    }
}