using System.Collections.Generic;
using System.Linq;
using RosettaUI;
using UnityEngine;
using UnityEngine.Rendering;

namespace PrefsGUI.RosettaUI
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PrefsMaterialPropertyCustom
    {
        // ReSharper disable once UnassignedField.Global
        public bool useDescriptionToLabel;
    }
    
    public static class PrefsMaterialPropertyExtension
    {
        public static PrefsMaterialPropertyCustom Custom { get; } = new();
        
        [RuntimeInitializeOnLoadMethod]
        private static void RegisterUI()
        {
            UICustom.RegisterElementCreationFunc<PrefsMaterialProperty>(
                (label, binder) => binder.Get().CreateElement(label)
            );
        }

        public static Element CreateElement(this PrefsMaterialProperty menu, LabelElement label = null)
        {
            return menu.IsEnable
                ? UI.Fold(label ?? menu._material.name, menu.CreateElementRaw())
                : null;
        }
        
        public static Element CreateElementRaw(this PrefsMaterialProperty menu)
        {
            return
                UI.Column(
                    new[]
                        {
                            CreateElements(menu.Colors),
                            CreateElements(menu.Vectors),
                            menu.Floats.Select(CreateFloatElement),
                            menu.Ranges.Select(prefs =>
                            {
                                var key = menu.KeyToPropertyName(prefs.key);
                                var label = KeyToLabelString(prefs.key);
                                var range = menu._propertySet.ranges.First(r => r.name == key);
                                return prefs.CreateSlider(label, range.min, range.max);
                            }),
                            menu.TexEnvs.Select(prefs =>
                            {
                                using var _ = new UICustom.PropertyOrFieldLabelModifierScope(
                                    ("x", "Tiling.x"),
                                    ("y", "Tiling.y"),
                                    ("z", "Offset.x"),
                                    ("w", "Offset.y")
                                );

                                var label = KeyToLabelString(prefs.key);
                                return prefs.CreateSlider(label, new Vector4(10, 10, 1, 1));
                            }),
                            CreateElements(menu.Ints)
                        }
                        .SelectMany(element => element)
                );

            IEnumerable<Element> CreateElements<T>(IEnumerable<PrefsParamOuter<T>> prefsSet) 
                 => prefsSet.Select(prefs => prefs.CreateElement(KeyToLabelString(prefs.key)));

            Element CreateFloatElement(PrefsParamOuter<float> prefs)
            {
                var propertyName = menu.KeyToPropertyName(prefs.key);
                var shader = menu._material.shader;
                var propertyIndex = shader.FindPropertyIndex(propertyName);
                var attributes = shader.GetPropertyAttributes(propertyIndex);
                var enumBlendMode = attributes.FirstOrDefault(attr => attr.StartsWith("Enum(UnityEngine.Rendering.BlendMode)"));
                if (enumBlendMode != null)
                {
                    return CreateEnumBlendModeElement(prefs);
                }

                return prefs.CreateElement(KeyToLabelString(prefs.key));
            }
     
             Element CreateEnumBlendModeElement(PrefsParamOuter<float> prefs)
             {
                 return UI.Row(
                     UI.Field(KeyToLabelString(prefs.key),
                         () => (BlendMode)prefs.Get(),
                         blendMode => prefs.Set((float)(int)blendMode)
                     ),
                     prefs.CreateDefaultButtonElement()
                 );
             }
             
            string KeyToLabelString(string key)
            {
                var name = menu.KeyToPropertyName(key);

                return Custom.useDescriptionToLabel
                    ? menu._propertySet.GetDescription(name)
                    : name;
            }
        }


    }
}