using PrefsGUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PrefsGUI
{

    public class PrefsGUIEditorImpl
    {
        Dictionary<GameObject, List<PrefsParam>> goParams = new Dictionary<GameObject, List<PrefsParam>>();

        float interaval = 1f;
        float lastTime;

        public void Update()
        {
            var time = (float)EditorApplication.timeSinceStartup;
            if (time - lastTime > interaval)
            {
                UpdateCompParam();
                lastTime = time;
            }
        }

        void UpdateCompParam()
        {
            var gos = Object.FindObjectsOfType<GameObject>();
            for (var iGo = 0; iGo < gos.Length; ++iGo)
            {
                var go = gos[iGo];
                var comps = go.GetComponents<Component>();
                var prefsList = new List<PrefsParam>();

                for (var i = 0; i < comps.Length; ++i)
                {
                    var comp = comps[i];
                    var fields = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    prefsList.AddRange(SearchChildPrefsParams(comp, fields));
                }

                if (prefsList.Any())
                {
                    goParams[go] = prefsList;
                }
            }
        }

        List<PrefsParam> SearchChildPrefsParams(object obj, FieldInfo[] fields, int level = 0)
        {
            var ret = new List<PrefsParam>();

            for (var fi = 0; fi < fields.Length; ++fi)
            {
                var field = fields[fi];
                var fieldType = field.FieldType;
                var fieldObj = field.GetValue(obj);

                if (fieldType.IsSubclassOf(typeof(PrefsParam)))
                {
                    ret.Add(fieldObj as PrefsParam);
                }
                else if (
                    fieldObj != null
                    && !fieldType.IsPrimitive
                    && !fieldType.IsSubclassOf(typeof(Component))
                    && fieldType.GetCustomAttribute<System.SerializableAttribute>() != null
                    && fieldType.Assembly.GetName().Name == "Assembly-CSharp"
                        )
                {
                    ret.AddRange(SearchChildPrefsParams(fieldObj, fieldType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)));
                }
            }

            return ret;
        }

        public void OnGUI(PrefsGUIEditor editor)
        {
            AdditionalHeader();

            if (editor.order == PrefsGUIEditor.Order.GameObject)
            {
                goParams.Where(dic => dic.Key != null).OrderBy(dic => dic.Key.name).ToList().ForEach(pair =>
                {
                    var go = pair.Key;
                    var prefsParams = pair.Value;

                    using (new GUILayout.HorizontalScope())
                    {
                        LabelWithEditPrefix(go.name, go, prefsParams);
                        GUILayout.FlexibleSpace();
                    }

                    GUIUtil.Indent(() =>
                    {
                        PrefsParamsGUI(prefsParams);
                    });
                });
            }
            else
            {
                PrefsParamsGUI(editor.allPrefs.ToList());
            }
        }



        protected virtual void AdditionalHeader()
        {
        }

        protected virtual void PrefsParamsGUI(List<PrefsParam> list)
        {
            list.ForEach(prefs =>
            {
                using (new GUILayout.HorizontalScope())
                {
                    prefs.OnGUI();
                }
            });
        }



        protected virtual void LabelWithEditPrefix(string label, Object target, List<PrefsParam> prefsList)
        {
            GUILayout.Label(label);

            const char separator = '.';
            var prefix = prefsList.Select(p => p.key.Split(separator)).Where(sepKeys => sepKeys.Length > 1).FirstOrDefault()?.First();

            GUILayout.Label("KeyPrefix:");

            var prefixNew = GUILayout.TextField(prefix, GUILayout.MinWidth(100f));
            if (prefix != prefixNew)
            {
                Undo.RecordObject(target, "Change PrefsGUI Prefix");
                EditorUtility.SetDirty(target);

                var prefixWithSeparator = string.IsNullOrEmpty(prefixNew) ? "" : prefixNew + separator;
                prefsList.ForEach(p =>
                {
                    p.key = prefixWithSeparator + p.key.Split(separator).Last();
                });
            }
        }
    }
}