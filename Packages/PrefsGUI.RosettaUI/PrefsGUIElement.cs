using System;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIElement
    {
        public static ButtonElement CreateDefaultButtonElement(Action onClick, Func<bool> isDefault)
        {
            var button = UI.Button(
                "default",
                onClick
            );

            button.onUpdate += _ =>
            {
                var color = isDefault() ? new Color(0.76f, 0.76f, 0.76f, 1f) : Color.red;
                button.SetColor(color);
            };

            return button;
        }
    }
}