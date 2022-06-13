using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PrefsGUI;
using UnityEngine;

namespace PrefsGUi.Editor
{
    public static class PrefsTypeUtility
    {
        static readonly Dictionary<Type, FieldInfo[]> typeToFields = new();

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

        static readonly Dictionary<Type, bool> hasContainPrefs = new();
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
                    var isPrefsEnumerableType = type.GetInterfaces()
                        .Any(interfaceType =>
                            interfaceType.IsGenericType
                            && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                            && interfaceType.GetGenericArguments().Any(HasContainPrefs));

                    ret = isPrefsEnumerableType
                          || GetFieldInfos(type).Any(field => HasContainPrefs(field.FieldType));
                    
                }

                hasContainPrefs[type] = ret;
            }

            return ret;
        }


        static readonly HashSet<object> circularReferenceGuard = new();
        public static HashSet<PrefsParam> SearchChildPrefsParams(object obj)
        {
            var ret = new HashSet<PrefsParam>();

            if (obj == null) return ret;
            if (circularReferenceGuard.Contains(obj)) return ret;
            circularReferenceGuard.Add(obj);


            if (obj is IEnumerable enumerable)
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
    }
}