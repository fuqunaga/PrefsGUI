using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class GUIComponent
    {
        public static bool DoGUIDefaultButton(bool isDefault)
        {
            var label = isDefault ? "default" : "<color=red>default</color>";

            return GUILayout.Button(label, GUILayout.ExpandWidth(false));
        }
    }
}