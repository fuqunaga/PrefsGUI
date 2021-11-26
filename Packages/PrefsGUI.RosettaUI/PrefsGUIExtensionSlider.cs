using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RosettaUI;
using UnityEngine;
using Binder = RosettaUI.Binder;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionSlider
    {
        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, LabelElement label = null)
        {
            IGetter<T> minGetter = null;
            IGetter<T> maxGetter = null;
            if (prefs is IPrefsSlider<T> prefsSlider)
            {
                minGetter = ConstGetter.Create(prefsSlider.defaultMin);
                maxGetter = ConstGetter.Create(prefsSlider.defaultMax);
            }

            return CreateSlider(prefs, label, minGetter, maxGetter);
        }

        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, T max)
        {
            return CreateSlider<T>(prefs, null, default, max);
        }
        
        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, T min, T max)
        {
            return CreateSlider(prefs, null, min, max);
        }
        
        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, LabelElement label, T max)
        {
            return CreateSlider(prefs, label, default, max);
        }

        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, LabelElement label, T min, T max)
        {
            return CreateSlider(prefs, label, ConstGetter.Create(min), ConstGetter.Create(max));
        }

        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, LabelElement label, IGetter<T> minGetter, IGetter<T> maxGetter)
        {
            return UI.Row(
                _CreateSlider(prefs, label, minGetter, maxGetter),
                prefs.CreateDefaultButtonElement()
            );
            
        }

        private static Element _CreateSlider<T>(PrefsParamOuter<T> prefs, LabelElement label, IGetter<T> minGetter, IGetter<T> maxGetter)
        {
            return UI.Slider(
                label ?? prefs.key,
                Binder.Create(prefs.Get, prefs.Set),
                minGetter,
                maxGetter
            );
        }
    }
}