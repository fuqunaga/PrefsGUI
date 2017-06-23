using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace PrefsGUI
{

    public abstract class PrefsGUIEditorBase : EditorWindow
    {
        protected IEnumerable<PrefsParam> PrefsList { get { return PrefsParam.all.Values.OrderBy(prefs => prefs.key); } }

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
    }

    public class PrefsGUIEditor : PrefsGUIEditorBase
    {
        [MenuItem("Window/PrefsGUI")]
        public static void ShowWindow()
        {
            GetWindow<PrefsGUIEditor>("PrefsGUI");
        }

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
                if ( setCurrentToDefaultWindow == null ) setCurrentToDefaultWindow = CreateInstance<SetCurrentToDefaultWindow>();
                setCurrentToDefaultWindow.parentWindow = this;
                setCurrentToDefaultWindow.ShowUtility();
            }
            GUI.enabled = true;

            GUILayout.Space(8f);

            using (var sc = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = sc.scrollPosition;

                var sync = FindObjectOfType<PrefsGUISync>();

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
                                if (isSync) sync._ignoreKeys.Add(key);
                                else sync._ignoreKeys.Remove(key);
                            }
                        }

                        prefs.OnGUI();
                    }
                });
            }

            if ((setCurrentToDefaultWindow!= null) && Event.current.type == EventType.repaint) setCurrentToDefaultWindow.Repaint();
        }
    }


    public class SetCurrentToDefaultWindow : PrefsGUIEditorBase
    {
        public PrefsGUIEditor parentWindow;
        Dictionary<string, bool> checkedList = new Dictionary<string, bool>();

        Vector2 scrollPosition;
        bool checkAll;

        protected override void OnGUIInternal()
        {
            var prefsList = PrefsList.Where(prefs => !prefs.IsDefault).ToList();
            prefsList.Where(prefs => !checkedList.ContainsKey(prefs.key)).Select(prefs => prefs.key).ToList().ForEach(key => checkedList[key] = true);


            EditorGUILayout.HelpBox("\nSelect Prefs to change Default.\n", MessageType.None);

            if ( checkAll != GUILayout.Toggle(checkAll, ""))
            {
                checkAll = !checkAll;
                prefsList.ForEach(prefs => checkedList[prefs.key] = checkAll);
            }

            using (var sc = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = sc.scrollPosition;
                prefsList.ForEach(prefs =>
                {
                    var key = prefs.key;
                    bool check = checkedList[key];

                    using (var h0 = new GUILayout.HorizontalScope())
                    {
                        if (check != GUILayout.Toggle(check, "", GUILayout.Width(20f))) checkedList[key] = !check;
                        GUI.enabled = false;
                        prefs.OnGUI();
                        GUI.enabled = true;
                    }
                });
            }


            var checkPrefsList = prefsList.Where(prefs => checkedList[prefs.key]).ToList();

            GUILayout.Space(8f);

            GUI.enabled = checkPrefsList.Any();
            if ( GUILayout.Button("SetCurrentToDefault"))
            {
                checkPrefsList.ForEach(prefs =>
                {
                    prefs.SetCurrentToDefault();
                });

                Close();
                parentWindow.Repaint();
            }
            GUI.enabled = true;
        }
    }
}