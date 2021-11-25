using RosettaUI;
using UnityEngine;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionField
    {
        #region CreateElement
        
        public static Element CreateElement<T>(this PrefsParamOuter<T> prefs)
        {
            return CreateElement(prefs, null);
        }

        public static Element CreateElement<T>(this PrefsParamOuter<T> prefs, LabelElement label)
        {
            return UI.Row(
                UI.Field(
                    label ?? prefs.key,
                    prefs.Get,
                    prefs.Set
                ),
                prefs.CreateDefaultButton()
            );
        }

        public static Element CreateElement<TPrefs0, TPrefs1, TOuter0, TOuter1>(
            this PrefsSet<TPrefs0, TPrefs1, TOuter0, TOuter1> prefs)
            where TPrefs0 : PrefsParamOuter<TOuter0>
            where TPrefs1 : PrefsParamOuter<TOuter1>
        {
            return UI.Fold(
                prefs.key,
                prefs.prefs0.CreateElement(),
                prefs.prefs1.CreateElement()
            );
        }

        #endregion
        
        
        public static Element CreateDefaultButton(this PrefsParam prefs)
        {
            var button = UI.Button(
                "default",
                prefs.ResetToDefault
            );

            button.onUpdate += _ =>
            {
                var color = prefs.IsDefault ? new Color(0.76f, 0.76f, 0.76f, 1f) : Color.red;
                button.SetColor(color);
            };

            return button;
        }
    }
}