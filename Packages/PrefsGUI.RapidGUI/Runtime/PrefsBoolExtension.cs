using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsBoolExtension
    {
        public static bool DoGUIToggle(this PrefsParam<bool> prefs, string label = null)
        {
            return prefs.DoGUIStandard((v) => GUILayout.Toggle(v, label ?? prefs.key));
        }
    }
}