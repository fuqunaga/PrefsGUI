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

        SetCurrentToDefaultWindow setCurrentToDefaultWindow;

        FastScrollViewVertical scrollViewAtoZ = new FastScrollViewVertical();
        FastScrollViewVertical scrollViewGameObject = new FastScrollViewVertical();



        private void Awake()
        {
            GameObjectPrefsUtility.onGoPrefsListChanged += OnGpListChanged;
        }

        private void OnDestroy()
        {
            GameObjectPrefsUtility.onGoPrefsListChanged -= OnGpListChanged;
        }

        void OnGpListChanged()
        {
            scrollViewAtoZ.SetNeedUpdateLayout();
            scrollViewGameObject.SetNeedUpdateLayout();
        }


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

            var prefsAll = GameObjectPrefsUtility.goPrefsList.SelectMany(gp => gp.prefsList).ToList();
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

                order = (Order)GUILayout.Toolbar((int)order, System.Enum.GetNames(typeof(Order)));
                EditorGUILayout.Space();
            }

            GUILayout.Space(8f);
            //needUpdateLayout = GUILayout.Toggle(needUpdateLayout, nameof(needUpdateLayout));

            var sync = FindObjectOfType<PrefsGUISync>();
            if (sync != null) GUILayout.Label("Sync");

            switch (order)
            {
                case Order.AtoZ:
                    scrollViewAtoZ.DoGUI(prefsAll.OrderBy(p => p.key), (prefs) =>
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            if (sync != null)
                            {
                                var key = prefs.key;
                                var isSync = !sync.ignoreKeys.Contains(key);

                                if (isSync != GUILayout.Toggle(isSync, GUIContent.none, ToggleWidth))
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

                case Order.GameObject:
                    scrollViewGameObject.DoGUI(GameObjectPrefsUtility.goPrefsList, (gp) =>
                    {
                        LabelWithEditPrefix(sync, gp);

                        using (new RGUI.IndentScope())
                        {
                            gp.prefsList.ToList().ForEach(prefs =>
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    SyncToggle(sync, prefs);
                                    prefs.DoGUI();
                                }
                            });
                        }
                    });
                    break;
            }
        }


        public void DoGUIGameObject(PrefsGUISync sync)
        {
            GameObjectPrefsUtility.goPrefsList.ForEach(gp =>
            {
                LabelWithEditPrefix(sync, gp);

                using (new RGUI.IndentScope())
                {
                    gp.prefsList.ToList().ForEach(prefs =>
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



        void LabelWithEditPrefix(PrefsGUISync sync, GameObjectPrefsUtility.GoPrefs gp)
        {
            var prefsList = gp.prefsList;
            using (new GUILayout.HorizontalScope())
            {
                SyncToggleList(sync, prefsList);

                using (new RGUI.EnabledScope(false))
                {
                    EditorGUILayout.ObjectField(gp.go, typeof(GameObject), true);
                }

                const char separator = '.';
                var prefix = prefsList.Select(p => p.key.Split(separator)).Where(sepKeys => sepKeys.Length > 1).FirstOrDefault()?.First();

                GUILayout.Label("KeyPrefix");

                var prefixNew = GUILayout.TextField(prefix, GUILayout.MinWidth(100f));
                if (prefix != prefixNew)
                {
                    var go = gp.go;
                    Undo.RecordObject(go, "Change PrefsGUI Prefix");

                    var prefixWithSeparator = string.IsNullOrEmpty(prefixNew) ? "" : prefixNew + separator;
                    prefsList.ToList().ForEach(p =>
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

                if (isSync != GUILayout.Toggle(isSync, GUIContent.none, ToggleWidth))
                {
                    Undo.RecordObject(sync, "Change PrefsGUI sync flag");
                    EditorUtility.SetDirty(sync);

                    if (isSync) sync.ignoreKeys.Add(key);
                    else sync.ignoreKeys.Remove(key);
                }
            }
        }

        void SyncToggleList(PrefsGUISync sync, IEnumerable<PrefsParam> prefsList)
        {
            if (sync != null)
            {
                var keys = prefsList.Select(p => p.key).ToList();
                var syncKeys = keys.Except(sync.ignoreKeys);

                var isSync = ToggleMixed(syncKeys.Count(), keys.Count);
                if (isSync.HasValue)
                {
                    Undo.RecordObject(sync, "Change PrefsGUIs sync flag");
                    EditorUtility.SetDirty(sync);

                    if (!isSync.Value)
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