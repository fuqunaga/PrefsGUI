using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PrefsGUI
{

    public static class GameObjectPrefsUtility
    {
        #region Type Define

        public class GoPrefs
        {
            public GameObject go;
            public List<MonoPrefs> monoPrefsList;

            public IEnumerable<PrefsParam> prefsList => monoPrefsList.SelectMany(mp => mp.prefsList);
            public IEnumerable<(GameObject, PrefsParam)> keys => prefsList.Select(p => (go, p));
        }

        public class MonoPrefs
        {
            public MonoBehaviour mono;
            public List<PrefsParam> prefsList;
        }

        #endregion



        #region GoPrefsList

        public static List<GoPrefs> goPrefsList = new List<GoPrefs>();

        static float interaval = 1f;
        static float lastTime;


        public static void UpdateGoPrefs()
        {
            var time = (float)EditorApplication.timeSinceStartup;
            if (time - lastTime > interaval)
            {
                DoUpdateGoPrefs();

                lastTime = time;
            }
        }


        static void DoUpdateGoPrefs()
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>()
                .Select(go => new GoPrefs()
                {
                    go = go,
                    monoPrefsList = go.GetComponents<MonoBehaviour>()
                        .Where(mono => !Assembly.GetAssembly(mono.GetType()).GetName().Name.StartsWith("UnityEngine.")) // skip unity classes
                        .Select(mono => new MonoPrefs() { mono = mono, prefsList = SearchChildPrefsParams(mono).ToList() })
                        .Where(mp => mp.prefsList.Any())
                        .ToList()
                })
                .Where(gp => gp.monoPrefsList.Any())
                .ToList();

            var inHierarchy = all
                .Where(gp => gp.go.scene.name != null)
                .ToList();

            var prefabSources = new HashSet<GameObject>(inHierarchy.Select(gp => PrefabUtility.GetCorrespondingObjectFromOriginalSource(gp.go)));

            var inProject = all
                .Except(inHierarchy)
                .Where(gp => !prefabSources.Contains(gp.go)) // ignore prefabs that the child is in the hierarchy
                ;

            goPrefsList = inHierarchy.Concat(inProject)
                .OrderBy(gp => gp.go.name)
                .ToList();
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

        public static HashSet<PrefsParam> SearchChildPrefsParams(object obj)
        {
            var ret = new HashSet<PrefsParam>();

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
                        if (fieldObj != null)
                        {
                            var prefs = fieldObj as PrefsParam;

                            if (prefs != null)
                            {
                                ret.Add(prefs);
                            }
                            else
                            {
                                var enumerable = fieldObj as IEnumerable;
                                if (enumerable != null)
                                {
                                    foreach (var elem in enumerable)
                                    {
                                        if (elem != null)
                                        {
                                            var elemPrefs = elem as PrefsParam;
                                            if (elemPrefs != null)
                                            {
                                                ret.Add(elemPrefs);
                                            }
                                            else
                                            {
                                                ret.UnionWith(SearchChildPrefsParams(elem));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ret.UnionWith(SearchChildPrefsParams(fieldObj));
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        #endregion

    }
}