using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsSetExtension
    {
        public static bool DoGUI<Prefs0, Prefs1, Outer0, Outer1>(this PrefsSet<Prefs0, Prefs1, Outer0, Outer1> prefs, string label = null)
            where Prefs0 : PrefsParamOuter<Outer0>
            where Prefs1 : PrefsParamOuter<Outer1>
        {
            using (new GUILayout.HorizontalScope())
            {
                RGUI.PrefixLabel(label ?? prefs.key);
                
                using var _ = new GUILayout.VerticalScope();

                var ret = prefs.prefs0.DoGUI(prefs.paramNames[0]);
                ret |= prefs.prefs1.DoGUI(prefs.paramNames[1]);
                
                return ret;
            }
        }
    }
}