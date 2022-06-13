using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionList
    {
        public static Element CreateElement<T>(this PrefsList<T> prefs, LabelElement label = null)
        {
            var listBinder = Binder.Create(prefs.Get, v => prefs.Set(v));

            var element = UI.Fold(
                UI.Row(
                    label ?? UI.Label(() => prefs.key),
                    UI.Space(),
                    UI.ListCounterField(listBinder),
                    PrefsGUIElement.CreateDefaultButtonElement(
                        onClick: prefs.ResetToDefaultCount,
                        isDefault: () => prefs.IsDefaultCount
                    )
                ),
                new[]
                {
                    UI.ListItemContainer(
                        listBinder,
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
                        }
                    )
                }
            );
            
            PrefsGUIExtension.SubscribeSyncedFlag(prefs, element);

            return element;
        }
    }
}