using RapidGUI;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsMinMaxExtension
    {
        public static bool DoGUISlider<T, TMinMax>(this PrefsMinMax<T, TMinMax> prefs, string label = null)
            where TMinMax : MinMax<T>, new()
        {
            return prefs.DoGUISlider(prefs.defaultMin, prefs.defaultMax, label);
        }

        public static bool DoGUISlider<T, TMinMax>(this PrefsMinMax<T, TMinMax> prefs, T max, string label = null)
            where TMinMax : MinMax<T>, new()
        {
            return prefs.DoGUISlider(prefs.defaultMin, max, label);
        }
        
        public static bool DoGUISlider<T, TMinMax>(this PrefsMinMax<T, TMinMax> prefs, T rangeMin, T rangeMax, string label = null)
            where TMinMax : MinMax<T>, new()
        {
            return prefs.DoGUIStandard((v) =>
            {
                RGUI.MinMaxSlider(ref v.min, ref v.max, rangeMin, rangeMax, label ?? prefs.key);
                return v;
            });
        }
    }
}