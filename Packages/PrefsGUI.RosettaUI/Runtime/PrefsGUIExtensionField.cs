using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionField
    {
        public static Element CreateElement<T>(this PrefsParamOuter<T> prefs, LabelElement label = null)
        {
            var element = UI.Row(
                UI.Field(
                    label ?? prefs.key,
                    prefs.Get,
                    v => prefs.Set(v)
                ),
                prefs.CreateDefaultButtonElement()
            );

            PrefsGUIExtension.SubscribeSyncedFlag(prefs, element);

            return element;
        }

        public static Element CreateElement<TPrefs0, TPrefs1, TOuter0, TOuter1>(
            this PrefsSet<TPrefs0, TPrefs1, TOuter0, TOuter1> prefs)
            where TPrefs0 : PrefsParamOuter<TOuter0>
            where TPrefs1 : PrefsParamOuter<TOuter1>
        {
            var fold = UI.Fold(
                prefs.key,
                prefs.prefs0.CreateElement(),
                prefs.prefs1.CreateElement()
            );
            
            SubscribePrefsSetSyncedFlag(prefs.prefs0, prefs.prefs1, fold);

            return fold;


            void SubscribePrefsSetSyncedFlag(PrefsParam prefs0, PrefsParam prefs1, Element element)
            {
                prefs0.onSyncedChanged += _ => OnSyncedChanged();
                prefs1.onSyncedChanged += _ => OnSyncedChanged();
                OnSyncedChanged();

                void OnSyncedChanged()
                {
                    var synced = prefs0.Synced && prefs1.Synced;
                    element?.SetColor(synced ? PrefsParam.syncedColor : null);
                }
            }

        }
    }
}