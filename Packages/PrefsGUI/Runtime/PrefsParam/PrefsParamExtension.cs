using System;

namespace PrefsGUI
{
    public static class PrefsParamExtension
    {
        public static void RegisterValueChangedCallbackAndCallOnce<T>(this PrefsParam<T> param, Action callback)
            where T : struct
        {
            param.RegisterValueChangedCallback(callback);
            callback?.Invoke();
        }
    }
}