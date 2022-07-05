using System;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI.Editor;
using RapidGUI;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrefsGUI.RapidGUI.Editor
{
    public interface IPrefsGUIEditorRapidGUIExtension
    {
        void GUIHeadLine();
        void GUIPrefsLeft(PrefsParam param);
        void GUIGroupLabelLeft(IEnumerable<PrefsParam> prefsList);
    }

    public class PrefsGUIEditorRapidGUI : PrefsGUIEditorRapidGUIBase
    {
        #region static

        private static IPrefsGUIEditorRapidGUIExtension _rapidGUIExtension;
        public static void RegisterExtension(IPrefsGUIEditorRapidGUIExtension ext) => _rapidGUIExtension = ext;

        [MenuItem("Window/PrefsGUI(RapidGUI)")]
        public static void ShowWindow()
        {
            GetWindow<PrefsGUIEditorRapidGUI>("PrefsGUI");
        }

        #endregion


        public enum Order
        {
            AtoZ,
            GameObject,
        }

        Order order;

        // SetCurrentToDefaultWindow setCurrentToDefaultWindow;

        FastScrollView scrollViewAtoZ = new FastScrollView();
        FastScrollView scrollViewGameObject = new FastScrollView();


        private void Awake()
        {
            PrefsAssetUtility.onObjPrefsListChanged += OnGpListChanged;
        }

        private void OnDestroy()
        {
            if (PrefsAssetUtility.onObjPrefsListChanged != null)
                PrefsAssetUtility.onObjPrefsListChanged -= OnGpListChanged;
        }
        
        protected void Update()
        {
            PrefsAssetUtility.UpdateObjPrefsIfNeed();
        }

        void OnGpListChanged()
        {
            scrollViewAtoZ.SetNeedUpdateLayout();
            scrollViewGameObject.SetNeedUpdateLayout();
        }


        bool showComponent;
        string searchWord = "";

        protected override void OnGUIInternal()
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save")) Prefs.Save();
                if (GUILayout.Button("Load")) Prefs.Load();
                if (GUILayout.Button("DeleteAll"))
                {
                    if (PrefsGUIEditorUtility.DisplayDialogDeleteAll())
                    {
                        Prefs.DeleteAll();
                    }
                }
            }

            var prefsAll = PrefsAssetUtility.ObjPrefsList.SelectMany(gp => gp.PrefsAll).ToList();
            var currentToDefaultEnable = !Application.isPlaying && prefsAll.Any(prefs => !prefs.IsDefault);
            using (new RGUI.EnabledScope(currentToDefaultEnable))
            {
                if (GUILayout.Button("Open Current To Default Window"))
                {
                    var setCurrentToDefaultWindow = GetWindow<SetCurrentToDefaultWindowRapidGUI>(true);
                    setCurrentToDefaultWindow.parentWindow = this;
                }
            }

            GUILayout.Space(8f);

            searchWord = RGUI.Field(searchWord, "Search");
            var searchWordLower = searchWord.ToLower();

            using (new GUILayout.HorizontalScope())
            {
                RGUI.PrefixLabel("Order");

                order = (Order)GUILayout.Toolbar((int)order, Enum.GetNames(typeof(Order)));
                EditorGUILayout.Space();
            }

            if (order == Order.GameObject)
            {
                showComponent = GUILayout.Toggle(showComponent, "Show Component");
            }

            GUILayout.Space(8f);

            // horizontal line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            _rapidGUIExtension?.GUIHeadLine();

            switch (order)
            {
                case Order.AtoZ:
                    {
                        var items = prefsAll.Where(p => IsContainWord(p.key, searchWordLower)).OrderBy(p => p.key);
                        scrollViewAtoZ.DoGUI(items, (prefs) =>
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                _rapidGUIExtension?.GUIPrefsLeft(prefs);
                                prefs.DoGUI();
                            }
                        });
                    }
                    break;

                case Order.GameObject:
                    {
                        scrollViewGameObject.DoGUI(PrefsAssetUtility.ObjPrefsList, (gp) =>
                        {
                            var objNameHit = IsContainWord(gp.obj.name, searchWordLower);
                            var componentNameHit = gp.holders.Any(holder => IsComponentContainWord(holder.component, searchWordLower));
                            var prefsHit = gp.PrefsAll.Any(p => IsContainWord(p.key, searchWordLower));

                            if (objNameHit || (showComponent && componentNameHit) || prefsHit)
                            {
                                LabelWithEditPrefix(gp.obj, gp.PrefsAll.ToList(), !showComponent);

                                using (new RGUI.IndentScope(16))
                                {
                                    foreach (var holder in gp.holders)
                                    {
                                        bool needFilter = !objNameHit;

                                        if (showComponent)
                                        {
                                            LabelWithEditPrefix(holder.component, holder.prefsSet.ToList(), true);

                                            needFilter &= !IsComponentContainWord(holder.component, searchWordLower);
                                        }

                                        var prefsSet = needFilter
                                            ? holder.prefsSet.Where(p => IsContainWord(p.key, searchWordLower))
                                            : holder.prefsSet;


                                        using (new RGUI.IndentScope(16))
                                        {
                                            foreach (var prefs in prefsSet)
                                            {
                                                using (new GUILayout.HorizontalScope())
                                                {
                                                    _rapidGUIExtension?.GUIPrefsLeft(prefs);
                                                    prefs.DoGUI();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }
                    break;
            }

            static bool IsContainWord(string word, string searchWordLower) => word.ToLower().Contains(searchWordLower);
            static bool IsComponentContainWord(Object obj, string searchWordLower)
            {
                return obj.name.ToLower().Contains(searchWordLower)
                    || obj.GetType().ToString().ToLower().Contains(searchWordLower);
            }
        }


  

        string LabelWithEditPrefix(Object obj, List<PrefsParam> prefsList, bool editPrefix)
        {
            var prefix = prefsList
                .Select(prefs => prefs.key)
                .Select(PrefsKeyUtility.GetPrefix)
                .FirstOrDefault(s => !string.IsNullOrEmpty(s));

            using (new GUILayout.HorizontalScope())
            {
                _rapidGUIExtension?.GUIGroupLabelLeft(prefsList);

                using (new RGUI.EnabledScope(false))
                {
                    EditorGUILayout.ObjectField(obj, typeof(Object), true);
                }

                if (editPrefix)
                {
                    GUILayout.Label("KeyPrefix");

                    var prefixNew = GUILayout.TextField(prefix, GUILayout.MinWidth(100f));
                    if (prefix != prefixNew)
                    {
                        PrefsGUIEditorUtility.UpdateKeyPrefix(prefixNew, obj, prefsList);
                    }
                }

                GUILayout.FlexibleSpace();
            }

            return prefix;
        }
    }
}