using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

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
            var prefsList = PrefsList.Where(prefs => !prefs.IsDefault).ToList();
            prefsList.Where(prefs => !checkedList.ContainsKey(prefs.key)).Select(prefs => prefs.key).ToList().ForEach(key => checkedList[key] = true);


            EditorGUILayout.HelpBox("\nSelect Prefs to change Default.\n", MessageType.None);

            if (checkAll != GUILayout.Toggle(checkAll, ""))
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
                        prefs.OnGUI();
                    }
                });
            }


            var checkPrefsList = prefsList.Where(prefs => checkedList[prefs.key]).ToList();

            GUILayout.Space(8f);

            GUI.enabled = checkPrefsList.Any();
            if (GUILayout.Button("SetCurrentToDefault"))
            {
                // Search Objects to recoard that has PrefsParams
                var components = FindObjectsOfType<MonoBehaviour>()
                    .Where(c =>
                    {
                        var t = c.GetType();
                        return Assembly.GetAssembly(t).GetName().Name.StartsWith("Assembly-CSharp") // skip unity classes
                        && IsContainPrefsParam(t);
                    })
                    .ToArray();

                Undo.RecordObjects(components, "Set PrefsGUI default value");


                // SetCurrent To Default
                checkPrefsList.ForEach(prefs =>
                {
                    prefs.SetCurrentToDefault();
                });

                Close();
                parentWindow.Repaint();
            }
            GUI.enabled = true;
        }

        HashSet<System.Type> _appearedTypes = new HashSet<System.Type>();
        bool IsContainPrefsParam(System.Type type)
        {
            if (_appearedTypes.Contains(type)) return false;
            _appearedTypes.Add(type);
            return type.IsSubclassOf(typeof(PrefsParam)) || type.GetFields().Any(fi => IsContainPrefsParam(fi.FieldType));
        }
    }
}