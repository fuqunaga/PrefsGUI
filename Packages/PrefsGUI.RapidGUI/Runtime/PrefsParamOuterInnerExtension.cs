using System;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsParamOuterInnerExtension
    {
        public static bool DoGUI<OuterT, InnerT>(this PrefsParamOuterInner<OuterT, InnerT> prefs, string label = null)
        {
            return prefs.DoGUIStandard((v) => RGUI.Field(v, label ?? prefs.key));
        }

        public static bool DoGUIStandard<TOuter, TInner>(this PrefsParamOuterInner<TOuter, TInner> prefs, Func<TOuter, TOuter> func, bool enableDefaultButton = true)
        {
            if (prefs.Synced) RGUI.BeginColor(PrefsParam.syncedColor);
            
            var changed = false;
            using (new GUILayout.HorizontalScope())
            {
                changed = prefs.DoGUICheckChanged(func);
                if (enableDefaultButton)
                {
                    changed |= prefs.DoGUIDefaultButton();
                }
            }

            if (prefs.Synced) RGUI.EndColor();

            return changed;
        }
        
        
        public static bool DoGUIDefaultButton<TOuter, TInner>(this PrefsParamOuterInner<TOuter, TInner> prefs)
        {
            var ret = GUIComponent.DoGUIDefaultButton(prefs.IsDefault);
            if (ret)
            {
                prefs.ResetToDefault();
            }

            return ret;
        }
    }
}