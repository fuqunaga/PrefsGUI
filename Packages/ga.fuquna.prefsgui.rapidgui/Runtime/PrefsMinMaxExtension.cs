using RapidGUI;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsMinMaxExtension
    {
        public static bool DoGUISlider<T>(this PrefsMinMax<T> prefs, string label = null)
        {
            return prefs.DoGUISlider(prefs.defaultMin, prefs.defaultMax, label);
        }

        public static bool DoGUISlider<T>(this PrefsMinMax<T> prefs, T max, string label = null)
        {
            return prefs.DoGUISlider(prefs.defaultMin, max, label);
        }
        
        public static bool DoGUISlider<T>(this PrefsMinMax<T> prefs, T rangeMin, T rangeMax, string label = null)
        {
            return prefs.DoGUIStandard((v) =>
            {
                RGUI.MinMaxSlider(ref v.min, ref v.max, rangeMin, rangeMax, label ?? prefs.key);
                return v;
            });
        }
    }
}