using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using RapidGUI;

namespace PrefsGUI.Editor
{
    public interface IPrefsGUIEditorExtension
    {
        void GUIHeadLine();
        void GUIPrefsLeft(PrefsParam param);
        void GUIGroupLabelLeft(IEnumerable<PrefsParam> prefsList);
    }

    public class PrefsGUIEditor : PrefsGUIEditorBase
    {
        #region static

        private static IPrefsGUIEditorExtension extension;
        public static void RegistExtension(IPrefsGUIEditorExtension ext) => extension = ext;

        [MenuItem("Window/PrefsGUI")]
        public static void ShowWindow()
        {
            GetWindow<PrefsGUIEditor>("PrefsGUI");
        }

        #endregion


        public enum Order
        {
            AtoZ,
            GameObject,
        }

        Order order;

        SetCurrentToDefaultWindow setCurrentToDefaultWindow;

        FastScrollView scrollViewAtoZ = new FastScrollView();
        FastScrollView scrollViewGameObject = new FastScrollView();


        private void Awake()
        {
            ObjectPrefsUtility.onGoPrefsListChanged += OnGpListChanged;
        }

        private void OnDestroy()
        {
            if (ObjectPrefsUtility.onGoPrefsListChanged != null)
                ObjectPrefsUtility.onGoPrefsListChanged -= OnGpListChanged;
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
                    if (EditorUtility.DisplayDialog("DeleteAll", "Are you sure to delete all current prefs parameters?",
                        "DeleteAll", "Don't Delete"))
                    {
                        Prefs.DeleteAll();
                    }
                }
            }

            var prefsAll = ObjectPrefsUtility.objPrefsList.SelectMany(gp => gp.prefsList).ToList();
            var currentToDefaultEnable = !Application.isPlaying && prefsAll.Any(prefs => !prefs.IsDefault);
            using (new RGUI.EnabledScope(currentToDefaultEnable))
            {
                if (GUILayout.Button("Open Current To Default Window"))
                {
                    if (setCurrentToDefaultWindow == null)
                        setCurrentToDefaultWindow = CreateInstance<SetCurrentToDefaultWindow>();
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

            extension?.GUIHeadLine();

            switch (order)
            {
                case Order.AtoZ:
                    scrollViewAtoZ.DoGUI(prefsAll.OrderBy(p => p.key), (prefs) =>
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            extension?.GUIPrefsLeft(prefs);
                            prefs.DoGUI();
                        }
                    });
                    break;

                case Order.GameObject:
                    scrollViewGameObject.DoGUI(ObjectPrefsUtility.objPrefsList, (gp) =>
                    {
                        LabelWithEditPrefix(gp);

                        using (new RGUI.IndentScope())
                        {
                            gp.prefsList.ToList().ForEach(prefs =>
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    extension?.GUIPrefsLeft(prefs);
                                    prefs.DoGUI();
                                }
                            });
                        }
                    });
                    break;
            }
        }


        void LabelWithEditPrefix(ObjectPrefsUtility.ObjPrefs objPrefs)
        {
            var prefsList = objPrefs.prefsList;
            using (new GUILayout.HorizontalScope())
            {
                extension?.GUIGroupLabelLeft(prefsList);

                using (new RGUI.EnabledScope(false))
                {
                    EditorGUILayout.ObjectField(objPrefs.obj, typeof(Object), true);
                }

                const char separator = '.';
                var prefix = prefsList.Select(p => p.key.Split(separator))
                    .FirstOrDefault(sepKeys => sepKeys.Length > 1)?.First();

                GUILayout.Label("KeyPrefix");

                var prefixNew = GUILayout.TextField(prefix, GUILayout.MinWidth(100f));
                if (prefix != prefixNew)
                {
                    var go = objPrefs.obj;
                    Undo.RecordObject(go, "Change PrefsGUI Prefix");

                    var prefixWithSeparator = string.IsNullOrEmpty(prefixNew) ? "" : prefixNew + separator;

                    foreach (var prefs in prefsList)
                    {
                        prefs.key = prefixWithSeparator + prefs.key.Split(separator).Last();
                        EditorUtility.SetDirty(objPrefs.GetPrefsParent(prefs));
                    }
                }

                GUILayout.FlexibleSpace();
            }
        }
    }
}