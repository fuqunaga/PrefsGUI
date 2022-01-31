using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionMinMaxSlider
    {
        public static Element CreateMinMaxSlider<T, TMinMax>(this PrefsMinMax<T, TMinMax> prefs, LabelElement label = null)
            where TMinMax : RapidGUI.MinMax<T>, new()
        {
            return WithDefaultButton(prefs,
                UI.MinMaxSlider(
                    label ?? prefs.key,
                    prefs.Get,
                    prefs.Set
                )
            );
        }


        public static Element CreateMinMaxSlider<T, TMinMax>(this PrefsMinMax<T, TMinMax> prefs, LabelElement label,
            TMinMax range)
            where TMinMax : RapidGUI.MinMax<T>, new()
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