using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.RosettaUI
{
    public static class UICustomPrefsGUI
    {
        private static readonly Dictionary<Type, Func<PrefsParam, Element>> CreationFuncTable = new();

        [RuntimeInitializeOnLoadMethod]
        public static void RegisterUICustom()
        {
            UICustom.RegisterElementCreationFunc<PrefsParam>(prefsParam =>
            {
                var type = prefsParam.GetType();
                var func = GetCreationFunc(type);
                return func(prefsParam);
            });

            static Func<PrefsParam, Element> GetCreationFunc(Type type)
            {
                if (!CreationFuncTable.TryGetValue(type, out var func))
                {
                    var prefsParamOuterType = GetPrefsParamOuterType(type);
                    var outerType = prefsParamOuterType.GetGenericArguments().First();

                    var methodInfo = typeof(PrefsGUIExtensionField)
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(mi => mi.Name == nameof(PrefsGUIExtensionField.CreateElement) && mi.GetParameters().Length == 2)
                        .Select(mi => mi.MakeGenericMethod(outerType))
                        .FirstOrDefault();

                    var parameters = new object[] {null, null};

                    func = (prefsParam) =>
                    {
                        parameters[0] = prefsParam;
                        return methodInfo?.Invoke(null, parameters) as Element;
                    };

                    CreationFuncTable[type] = func;
                }

                return func;
            }
            

            static Type GetPrefsParamOuterType(Type type)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PrefsParamOuter<>))
                {
                    return type;
                }

                if (type.BaseType is { } baseType)
                {
                    return GetPrefsParamOuterType(baseType);
                }
                
                throw new ArgumentException($"{type} is not derived {nameof(PrefsParamOuter<object>)}", nameof(type));
            }
        }
    }
}