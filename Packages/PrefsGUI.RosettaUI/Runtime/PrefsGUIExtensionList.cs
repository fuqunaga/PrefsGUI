using System;
using System.Collections;
using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionList
    {
        public static Element CreateElement<TList, TListForUI>(
            this PrefsListBase<TList, TListForUI> prefs,
            in ListViewOption option
        )
            where TList : new()
            where TListForUI : IList
            => prefs.CreateElement(null, option);

        public static Element CreateElement<TList, TListForUI>(
            this PrefsListBase<TList, TListForUI> prefs,
            LabelElement label = null,
            in ListViewOption? option = null,
            Func<IBinder, int, Element> createItemElementFunc = null
        )
            where TList : new()
            where TListForUI : IList
        {
            var listAccessor = prefs.GetListAccessor();
            var element = UI.Row(
                UI.List(
                    label ?? UI.Label(() => prefs.key),
                    () => listAccessor.InnerList,
                    (binder, idx) =>
                    {
                        createItemElementFunc ??= UI.ListItemDefault;
                        var field = createItemElementFunc(binder, idx);

                        return (idx < prefs.DefaultValueCount)
                            ? UI.Row(
                                field,
                                prefs.CreateDefaultButtonElementAt(idx)
                            )
                            : field;
                    },
                    option
                ),
                prefs.CreateDefaultButtonElement()
            );
            
            PrefsGUIExtension.SubscribeSyncedFlag(prefs, element);

            return element;
        }

        public static Element CreateDefaultButtonElementAt<TList, TListForUI>(this PrefsListBase<TList, TListForUI> prefs, int index) 
            where TList : new()
            where TListForUI : IList
        {
            return PrefsGUIElement.CreateDefaultButtonElement(
                onClick: () => prefs.ResetToDefaultAt(index),
                isDefault: () => prefs.IsDefaultAt(index)
            );
        }
    }
}