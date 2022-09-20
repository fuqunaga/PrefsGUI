using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionList
    {
        public static Element CreateElement<T>(this PrefsList<T> prefs, ListViewOption option)
            => prefs.CreateElement(null, option);
        
        public static Element CreateElement<T>(this PrefsList<T> prefs, LabelElement label = null, ListViewOption option = null)
        {
            var element = UI.Row(
                UI.List(
                    label ?? UI.Label(() => prefs.key),
                    () => prefs,
                    (binder, idx) =>
                    {
                        var field = UI.ListItemDefault(binder, idx);
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
                    },
                    option
                ),
                prefs.CreateDefaultButtonElement()
            );
            
            PrefsGUIExtension.SubscribeSyncedFlag(prefs, element);

            return element;
        }
    }
}