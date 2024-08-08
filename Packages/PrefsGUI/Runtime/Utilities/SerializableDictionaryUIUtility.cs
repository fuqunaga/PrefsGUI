using UnityEngine;
using UnityEngine.UIElements;

namespace PrefsGUI.Utility
{
    public static class SerializableDictionaryUIUtility
    {
        public const string duplicatedKeyMarkName = "prefsgui-duplicated-key-mark";
        
        public static VisualElement CreateDuplicatedKeyMark()
        {
            var color = new Color(0.9f, 0.4f, 0.1f);
            var backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            const float borderRadius = 7f;
            const float marginLR = 4f;
            const float paddingLR = 6f;

            var mark = new Label()
            {
                name = duplicatedKeyMarkName,
                text = "duplicate key",
                tooltip = "This item will not be added to the dictionary.",
                style =
                {
                    color = color,
                    backgroundColor = backgroundColor,
                    borderBottomLeftRadius = borderRadius,
                    borderBottomRightRadius = borderRadius,
                    borderTopLeftRadius = borderRadius,
                    borderTopRightRadius = borderRadius,
                    marginLeft = marginLR,
                    marginRight = marginLR,
                    paddingLeft = paddingLR,
                    paddingRight = paddingLR,
                }
            };

            return mark;
        }
    }
}