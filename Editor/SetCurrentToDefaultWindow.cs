using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace PrefsGUI
{
    public class SetCurrentToDefaultWindow : PrefsGUIEditorBase
    {
        public PrefsGUIEditor parentWindow;
        Dictionary<string, bool> checkedList = new Dictionary<string, bool>();

        Vector2 scrollPosition;
        bool checkAll;

        protected override void OnGUIInternal()
        {
            var prefsNonDefaults = prefsAll.Where(prefs => !prefs.IsDefault).ToList();
            prefsNonDefaults.Where(prefs => !checkedList.ContainsKey(prefs.key)).Select(prefs => prefs.key).ToList().ForEach(key => checkedList[key] = true);


            EditorGUILayout.HelpBox("\nSelect Prefs to change Default.\n", MessageType.None);

            if (checkAll != GUILayout.Toggle(checkAll, ""))
            {
                checkAll = !checkAll;
                prefsNonDefaults.ForEach(prefs => checkedList[prefs.key] = checkAll);
            }

            using (var sc = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = sc.scrollPosition;
                prefsNonDefaults.ForEach(prefs =>
                {
                    var key = prefs.key;
                    bool check = checkedList[key];

                    using (new GUILayout.HorizontalScope())
                    {
                        if (check != GUILayout.Toggle(check, "", GUILayout.Width(20f))) checkedList[key] = !check;
                        prefs.DoGUI();
                    }
                });
            }


            var checkedPrefs = prefsNonDefaults.Where(prefs => checkedList[prefs.key]).ToList();

            GUILayout.Space(8f);

            GUI.enabled = checkedPrefs.Any();
            if (GUILayout.Button("SetCurrentToDefault"))
            {
                // Search Objects to recoard that has PrefsParams
                var monos = FindObjectsOfType<MonoBehaviour>()
                    .Where(mono => Assembly.GetAssembly(mono.GetType()).GetName().Name.StartsWith("Assembly-CSharp")) // skip unity classes
                    .Where(mono =>
                    {
                        var prefs = SearchChildPrefsParams(mono);
                        return checkedPrefs.Any(p => prefs.Contains(p));
                    })
                    .ToArray();

     
                Undo.RecordObjects(monos, "Set PrefsGUI default value");


                // SetCurrent To Default
                checkedPrefs.ForEach(prefs => prefs.SetCurrentToDefault());

                monos.ToList().ForEach(mono => EditorUtility.SetDirty(mono));


                Close();
                parentWindow.Repaint();
            }
            GUI.enabled = true;
        }
    }
}