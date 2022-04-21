using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class MaterialPropertyDebugMenuExtension
    {
        public static void DoGUI(this MaterialPropertyDebugMenu menu, bool labelEnable = true)
        {
            if (menu.IsEnable)
            {
                if (labelEnable) GUILayout.Label(menu._material.name);
                using (new RGUI.IndentScope())
                {
                    menu.Colors.ForEach(c => c.DoGUI(KeyToPropertyName(c.key)));
                    menu.Vectors.ForEach(v =>
                    {
                        var n = KeyToPropertyName(v.key);
                        if (MaterialPropertyDebugMenu.customVectorGUI.ContainsKey(n))
                        {
                            MaterialPropertyDebugMenu.customVectorGUI[n](v, n);
                        }
                        else
                        {
                            v.DoGUISlider(n);
                        }
                    });
                    menu.Floats.ForEach(f => f.DoGUISlider(KeyToPropertyName(f.key)));
                    menu.Ranges.ForEach(range =>
                    {
                        var n = KeyToPropertyName(range.key);
                        var mr = menu._propertySet.ranges.Find(r => r.name == n);

                        range.DoGUISlider(mr.min, mr.max, n);
                    });

                    menu.TexEnvs.ForEach(t =>
                    {
                        var label = KeyToPropertyName(t.key);
                        t.DoGUISlider(Vector4.zero, new Vector4(10, 10, 1, 1), label);
                    });
                }

                menu.UpdateMaterial();

                string KeyToPropertyName(string key) => menu.KeyToPropertyName(key); 
            }
        }
    }
}