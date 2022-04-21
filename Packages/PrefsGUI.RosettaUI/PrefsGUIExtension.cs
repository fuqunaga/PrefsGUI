using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RosettaUI;
using UnityEngine;
using UnityEngine.Assertions;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtension
    {
        private static readonly Dictionary<Type, Func<PrefsParam, LabelElement, Element>> FuncTable = new();

        private static MethodInfo _listCreateElementMethodInfo;
        private static MethodInfo _fieldCreateElementMethodInfo;

        private static MethodInfo GetNonGenericMethodInfo(Type prefsType)
        {
            if (!prefsType.IsGenericType) return null;

            var definitionType = prefsType.GetGenericTypeDefinition();
            
            if (definitionType == typeof(PrefsList<>))
            {
                _listCreateElementMethodInfo ??= typeof(PrefsGUIExtensionList)
                    .GetMethod(nameof(PrefsGUIExtensionList.CreateElement), BindingFlags.Static | BindingFlags.Public);

                return _listCreateElementMethodInfo;
            }

            if (definitionType == typeof(PrefsParamOuter<>))
            {

                if (_fieldCreateElementMethodInfo == null)
                {
                    _fieldCreateElementMethodInfo = typeof(PrefsGUIExtensionField)
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(mi => mi.Name == nameof(PrefsGUIExtensionField.CreateElement))
                        .FirstOrDefault(mi =>
                            mi.GetParameters()[0].ParameterType.GetGenericTypeDefinition() ==
                            typeof(PrefsParamOuter<>));
                }

                return _fieldCreateElementMethodInfo;
            }

            return null;
        }
        
        public static Element CreateElement(this PrefsParam prefs, LabelElement label = null)
        {
            var originalType = prefs.GetType();
            if (!FuncTable.TryGetValue(originalType, out var func))
            {
                var type = originalType;
                var mi = GetNonGenericMethodInfo(type);
                while (mi == null)
                {
                    type = type.BaseType;
                    Assert.IsNotNull(type, originalType.ToString());
                    
                    mi = GetNonGenericMethodInfo(type);
                }
    
                var arg = type.GetGenericArguments()[0];
                mi = mi.MakeGenericMethod(arg);
                
                func = (p, l) => mi.Invoke(null, new object[] {p, l}) as Element;

                FuncTable[originalType] = func;
            }

            return func?.Invoke(prefs, label);
        }




        public static ButtonElement CreateDefaultButtonElement(this PrefsParam prefs)
        {
            return PrefsGUIElement.CreateDefaultButtonElement(prefs.ResetToDefault, () => prefs.IsDefault);
        }
    }
}