using RapidGUI;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsFloatExtension
    {
        public static bool DoGUISlider(this PrefsSlider<float> prefs, float min, float max, string label = null)
        {
            return prefs.DoGUIStandard((v) => RGUI.Slider(v, min, max, label ?? prefs.key));
        }
    }
}