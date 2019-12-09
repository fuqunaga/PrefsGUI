using RapidGUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIExample : PrefsGUIExampleBase
    {
        private WindowLaunchers windows;
        private Rect rect;

        private void Start()
        {
            windows = new WindowLaunchers();
            windows.isWindow = false;
            windows.Add("Part1", typeof(PrefsGUIExample_Part1));
            windows.Add("Part2", typeof(PrefsGUIExample_Part2));
            windows.Add("Part3", typeof(PrefsGUIExample_Part3));
        }


        protected override void DoGUI()
        {
            windows.DoGUI();
            GUILayout.Space(50f);
            GUILayout.Label($"file path: {KVS.PrefsKVSPathSelector.path}");

            if (GUILayout.Button("Save")) Prefs.Save();
            if (GUILayout.Button("DeleteAll")) Prefs.DeleteAll();
        }
    }
}