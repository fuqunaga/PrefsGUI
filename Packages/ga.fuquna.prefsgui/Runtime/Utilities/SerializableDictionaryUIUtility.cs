using UnityEngine;
using UnityEngine.UIElements;

namespace PrefsGUI.Utility
{
    public static class SerializableDictionaryUIUtility
    {
        public const string UssClassNameMark = "prefsgui-serialiable-dictionary-mark";
        public const string UssClassNameMarkPrevious = "prefsgui-serialiable-dictionary-mark--previous";
        public const string UssClassNameMarkDuplicate = "prefsgui-serialiable-dictionary-mark--duplicate";
        
        public static readonly Color PreviousKeyMarkColor = new(0.2f, 0.5f, 0.9f);
        public static readonly Color DuplicateKeyMarkColor = new(0.9f, 0.4f, 0.1f);
        
        public static VisualElement CreatePreviousKeyMark()
        {
            var mark = CreateEmptyMark();
            mark.AddToClassList(UssClassNameMarkPrevious);

            mark.text = "previous key";
            mark.style.color = PreviousKeyMarkColor;

            return mark;
        }
        
        public static VisualElement CreateDuplicatedKeyMark()
        {
            var mark = CreateEmptyMark();
            mark.AddToClassList(UssClassNameMarkDuplicate);

            mark.text = "duplicate key";
            mark.style.color = DuplicateKeyMarkColor;

            return mark;
        }

        public static Label CreateEmptyMark()
        {
            var backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            const float borderRadius = 7f;
            const float marginLR = 4f;
            const float paddingLR = 6f;

            var mark = new Label()
            {
                style =
                {
                    backgroundColor = backgroundColor,
                    borderBottomLeftRadius = borderRadius,
                    borderBottomRightRadius = borderRadius,
                    borderTopLeftRadius = borderRadius,
                    borderTopRightRadius = borderRadius,
                    marginLeft = marginLR,
                    marginRight = marginLR,
                    paddingLeft = paddingLR,
                    paddingRight = paddingLR,
                    paddingBottom = 1f
                }
            };
            
            mark.AddToClassList(UssClassNameMark);

            return mark;
        }
    }
}