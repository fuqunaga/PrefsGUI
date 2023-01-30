using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RosettaUI;
#if UNITY_EDITOR
using UnityEditor;
#else
using UnityEngine;
#endif

namespace PrefsGUI.RosettaUI
{
    public static class UICustomPrefsGUI
    {
        private static readonly Dictionary<Type, Func<IBinder, Element>> CreationFuncTable = new();
        
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        public static void RegisterUICustom()
        {
            UICustom.RegisterElementCreationFunc(typeof(PrefsParam), (label, binder) =>
            {
                var func = GetCreationFunc(binder.GetObject().GetType());
                return func(binder);
            });

            static Func<IBinder, Element> GetCreationFunc(Type type)
            {
                if (CreationFuncTable.TryGetValue(type, out var func)) return func;
                
                var prefsParamOuterType = GetPrefsParamOuterType(type);
                var outerType = prefsParamOuterType.GetGenericArguments().First();

                var methodInfo = typeof(PrefsGUIExtensionField)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(mi => mi.Name == nameof(PrefsGUIExtensionField.CreateElement) && mi.GetParameters().Length == 2)
                    .Select(mi => mi.MakeGenericMethod(outerType))
                    .FirstOrDefault();

                var parameters = new object[] {null, null};

                func = (binder) =>
                {
                    parameters[0] = binder.GetObject();
                    return methodInfo?.Invoke(null, parameters) as Element;
                };

                CreationFuncTable[type] = func;

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