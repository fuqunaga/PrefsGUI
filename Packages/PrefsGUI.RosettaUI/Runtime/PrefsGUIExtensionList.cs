using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionList
    {
        public static Element CreateElement<T>(this PrefsList<T> prefs, in ListOption option)
            => prefs.CreateElement(null, option);
        
        public static Element CreateElement<T>(this PrefsList<T> prefs, LabelElement label = null, in ListOption? option = null)
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
                                prefs.CreateDefaultButtonElementAt(idx)
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

        public static Element CreateDefaultButtonElementAt<T>(this PrefsList<T> prefs, int index)
        {
            return PrefsGUIElement.CreateDefaultButtonElement(
                onClick: () => prefs.ResetToDefaultAt(index),
                isDefault: () => prefs.IsDefaultAt(index)
            );
        }
    }
}