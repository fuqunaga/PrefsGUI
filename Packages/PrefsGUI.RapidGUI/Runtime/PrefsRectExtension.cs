using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsRectExtension
    {
        public static bool DoGUISlider(this PrefsSlider<Rect> prefs, float max, string label = null)
        {
            return prefs.DoGUISlider(new Rect(max, max, max, max), label);
        }
    }
}