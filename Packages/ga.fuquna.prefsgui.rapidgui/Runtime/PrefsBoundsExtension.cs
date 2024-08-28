using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsBoundsExtension
    {
        public static bool DoGUISlider(this PrefsSlider<Bounds> prefs, float max, string label = null)
        {
            return prefs.DoGUISlider(new Bounds(Vector3.one * max, Vector3.one * max), label);
        }
    }
}