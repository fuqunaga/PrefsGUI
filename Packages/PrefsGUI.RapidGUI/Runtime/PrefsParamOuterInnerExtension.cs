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
            var customLabel = prefs.GetCustomLabel();
            if (customLabel != null) RGUI.BeginCustomLabel(customLabel);
            if (prefs.synced) RGUI.BeginColor(PrefsParam.syncedColor);
            
            var changed = false;
            using (new GUILayout.HorizontalScope())
            {
                changed = prefs.DoGUICheckChanged(func);
                if (enableDefaultButton)
                {
                    changed |= prefs.DoGUIDefaultButton();
                }
            }

            if (prefs.synced) RGUI.EndColor();
            if (customLabel != null) RGUI.EndCustomLabel();

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