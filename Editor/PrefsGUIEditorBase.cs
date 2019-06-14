using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Collections;

namespace PrefsGUI
{
    public abstract class PrefsGUIEditorBase : EditorWindow
    {
        protected IEnumerable<PrefsParam> prefsAll { get { return PrefsParam.all.Values.OrderBy(prefs => prefs.key); } }

        void OnGUI()
        {
            var buttunStyleOrig = GUI.skin.button;
            var buttonStyle = new GUIStyle(buttunStyleOrig);
            buttonStyle.richText = true;
            GUI.skin.button = buttonStyle;

            OnGUIInternal();

            GUI.skin.button = buttunStyleOrig;
        }

        protected abstract void OnGUIInternal();



        static Dictionary<Type, FieldInfo[]> typeToFields = new Dictionary<Type, FieldInfo[]>();

        static protected FieldInfo[] GetFieldInfos(Type type)
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
        public static  bool HasContainPrefs(Type type)
        {
            if ( !hasContainPrefs.TryGetValue(type, out var ret))
            {
                hasContainPrefs[type] = false; // avoid self reference

                if ( type.IsSubclassOf(typeof(PrefsParam)))
                {
                    ret = true;
                }
                else
                {
                    if ( type.GetInterfaces()
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

        public static  HashSet<PrefsParam> SearchChildPrefsParams(object obj)
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

    }
}