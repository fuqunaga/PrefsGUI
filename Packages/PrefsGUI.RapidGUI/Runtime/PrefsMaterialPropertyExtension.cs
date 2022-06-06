using System.Collections.Generic;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsMaterialPropertyExtension
    {
        static readonly Dictionary<string, string> texEnvCustomLabel = new()
        {
            {"x", "Tiling.x"},
            {"y", "Tiling.y"},
            {"z", "Offset.x"},
            {"w", "Offset.y"}
        };
        
        public static void DoGUI(this PrefsMaterialProperty menu, bool labelEnable = true)
        {
            if (menu.IsEnable)
            {
                if (labelEnable) GUILayout.Label(menu._material.name);
                using (new RGUI.IndentScope())
                {
                    menu.Colors.ForEach(c => c.DoGUI(KeyToPropertyName(c.key)));
                    menu.Vectors.ForEach(v =>v.DoGUISlider(KeyToPropertyName(v.key)));
                    menu.Floats.ForEach(f => f.DoGUISlider(KeyToPropertyName(f.key)));
                    menu.Ranges.ForEach(range =>
                    {
                        var n = KeyToPropertyName(range.key);
                        var mr = menu._propertySet.ranges.Find(r => r.name == n);

                        range.DoGUISlider(mr.min, mr.max, n);
                    });

                    using (new RGUI.CustomLabelScope(texEnvCustomLabel))
                    {
                        menu.TexEnvs.ForEach(t =>
                        {
                            var label = KeyToPropertyName(t.key);
                            t.DoGUISlider(Vector4.zero, new Vector4(10, 10, 1, 1), label);
                        });
                    }
                }

                string KeyToPropertyName(string key) => menu.KeyToPropertyName(key); 
            }
        }
    }
}