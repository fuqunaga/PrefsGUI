using System;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsParamExtension
    {
        public static bool DoGUI(this PrefsParam prefs, string label = null)
        {
            return DispatchDoGUI(prefs, label);
        }

        private static readonly Dictionary<Type, Func<PrefsParam, string, bool>> doGUIFuncTable = new();

        private static bool DispatchDoGUI(PrefsParam prefs, string label)
        {
            var type = prefs.GetType();

            if (!doGUIFuncTable.TryGetValue(type, out var func))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PrefsParamOuterInner<,>))
                {
                    var mi = typeof(PrefsParamOuterInnerExtension).GetMethod(
                        nameof(PrefsParamOuterInnerExtension.DoGUI),
                        BindingFlags.Public | BindingFlags.Static
                    );

                    doGUIFuncTable[type] = func = (p, l) =>
                    {
                        var array = ArrayPool<object>.Shared.Rent(2);
                        array[0] = p;
                        array[1] = l;

                        var ret = (bool) mi.Invoke(null, array);

                        ArrayPool<object>.Shared.Return(array);

                        return ret;
                    };
                }
            }

            return func(prefs, label);
        }
    }
}