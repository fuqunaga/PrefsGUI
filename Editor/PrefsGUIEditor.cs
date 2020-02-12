using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using RapidGUI;
using System;
using System.Text.RegularExpressions;

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


        bool showComponent;

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

            if (order == Order.GameObject)
            {
                showComponent = GUILayout.Toggle(showComponent, "Show Component");
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
                    {
                        scrollViewGameObject.DoGUI(ObjectPrefsUtility.objPrefsList, (gp) =>
                        {
                            var prefixGo = LabelWithEditPrefix(gp.obj, gp.prefsList, 0);

                            using (new RGUI.IndentScope(16))
                            {
                                foreach (var holder in gp.holders)
                                {
                                    if (showComponent)
                                    {
                                        LabelWithEditPrefix(holder.parent, holder.prefsSet, 1, prefixGo);
                                    }

                                    using (new RGUI.IndentScope(16))
                                    {
                                        foreach (var prefs in holder.prefsSet)
                                        {
                                            using (new GUILayout.HorizontalScope())
                                            {
                                                extension?.GUIPrefsLeft(prefs);
                                                prefs.DoGUI();
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }
                    break;
            }
        }


        const char separator = '.';


        static (string prefix, int idx) GetPrefixAndIdx(string key, int prefixLevel, string parentPrefix)
        {
            var elems = key.Split(separator);
            var prefixNum = elems.Length - 1;
            if (prefixNum >= prefixLevel + 1)
            {
                return (elems[prefixLevel], prefixLevel);
            }
            else if (prefixNum > 0)
            {
                return (elems.First() == parentPrefix)
                    ? (null, prefixLevel)
                    : (elems.First(), 0);
            }

            return (null, 0);
        }


        static string GetPrefix(IEnumerable<string> keys, int prefixLevel, string parentPrefix)
        {
            var prefixes = keys
                .Select(key => GetPrefixAndIdx(key, prefixLevel, parentPrefix).prefix)
                .Where(prefix => (prefixLevel > 0 && prefix != null)) // 空のPrefix：level0有効（複数のPrefixが競合している扱い）、level>0無視
                .Distinct();

            return prefixes.Count() == 1 ? prefixes.First() : "";
        }

        string LabelWithEditPrefix(UnityEngine.Object obj, IEnumerable<PrefsParam> prefsSet, int prefixLevel, string parentPrefix = "")
        {
            var keys = prefsSet.Select(prefs => prefs.key);
            var prefix = GetPrefix(keys, prefixLevel, parentPrefix);

            using (new GUILayout.HorizontalScope())
            {
                extension?.GUIGroupLabelLeft(prefsSet);

                using (new RGUI.EnabledScope(false))
                {
                    EditorGUILayout.ObjectField(obj, typeof(UnityEngine.Object), true);
                }

                GUILayout.Label("KeyPrefix");

                var prefixNew = GUILayout.TextField(prefix, GUILayout.MinWidth(100f));
                if (prefix != prefixNew)
                {
                    Undo.RecordObject(obj, "Change PrefsGUI Prefix");


                    foreach (var prefs in prefsSet)
                    {
                        var elems = prefs.key.Split(separator).ToList();
                        var (p, idx) = GetPrefixAndIdx(prefs.key, prefixLevel, parentPrefix);


                        if (!string.IsNullOrEmpty(p))
                        {

                            elems[idx] = prefixNew; // replace
                        }
                        else
                        {
                            elems.Insert(idx, prefixNew);
                        }

                        prefs.key = string.Join(separator.ToString(), elems.Where(str => !string.IsNullOrEmpty(str)).ToArray());
                    }

                    prefix = prefixNew;
                    EditorUtility.SetDirty(obj);
                }


                GUILayout.FlexibleSpace();
            }

            return prefix;
        }
    }
}