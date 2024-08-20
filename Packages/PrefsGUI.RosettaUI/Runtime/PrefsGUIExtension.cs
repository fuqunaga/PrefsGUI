using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RosettaUI;
using UnityEngine.Assertions;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtension
    {
        private static readonly Dictionary<Type, Func<PrefsParam, LabelElement, Element>> FuncTable = new();

        private static MethodInfo _dictionaryCreateElementMethodInfo;
        private static MethodInfo _listCreateElementMethodInfo;
        private static MethodInfo _fieldCreateElementMethodInfo;

        
        public static Element CreateElement(this PrefsParam prefs, LabelElement label = null)
        {
            var func = GetCreateElementFunc(prefs.GetType());
            return func?.Invoke(prefs, label);
        }

        public static ButtonElement CreateDefaultButtonElement(this PrefsParam prefs)
        {
            return PrefsGUIElement.CreateDefaultButtonElement(prefs.ResetToDefault, () => prefs.IsDefault);
        }
        
        public static void SubscribeSyncedFlag(PrefsParam prefs, Element element)
        {
            prefs.onSyncedChanged += OnSyncedChanged;
            OnSyncedChanged(prefs.Synced);

            void OnSyncedChanged(bool synced)
            {
                element?.SetColor(synced ? PrefsParam.syncedColor : null);
            }
        }
        
        
        private static Func<PrefsParam, LabelElement, Element> GetCreateElementFunc(Type originalType)
        {
            if (FuncTable.TryGetValue(originalType, out var func))
            {
                return func;
            }
            
            // MethodInfoが見つかるまで親クラスをさかのぼっていく
            var type = originalType;
            var mi = GetGenericMethodInfo(type);
            while (mi == null)
            {
                type = type.BaseType;
                Assert.IsNotNull(type, originalType.ToString());

                mi = GetGenericMethodInfo(type);
            }

            var arg = type.GetGenericArguments();
            mi = mi.MakeGenericMethod(arg);
            var parameterCount = mi.GetParameters().Length;
            var parameterArray = new object[parameterCount];

            func = (p, l) =>
            {
                parameterArray[0] = p;
                parameterArray[1] = l;
                return mi.Invoke(null, parameterArray) as Element;
            };

            FuncTable[originalType] = func;

            return func;
        }
        
        
        private static MethodInfo GetGenericMethodInfo(Type prefsType)
        {
            if (!prefsType.IsGenericType) return null;

            var definitionType = prefsType.GetGenericTypeDefinition();

            if (definitionType == typeof(PrefsDictionary<,>))
            {
                _dictionaryCreateElementMethodInfo ??= GetCreateElementMethodInfo(typeof(PrefsGUIExtensionDictionary), definitionType);
                return _dictionaryCreateElementMethodInfo;
            }
            
            if (definitionType == typeof(PrefsListBase<,>))
            {
                _listCreateElementMethodInfo ??= GetCreateElementMethodInfo(typeof(PrefsGUIExtensionListBase), definitionType);
                return _listCreateElementMethodInfo;
            }

            if (definitionType == typeof(PrefsParamOuter<>))
            {
                _fieldCreateElementMethodInfo ??= GetCreateElementMethodInfo(typeof(PrefsGUIExtensionField), definitionType);
                return _fieldCreateElementMethodInfo;
            }

            return null;
            
            
            // [ExtensionClass].CreateElement(prefs, label) のMethodInfoを取得する
            static MethodInfo GetCreateElementMethodInfo(Type ExtensionType, Type prefsType)
            {
                return ExtensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(mi => mi.Name == "CreateElement")
                    .First(mi =>
                    {
                        var parameterInfos = mi.GetParameters();
                        return
                            parameterInfos[0].ParameterType.GetGenericTypeDefinition() == prefsType
                            && parameterInfos[1].ParameterType == typeof(LabelElement)
                            ;
                    });
            }
        }
    }
}