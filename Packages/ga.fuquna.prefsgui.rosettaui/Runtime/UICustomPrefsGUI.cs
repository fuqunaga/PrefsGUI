using System;
using RosettaUI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#else
using UnityEngine;
#endif

namespace PrefsGUI.RosettaUI
{
    public static class UICustomPrefsGUI
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        public static void RegisterUICustom()
        {
            UICustom.RegisterElementCreationFunc(typeof(PrefsParam), (label, binder) =>
            {
                if (binder.GetObject() is not PrefsParam prefs)
                {
                    throw new ArgumentException($"{binder.GetObject()} is not {nameof(PrefsParam)}", nameof(binder));
                }

                return prefs.CreateElement(label);
            });
        }
    }
}