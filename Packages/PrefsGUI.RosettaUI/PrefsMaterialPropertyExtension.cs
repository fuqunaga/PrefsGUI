using System;
using System.Collections.Generic;
using System.Linq;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsMaterialPropertyExtension
    {
        public static Dictionary<string, Func<string, PrefsVector4, Element>> customVectorUI = new();
            

        [RuntimeInitializeOnLoadMethod]
        static void RegisterUI()
        {
            UICustom.RegisterElementCreationFunc<PrefsMaterialProperty>(
                (label, menu) => menu.CreateElement()
            );
        }

        public static Element CreateElement(this PrefsMaterialProperty menu)
        {
            return menu.IsEnable
                ? UI.Fold(menu._material.name, menu.CreateElementRaw())
                : null;
        }
        
        public static Element CreateElementRaw(this PrefsMaterialProperty menu)
        {
            return
                UI.Column(
                    new[]
                        {
                            menu.Colors.Select(prefs => prefs.CreateElement(menu.KeyToPropertyName(prefs.key))),
                            menu.Vectors.Select(prefs =>
                            {
                                var key = menu.KeyToPropertyName(prefs.key);
                                return customVectorUI.TryGetValue(key, out var func)
                                    ? func(key, prefs)
                                    : prefs.CreateSlider(key);
                            }),
                            menu.Floats.Select(prefs => prefs.CreateElement(menu.KeyToPropertyName(prefs.key))),
                            menu.Ranges.Select(prefs =>
                            {
                                var key = menu.KeyToPropertyName(prefs.key);
                                var range = menu._propertySet.ranges.First(r => r.name == key);
                                return prefs.CreateSlider(key, range.min, range.max);
                            }),
                            menu.TexEnvs.Select(prefs =>
                            {
                                using var _ = new UICustom.PropertyOrFieldLabelModifierScope(
                                    ("x", "Tiling.x"),
                                    ("y", "Tiling.y"),
                                    ("z", "Offset.x"),
                                    ("w", "Offset.y")
                                );
                                
                                var key = menu.KeyToPropertyName(prefs.key);
                                return prefs.CreateSlider(key, new Vector4(10, 10, 1, 1));
                            }),
                            menu.Ints.Select(prefs => prefs.CreateElement(menu.KeyToPropertyName(prefs.key)))
                        }
                        .SelectMany(element => element)
                ).RegisterValueChangeCallback(menu.UpdateMaterial);
        }
    }
}