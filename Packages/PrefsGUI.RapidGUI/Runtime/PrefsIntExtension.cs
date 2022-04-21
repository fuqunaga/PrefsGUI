using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsIntExtension
    {
        public static bool DoGUISlider(this PrefsSlider<int> prefs, int min, int max, string label = null)
        {
            return prefs.DoGUIStandard((v) => RGUI.Slider(v, min, max, label ?? prefs.key));
        }

        public static bool DoGUIToolbar(this PrefsSlider<int> prefs, string[] texts, string label = null)
        {
            return prefs.DoGUIStandard((v) =>
            {
                using (new GUILayout.HorizontalScope())
                {
                    RGUI.PrefixLabel(label ?? prefs.key);
                    return GUILayout.Toolbar(v, texts);
                }
            });
        }

        public static bool DoGUISelectionGrid(this PrefsSlider<int> prefs, string[] texts, int xCount, string label = null)
        {
            return prefs.DoGUIStandard((v) =>
            {
                using (new GUILayout.HorizontalScope())
                {
                    RGUI.PrefixLabel(label ?? prefs.key);
                    return GUILayout.SelectionGrid(v, texts, xCount);
                }
            });
        }
    }
}