using System.Collections.Generic;
using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionList
    {
        public static Element CreateElement<T>(this PrefsList<T> prefs)
        {
            return CreateElement(prefs, null);
        }

        public static Element CreateElement<T>(this PrefsList<T> prefs, LabelElement label)
        {
            return UI.Row(
                UI.List(
                    label ?? prefs.key,
                    prefs.Get,
                    iList => prefs.Set((List<T>)iList),
                    (binder, idx) =>
                    {
                        var field = BinderToElement.CreateListViewItemDefaultElement(binder, idx);
                        var ret = field;
                        
                        if (idx < prefs.DefaultValueCount)
                        {
                            ret = UI.Row(
                                field,
                                PrefsGUIElement.CreateDefaultButtonElement(
                                    onClick: () => prefs.ResetToDefaultAt(idx),
                                    isDefault: () => prefs.IsDefaultAt(idx)
                                    )
                         
                            );
                        }

                        return ret;
                    }),
                prefs.CreateDefaultButtonElement()
            );
        }
    }
}