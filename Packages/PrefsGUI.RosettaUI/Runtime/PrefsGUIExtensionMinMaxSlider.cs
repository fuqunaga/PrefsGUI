using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionMinMaxSlider
    {
        public static Element CreateMinMaxSlider<T>(this PrefsMinMax<T> prefs, LabelElement label = null)
        {
            return AddDefaultButtonAndSyncSubscription(prefs,
                UI.MinMaxSlider(
                    label ?? UI.Label(() => prefs.key),
                    prefs.Get,
                    v => prefs.Set(v)
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
            return AddDefaultButtonAndSyncSubscription(prefs,
                UI.MinMaxSlider(
                    label ?? UI.Label(() => prefs.key),
                    prefs.Get,
                    v => prefs.Set(v),
                    range
                )
            );
        }

        static Element AddDefaultButtonAndSyncSubscription(PrefsParam prefs, Element element)
        {
            var ret = UI.Row(element, prefs.CreateDefaultButtonElement());
            PrefsGUIExtension.SubscribeSyncedFlag(prefs, ret);

            return ret;
        }
    }
}