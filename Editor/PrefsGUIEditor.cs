using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace PrefsGUI
{
    public class PrefsGUIEditor : PrefsGUIEditorBase
    {
        [MenuItem("Window/PrefsGUI")]
        public static void ShowWindow()
        {
            GetWindow<PrefsGUIEditor>("PrefsGUI");
        }

        public enum Order
        {
            AtoZ,
            GameObject,

        }

        Order _order;

        Vector2 scrollPosition;
        SetCurrentToDefaultWindow setCurrentToDefaultWindow;

        protected override void OnGUIInternal()
        {
            using (var h0 = new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save")) Prefs.Save();
                if (GUILayout.Button("Load")) Prefs.Load();
                if (GUILayout.Button("DeleteAll"))
                {
                    if (EditorUtility.DisplayDialog("DeleteAll", "Are you sure to delete all current prefs parameters?", "DeleteAll", "Don't Delete"))
                    {
                        Prefs.DeleteAll();
                    }
                }
            }

            var currentToDefaultEnable = !Application.isPlaying && PrefsList.Any(prefs => !prefs.IsDefault);
            GUI.enabled = currentToDefaultEnable;
            if (GUILayout.Button("Open Current To Default Window"))
            {
                if (setCurrentToDefaultWindow == null) setCurrentToDefaultWindow = CreateInstance<SetCurrentToDefaultWindow>();
                setCurrentToDefaultWindow.parentWindow = this;
                setCurrentToDefaultWindow.ShowUtility();
            }
            GUI.enabled = true;

            GUILayout.Space(8f);

            using (var h = new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Order");
                _order = (Order)GUILayout.SelectionGrid((int)_order, System.Enum.GetNames(typeof(Order)), 5);
            }

            GUILayout.Space(8f);

            using (var sc = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = sc.scrollPosition;

                var sync = FindObjectOfType<PrefsGUISync>();
                if (sync != null) GUILayout.Label("sync");


                if (Order.GameObject == _order)
                {
                    _goParams.Where(dic => dic.Key != null).OrderBy(dic => dic.Key.name).ToList().ForEach(pair =>
                    {
                        var go = pair.Key;
                        var prefsList = pair.Value;

                        LabelWithEditPrefix(sync, go.name, go, prefsList);

                        GUIUtil.Indent(() =>
                        {
                            prefsList.ForEach(prefs =>
                            {
                                using (var h = new GUILayout.HorizontalScope())
                                {
                                    SyncToggle(sync, prefs);
                                    prefs.OnGUI();
                                }
                            });
                        });
                    });
                }
                else
                {
                    PrefsList.ToList().ForEach(prefs =>
                    {
                        using (var h = new GUILayout.HorizontalScope())
                        {
                            if (sync != null)
                            {
                                var key = prefs.key;
                                var isSync = !sync._ignoreKeys.Contains(key);

                                if (isSync != GUILayout.Toggle(isSync, "", GUILayout.Width(16f)))
                                {
                                    Undo.RecordObject(sync, "Change PrefsGUI sync flag");

                                    if (isSync) sync._ignoreKeys.Add(key);
                                    else sync._ignoreKeys.Remove(key);
                                }
                            }

                            prefs.OnGUI();
                        }
                    });
                }
            }

            if ((setCurrentToDefaultWindow != null) && Event.current.type == EventType.Repaint) setCurrentToDefaultWindow.Repaint();
        }


        Dictionary<GameObject, List<PrefsParam>> _goParams = new Dictionary<GameObject, List<PrefsParam>>();

        float _interaval = 1f;
        float _lastTime;

        private void Update()
        {
            var time = (float)EditorApplication.timeSinceStartup;
            if (time - _lastTime > _interaval)
            {
                UpdateCompParam();
                _lastTime = time;
            }
        }

        void UpdateCompParam()
        {
            var gos = FindObjectsOfType<GameObject>();
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
                    _goParams[go] = prefsList;
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


        void LabelWithEditPrefix(PrefsGUISync sync, string label, Object target, List<PrefsParam> prefsList)
        {
            using (var h = new GUILayout.HorizontalScope())
            {
                SyncToggleList(sync, prefsList);
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

                GUILayout.FlexibleSpace();
            }
        }

        void SyncToggle(PrefsGUISync sync, PrefsParam prefs)
        {
            if (sync != null)
            {
                var key = prefs.key;
                var isSync = !sync._ignoreKeys.Contains(key);

                if (isSync != GUILayout.Toggle(isSync, "", GUILayout.Width(16f)))
                {
                    Undo.RecordObject(sync, "Change PrefsGUI sync flag");
                    EditorUtility.SetDirty(sync);

                    if (isSync) sync._ignoreKeys.Add(key);
                    else sync._ignoreKeys.Remove(key);
                }
            }
        }

        void SyncToggleList(PrefsGUISync sync, List<PrefsParam> prefsList)
        {
            if (sync != null)
            {
                var keys = prefsList.Select(p => p.key).ToList();
                var syncKeys = keys.Except(sync._ignoreKeys);
                var syncKeysCount = syncKeys.Count();
                var isSync = syncKeys.Any();
                var mixed = syncKeysCount != 0 && syncKeysCount != prefsList.Count;

                if (isSync != GUILayout.Toggle(isSync, "", mixed ? "ToggleMixed" : GUI.skin.toggle))
                {
                    isSync = !isSync;
                    Undo.RecordObject(sync, "Change PrefsGUIs sync flag");
                    EditorUtility.SetDirty(sync);

                    if (!isSync)
                    {
                        sync._ignoreKeys.AddRange(syncKeys);
                    }
                    else
                    {
                        keys.ForEach(k => sync._ignoreKeys.Remove(k));
                    }
                }
            }
        }
    }
}