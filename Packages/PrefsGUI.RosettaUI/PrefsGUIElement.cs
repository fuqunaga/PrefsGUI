using System;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIElement
    {
        public static ButtonElement CreateDefaultButtonElement(Action onClick, Func<bool> isDefault)
        {
            var button = UI.Button("default");
            
            button.onClick += () =>
            {
                onClick();
                button.NotifyViewValueChanged();
            };

            button.onUpdate += _ =>
            {
                var color = isDefault() ? (Color?)null : Color.red;
                button.SetColor(color);
            };

            return button;
        }
    }
}