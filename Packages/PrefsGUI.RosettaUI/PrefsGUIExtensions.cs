using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensions
    {
        public static Element CreateElement<T>(this PrefsParamOuter<T> prefs, Action<T> onValueChanged = null)
        {
            return UI.Row(
                UI.Field(
                    prefs.key,
                    targetExpression: () => prefs.Get(),
                    onValueChanged: (v) =>
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
            IGetter<T> minGetter = null;
            IGetter<T> maxGetter = null;
            if (prefs is IPrefsSlider<T> prefsSlider)
            {
                minGetter = ConstGetter.Create(prefsSlider.defaultMin);
                maxGetter = ConstGetter.Create(prefsSlider.defaultMax);
            }

            return CreateSlider(prefs, minGetter, maxGetter, onValueChanged);
        }

        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, T min, T max,
            Action<T> onValueChanged = null)
        {
            return CreateSlider(prefs, ConstGetter.Create(min), ConstGetter.Create(max), onValueChanged);
        }


        private static readonly Dictionary<Type, MethodInfo> CreateMinMaxSliderMethodTable =
            new Dictionary<Type, MethodInfo>();

        public static Element CreateSlider<T>(this PrefsParamOuter<T> prefs, IGetter<T> minGetter, IGetter<T> maxGetter,
            Action<T> onValueChanged = null)
        {
            var type = typeof(T);
            var valueType = GetMinMaxValueType(type);

            Element slider = null;
            if (valueType != null)
            {
                if (!CreateMinMaxSliderMethodTable.TryGetValue(type, out var mi))
                {
                    mi = typeof(PrefsGUIExtensions)
                        .GetMethod(nameof(_CreateMinMaxSlider), BindingFlags.Static | BindingFlags.NonPublic)
                        ?.MakeGenericMethod(type, valueType);

                    CreateMinMaxSliderMethodTable[type] = mi;
                }

                slider = (Element) mi?.Invoke(null, new object[] {prefs, minGetter, maxGetter, onValueChanged});
            }
            else
            {
                slider = _CreateSlider(prefs, minGetter, maxGetter, onValueChanged);
            }

            return UI.Row(
                slider,
                prefs.CreateDefaultButton()
            );
                        static Type GetMinMaxValueType(Type type)
                        {
                            while (type != null)
                            {
                                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(RapidGUI.MinMax<>)))
                                {
                                    return type.GetGenericArguments().First();
                                }
            
                                type = type.BaseType;
                            }
            
                            return null;
                        }

        }

        static Element _CreateSlider<T>(PrefsParamOuter<T> prefs, IGetter<T> minGetter, IGetter<T> maxGetter,
            Action<T> onValueChanged)
        {
            return UI.Slider(
                prefs.key,
                targetExpression: () => prefs.Get(),
                minGetter: minGetter,
                maxGetter: maxGetter,
                onValueChanged: (v) =>
                {
                    prefs.Set(v);
                    onValueChanged?.Invoke(v);
                });
        }


        // TODO: dirty way. avoid boxing
        private static readonly Dictionary<Type, (FieldInfo, FieldInfo)?> _minMaxFiedInfoTable =
            new Dictionary<Type, (FieldInfo, FieldInfo)?>();

        static Element _CreateMinMaxSlider<T, TValue>(PrefsParamOuter<T> prefs, IGetter<TValue> minGetter, IGetter<TValue> maxGetter, Action<T> onValueChanged)
        {
            var type = typeof(T);
            if (!_minMaxFiedInfoTable.TryGetValue(type, out var pair))
            {
                var min = typeof(T).GetField("min");
                var max = typeof(T).GetField("max");

                _minMaxFiedInfoTable[type] = pair = (min, max);
            }

            var (minField, maxField) = pair.Value;

            Func<MinMax<TValue>> getFunc = () =>
            {
                var minMax = prefs.Get() as RapidGUI.MinMax<TValue>;
                return MinMax.Create(minMax.min, minMax.max);
            };

            return UI.MinMaxSlider(
                prefs.key,
                targetExpression: () => getFunc(),
                minGetter: minGetter,
                maxGetter: maxGetter,
                onValueChanged: (v) =>
                {
                    var minMax = (T) Activator.CreateInstance(typeof(T));
                    minField.SetValue(minMax, v.min);
                    maxField.SetValue(minMax, v.max);

                    prefs.Set(minMax);
                    onValueChanged?.Invoke(minMax);
                });
        }


        static Element CreateDefaultButton(this PrefsParam prefs)
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