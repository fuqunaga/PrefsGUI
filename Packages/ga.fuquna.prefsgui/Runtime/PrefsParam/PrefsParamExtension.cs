using System;

namespace PrefsGUI
{
    public static class PrefsParamExtension
    {
        public static void RegisterValueChangedCallbackAndCallOnce(this PrefsParam param, Action callback)
        {
            param.RegisterValueChangedCallback(callback);
            callback?.Invoke();
        }
    }
}