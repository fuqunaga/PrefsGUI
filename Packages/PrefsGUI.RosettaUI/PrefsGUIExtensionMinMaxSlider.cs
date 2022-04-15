using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionMinMaxSlider
    {
        public static Element CreateMinMaxSlider<T>(this PrefsMinMax<T> prefs, LabelElement label = null)
        {
            return WithDefaultButton(prefs,
                UI.MinMaxSlider(
                    label ?? prefs.key,
                    prefs.Get,
                    prefs.Set
                )
            );
        }

        public static Element CreateMinMaxSlider<T>(this PrefsMinMax<T> prefs, T max)
        {
            return CreateMinMaxSlider(prefs, null, max);
        }
        
        public static Element CreateMinMaxSlider<T>(this PrefsMinMax<T> prefs, T min, T max)

        {
            return CreateMinMaxSlider(prefs, null, min, max);
        }
        
        public static Element CreateMinMaxSlider<T>(this PrefsMinMax<T> prefs, LabelElement label, T max)

        {
            return CreateMinMaxSlider(prefs, label, default, max);
        }
        
        public static Element CreateMinMaxSlider<T>(this PrefsMinMax<T> prefs, LabelElement label, T min, T max)
        {
            var range = new PrefsMinMax<T>.MinMax()
            {
                min = min,
                max = max
            };
            
            return CreateMinMaxSlider(prefs, label, range);
        }
        
        public static Element CreateMinMaxSlider<T>(this PrefsMinMax<T> prefs, LabelElement label, PrefsMinMax<T>.MinMax range)
        {
            return WithDefaultButton(prefs,
                UI.MinMaxSlider(
                    label ?? prefs.key,
                    prefs.Get,
                    prefs.Set,
                    range
                )
            );
        }

        static Element WithDefaultButton(PrefsParam prefs, Element element) =>
            UI.Row(element, prefs.CreateDefaultButtonElement());
    }
}