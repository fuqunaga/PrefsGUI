using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsParamExtension
    {
        public static bool DoGUI(this PrefsParam prefs, string label = null)
        {
            return DispatchDoGUI(prefs, label);
        }

        private static readonly Dictionary<Type, Func<PrefsParam, string, bool>> DoGUIFuncTable = new();

        private static readonly Dictionary<Type, MethodInfo> GenericTypeDefinitionToMethodInfo = new ()
        {
            [typeof(PrefsParamOuterInner<,>)] = GetDoGUIMethodInfo(typeof(PrefsParamOuterInnerExtension)),
            [typeof(PrefsSet<,,,>)] = GetDoGUIMethodInfo(typeof(PrefsSetExtension)),
        };

        static MethodInfo GetDoGUIMethodInfo(Type type)
        {
            return type.GetMethod("DoGUI", BindingFlags.Public | BindingFlags.Static);
        }

        private static readonly object[] SharedArray = new object[2];
        
        private static bool DispatchDoGUI(PrefsParam prefs, string label)
        {
            var originalType = prefs.GetType();

            if (!DoGUIFuncTable.TryGetValue(originalType, out var func))
            {
                for (var type = originalType; type != null; type = type.BaseType)
                {
                    if (!type.IsGenericType) continue;
                    
                    var genericTypeDefinition = type.GetGenericTypeDefinition();
                    if (!GenericTypeDefinitionToMethodInfo.TryGetValue(genericTypeDefinition, out var mi)) continue;

                    var miGeneric = mi.MakeGenericMethod(type.GetGenericArguments());
                    
                    DoGUIFuncTable[originalType] = func = (p, l) =>
                    {
                        SharedArray[0] = p;
                        SharedArray[1] = l;

                        return (bool) miGeneric.Invoke(null, SharedArray);
                    };

                    break;
                }
            }
            
            Assert.IsNotNull(func, $"Prefs.key[{prefs.key}] type[{prefs.GetType()}]");

            return func(prefs, label);
        }
    }
}