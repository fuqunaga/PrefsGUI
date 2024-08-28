using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsBoundsIntExtension
    {
        public static bool DoGUISlider(this PrefsSlider<BoundsInt> prefs, int max, string label = null)
        {
            return prefs.DoGUISlider(new BoundsInt(Vector3Int.one * max, Vector3Int.one * max), label);
        }
    }
}