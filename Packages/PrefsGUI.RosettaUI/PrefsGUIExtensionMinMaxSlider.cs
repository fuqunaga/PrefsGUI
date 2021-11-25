using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RosettaUI;
using Binder = RosettaUI.Binder;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionMinMaxSlider
    {
        // TODO: dirty way. avoid boxing
        private static readonly Dictionary<Type, (FieldInfo, FieldInfo)?> MinMaxFieldInfoTable = new Dictionary<Type, (FieldInfo, FieldInfo)?>();
        
        
        #region MinMaxSlider

        public static Element CreateSlider<T, TMinMax>(this PrefsMinMax<T, TMinMax> prefs, LabelElement label = null)
            where TMinMax : RapidGUI.MinMax<T>, new()
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
        
        public static Element CreateSlider<T, TMinMax>(this PrefsMinMax<T, TMinMax> prefs, T max)
            where TMinMax : RapidGUI.MinMax<T>, new()
        {
            return CreateSlider(prefs, null, default, max);
        }
        
        public static Element CreateSlider<T, TMinMax>(this PrefsMinMax<T, TMinMax> prefs, T min, T max)
            where TMinMax : RapidGUI.MinMax<T>, new()
        {
            return CreateSlider(prefs, null, min, max);
        }
        
        public static Element CreateSlider<T, TMinMax>(this PrefsMinMax<T, TMinMax> prefs,LabelElement label, T max)
            where TMinMax : RapidGUI.MinMax<T>, new()
        {
            return CreateSlider(prefs, label, default, max);
        }

        
        public static Element CreateSlider<T, TMinMax>(this PrefsMinMax<T, TMinMax> prefs, LabelElement label, T min, T max)
            where TMinMax : RapidGUI.MinMax<T>, new()
        {
            return CreateSlider(prefs, label, ConstGetter.Create(min), ConstGetter.Create(max));
        }
        
        public static Element CreateSlider<TMinMax, TValue>(this PrefsParamOuter<TMinMax> prefs, LabelElement label, IGetter<TValue> minGetter, IGetter<TValue> maxGetter)
        {
            return UI.Row(
                _CreateMinMaxSlider(prefs,label, minGetter, maxGetter),
                prefs.CreateDefaultButton()
            );
        }

        #endregion
        
        
        private static Element _CreateMinMaxSlider<T, TValue>(PrefsParamOuter<T> prefs, LabelElement label, IGetter<TValue> minGetter, IGetter<TValue> maxGetter)
        {
            var type = typeof(T);
            if (!MinMaxFieldInfoTable.TryGetValue(type, out var pair))
            {
                var min = typeof(T).GetField("min");
                var max = typeof(T).GetField("max");

                MinMaxFieldInfoTable[type] = pair = (min, max);
            }

            var (minField, maxField) = pair.Value;

            return UI.MinMaxSlider(
                label ?? prefs.key,
                Binder.Create(
                    () =>
                    {
                        var minMax = prefs.Get() as RapidGUI.MinMax<TValue>;
                        return MinMax.Create(minMax.min, minMax.max);
                    },
                    v => {
                        var minMax = (T) Activator.CreateInstance(typeof(T));
                        minField.SetValue(minMax, v.min);
                        maxField.SetValue(minMax, v.max);

                        prefs.Set(minMax);
                    }),
                minGetter,
                maxGetter
            );
        }
    }
}