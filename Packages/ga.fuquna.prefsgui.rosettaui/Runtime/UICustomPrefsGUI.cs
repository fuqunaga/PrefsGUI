using System;
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

            UICustom.RegisterElementCreationFunc(typeof(AddressAndPort), (label, binder) =>
            {
                var addressName = nameof(AddressAndPort.address);
                var portName = nameof(AddressAndPort.port);
                var addressBinder = PropertyOrFieldBinder.Create(binder, addressName);
                var portBinder = PropertyOrFieldBinder.Create(binder, portName);

                return UI.ClipboardContextMenu(new CompositeFieldElement(label, new[]
                {
                    UI.Field(addressName, addressBinder).SetWidth(180f),
                    UI.Field(portName, portBinder).SetWidth(120f)
                }), binder);
            });
        }
    }
}