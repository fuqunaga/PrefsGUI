using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using RapidGUI;

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

        Order order;

        Vector2 scrollPosition;
        SetCurrentToDefaultWindow setCurrentToDefaultWindow;

        protected override void OnGUIInternal()
        {
            using (new GUILayout.HorizontalScope())
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

            var currentToDefaultEnable = !Application.isPlaying && prefsAll.Any(prefs => !prefs.IsDefault);
            using (new RGUI.EnabledScope(currentToDefaultEnable))
            {
                if (GUILayout.Button("Open Current To Default Window"))
                {
                    if (setCurrentToDefaultWindow == null) setCurrentToDefaultWindow = CreateInstance<SetCurrentToDefaultWindow>();
                    setCurrentToDefaultWindow.parentWindow = this;
                    setCurrentToDefaultWindow.ShowUtility();
                }
            }

            GUILayout.Space(8f);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Order");
                order = (Order)GUILayout.SelectionGrid((int)order, System.Enum.GetNames(typeof(Order)), 5);
            }

            GUILayout.Space(8f);

            using (var sc = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = sc.scrollPosition;

                var sync = FindObjectOfType<PrefsGUISync>();
                if (sync != null) GUILayout.Label("sync");


                switch (order)
                {
                    case Order.GameObject:
                        {
                            _goParams.Where(dic => dic.Key != null).OrderBy(dic => dic.Key.name).ToList().ForEach(pair =>
                            {
                                var go = pair.Key;
                                var prefsList = pair.Value;

                                LabelWithEditPrefix(sync, go.name, go, prefsList);
                                using (new RGUI.IndentScope())
                                {
                                    prefsList.ForEach(prefs =>
                                    {
                                        using (new GUILayout.HorizontalScope())
                                        {
                                            SyncToggle(sync, prefs);
                                            prefs.DoGUI();
                                        }
                                    });
                                }
                            });
                        }
                        break;

                    default:
                        prefsAll.ToList().ForEach(prefs =>
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                if (sync != null)
                                {
                                    var key = prefs.key;
                                    var isSync = !sync.ignoreKeys.Contains(key);

                                    if (isSync != GUILayout.Toggle(isSync, "", GUILayout.Width(16f)))
                                    {
                                        Undo.RecordObject(sync, "Change PrefsGUI sync flag");

                                        if (isSync) sync.ignoreKeys.Add(key);
                                        else sync.ignoreKeys.Remove(key);
                                    }
                                }

                                prefs.DoGUI();
                            }
                        });
                        break;

                }

                if ((setCurrentToDefaultWindow != null) && Event.current.type == EventType.Repaint) setCurrentToDefaultWindow.Repaint();
            }
        }


        Dictionary<GameObject, List<PrefsParam>> _goParams = new Dictionary<GameObject, List<PrefsParam>>();

        float _interaval = 1f;
        float _lastTime;

        void Update()
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
                    prefsList.AddRange(SearchChildPrefsParams(comp));
                }

                if (prefsList.Any())
                {
                    _goParams[go] = prefsList;
                }
            }
        }

        void LabelWithEditPrefix(PrefsGUISync sync, string label, GameObject go, List<PrefsParam> prefsList)
        {
            using (new GUILayout.HorizontalScope())
            {
                SyncToggleList(sync, prefsList);
                GUILayout.Label(label);

                const char separator = '.';
                var prefix = prefsList.Select(p => p.key.Split(separator)).Where(sepKeys => sepKeys.Length > 1).FirstOrDefault()?.First();

                GUILayout.Label("KeyPrefix:");

                var prefixNew = GUILayout.TextField(prefix, GUILayout.MinWidth(100f));
                if (prefix != prefixNew)
                {
                    Undo.RecordObject(go, "Change PrefsGUI Prefix");
                    
                    var prefixWithSeparator = string.IsNullOrEmpty(prefixNew) ? "" : prefixNew + separator;
                    prefsList.ForEach(p =>
                    {
                        p.key = prefixWithSeparator + p.key.Split(separator).Last();
                    });

                    go.GetComponents<Component>().ToList().ForEach(c => EditorUtility.SetDirty(c));
                }

                GUILayout.FlexibleSpace();
            }
        }

        void SyncToggle(PrefsGUISync sync, PrefsParam prefs)
        {
            if (sync != null)
            {
                var key = prefs.key;
                var isSync = !sync.ignoreKeys.Contains(key);

                if (isSync != GUILayout.Toggle(isSync, "", GUILayout.Width(16f)))
                {
                    Undo.RecordObject(sync, "Change PrefsGUI sync flag");
                    EditorUtility.SetDirty(sync);

                    if (isSync) sync.ignoreKeys.Add(key);
                    else sync.ignoreKeys.Remove(key);
                }
            }
        }

        void SyncToggleList(PrefsGUISync sync, List<PrefsParam> prefsList)
        {
            if (sync != null)
            {
                var keys = prefsList.Select(p => p.key).ToList();
                var syncKeys = keys.Except(sync.ignoreKeys);
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
                        sync.ignoreKeys.AddRange(syncKeys);
                    }
                    else
                    {
                        keys.ForEach(k => sync.ignoreKeys.Remove(k));
                    }
                }
            }
        }
    }
}