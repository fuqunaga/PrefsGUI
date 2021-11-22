using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RosettaUI;
using UnityEngine;
using Binder = RosettaUI.Binder;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtension
    {
        private static readonly Dictionary<Type, MethodInfo> CreateMinMaxSliderMethodTable =
            new Dictionary<Type, MethodInfo>();


        // TODO: dirty way. avoid boxing
        private static readonly Dictionary<Type, (FieldInfo, FieldInfo)?> MinMaxFieldInfoTable =
            new Dictionary<Type, (FieldInfo, FieldInfo)?>();

        public static Element CreateElement<T>(this PrefsParamOuter<T> prefs, Action<T> onValueChanged = null)
        {
            return CreateElement(prefs, null, onValueChanged);
        }

        public static Element CreateElement<T>(this PrefsParamOuter<T> prefs, LabelElement label,
            Action<T> onValueChanged = null)
        {
            return UI.Row(
                UI.Field(
                    label ?? prefs.key,
                    () => prefs.Get(),
                    v =>
                    {
                        prefs.Set(v);
                        onValueChanged?.Invoke(v);
                    }),
                prefs.CreateDefaultButton()
            );
        }

        public static Element CreateElement<TPrefs0, TPrefs1, TOuter0, TOuter1>(
            this PrefsSet<TPrefs0, TPrefs1, TOuter0, TOuter1> prefs)
            where TPrefs0 : PrefsParamOuter<TOuter0>
            where TPrefs1 : PrefsParamOuter<TOuter1>
        {
            return UI.Fold(
                prefs.key,
                prefs.prefs0.CreateElement(),
                prefs.prefs1.CreateElement()
            );
        }


        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, Action<T> onValueChanged = null)
        {
            return CreateSlider(prefs, null, onValueChanged);
        }

        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, LabelElement label,
            Action<T> onValueChanged = null)
        {
            IGetter<T> minGetter = null;
            IGetter<T> maxGetter = null;
            if (prefs is IPrefsSlider<T> prefsSlider)
            {
                minGetter = ConstGetter.Create(prefsSlider.defaultMin);
                maxGetter = ConstGetter.Create(prefsSlider.defaultMax);
            }

            return CreateSlider(prefs, label, minGetter, maxGetter, onValueChanged);
        }

        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, T max, Action<T> onValueChanged = null)
        {
            return CreateSlider<T>(prefs, null, default, max, onValueChanged);
        }
        
        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, T min, T max, Action<T> onValueChanged = null)
        {
            return CreateSlider(prefs, null, min, max, onValueChanged);
        }
        
        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, LabelElement label, T max, Action<T> onValueChanged = null)
        {
            return CreateSlider(prefs, label, default, max, onValueChanged);
        }

        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, LabelElement label, T min, T max,
            Action<T> onValueChanged = null)
        {
            return CreateSlider(prefs, label, ConstGetter.Create(min), ConstGetter.Create(max), onValueChanged);
        }

        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, LabelElement label, IGetter<T> minGetter,
            IGetter<T> maxGetter,
            Action<T> onValueChanged = null)
        {
            var type = typeof(T);
            var valueType = GetMinMaxValueType(type);

            Element slider = null;
            if (valueType != null)
            {
                if (!CreateMinMaxSliderMethodTable.TryGetValue(type, out var mi))
                {
                    mi = typeof(PrefsGUIExtension)
                        .GetMethod(nameof(_CreateMinMaxSlider), BindingFlags.Static | BindingFlags.NonPublic)
                        ?.MakeGenericMethod(type, valueType);

                    CreateMinMaxSliderMethodTable[type] = mi;
                }

                slider = (Element) mi?.Invoke(null, new object[] {prefs, minGetter, maxGetter, onValueChanged});
            }
            else
            {
                slider = _CreateSlider(prefs, label, minGetter, maxGetter, onValueChanged);
            }

            return UI.Row(
                slider,
                prefs.CreateDefaultButton()
            );

            static Type GetMinMaxValueType(Type type)
            {
                while (type != null)
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RapidGUI.MinMax<>))
                        return type.GetGenericArguments().First();

                    type = type.BaseType;
                }

                return null;
            }
        }

        private static Element _CreateSlider<T>(PrefsParamOuter<T> prefs, LabelElement label, IGetter<T> minGetter,
            IGetter<T> maxGetter,
            Action<T> onValueChanged)
        {
            return UI.Slider(
                label ?? prefs.key,
                Binder.Create(
                    prefs.Get,
                    v =>
                    {
                        prefs.Set(v);
                        onValueChanged?.Invoke(v);
                    }
                ),
                minGetter,
                maxGetter
            );
        }

        private static Element _CreateMinMaxSlider<T, TValue>(PrefsParamOuter<T> prefs, IGetter<TValue> minGetter,
            IGetter<TValue> maxGetter, Action<T> onValueChanged)
        {
            var type = typeof(T);
            if (!MinMaxFieldInfoTable.TryGetValue(type, out var pair))
            {
                var min = typeof(T).GetField("min");
                var max = typeof(T).GetField("max");

                MinMaxFieldInfoTable[type] = pair = (min, max);
            }

            var (minField, maxField) = pair.Value;

            Func<MinMax<TValue>> getFunc = () =>
            {
                var minMax = prefs.Get() as RapidGUI.MinMax<TValue>;
                return MinMax.Create(minMax.min, minMax.max);
            };

            return UI.MinMaxSlider(
                prefs.key,
                Binder.Create(
                    () => getFunc(),
                    v => {
                        var minMax = (T) Activator.CreateInstance(typeof(T));
                        minField.SetValue(minMax, v.min);
                        maxField.SetValue(minMax, v.max);

                        prefs.Set(minMax);
                        onValueChanged?.Invoke(minMax);
                    }),
                minGetter,
                maxGetter
            );
        }


        private static Element CreateDefaultButton(this PrefsParam prefs)
        {
            var button = UI.Button(
                "default",
                prefs.ResetToDefault
            );

            button.onUpdate += _ =>
            {
                var color = prefs.IsDefault ? new Color(0.76f, 0.76f, 0.76f, 1f) : Color.red;
                button.SetColor(color);
            };

            return button;
        }
    }
}