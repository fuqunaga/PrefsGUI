using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsSetExtension
    {
        public static bool DoGUI<TPrefs0, TPrefs1, TOuter0, TOuter1>(this PrefsSet<TPrefs0, TPrefs1, TOuter0, TOuter1> prefs, string label = null)
            where TPrefs0 : PrefsParamOuter<TOuter0>
            where TPrefs1 : PrefsParamOuter<TOuter1>
        {
            bool ret = false;
            
            var synced = prefs.prefs0.Synced && prefs.prefs1.Synced;
            if (synced) RGUI.BeginColor(PrefsParam.syncedColor);

            using (new GUILayout.HorizontalScope())
            {
                RGUI.PrefixLabel(label ?? prefs.key);

                using (new GUILayout.VerticalScope())
                {
                    ret = prefs.prefs0.DoGUI(prefs.paramNames[0]);
                    ret |= prefs.prefs1.DoGUI(prefs.paramNames[1]);
                }
            }

            if (synced) RGUI.EndColor();
            
            return ret;
        }
    }
}