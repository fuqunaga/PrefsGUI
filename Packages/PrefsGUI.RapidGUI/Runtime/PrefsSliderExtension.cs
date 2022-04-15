using System.Collections.Generic;
using RapidGUI;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsSliderExtension
    {
        private static readonly Dictionary<PrefsParam, bool> isOpenTable = new();

        public static bool DoGUISlider<T>(this PrefsSlider<T> prefs, string label = null)
            where T : struct
        {
            return prefs.DoGUISlider(prefs.defaultMin, prefs.defaultMax, label);
        }

        public static bool DoGUISlider<T>(this PrefsSlider<T> prefs, T max, string label = null)
            where T : struct
        {
            return prefs.DoGUISlider(prefs.defaultMin, max, label);
        }
        
        public static bool DoGUISlider<T>(this PrefsSlider<T> prefs, T min, T max, string label = null)
            where T : struct
        {
            isOpenTable.TryGetValue(prefs, out var isOpen);
            
            var ret = prefs.DoGUIStandard((T v) => RGUI.Slider(v, min, max, label ?? prefs.key, ref isOpen));

            isOpenTable[prefs] = isOpen;

            return ret;
        }
    }
}