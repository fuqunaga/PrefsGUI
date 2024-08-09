using PrefsGUI.Utility;
using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionDictionary
    {
        public static Element CreateElement<TKey, TValue>(this PrefsDictionary<TKey, TValue> prefs, in ListViewOption option) 
            => prefs.CreateElement(null, option);
        
        public static Element CreateElement<TKey, TValue>(this PrefsDictionary<TKey, TValue> prefs, LabelElement label = null, in ListViewOption? option = null)
        {
            var listAccessor = prefs.GetListAccessor();
            var element = UI.Row(
                UI.List(
                    label ?? UI.Label(() => prefs.key),
                    () => listAccessor.InnerList,
                    (binder, idx) =>
                    {
                        var field = CreateDictionaryItemDefault((IBinder<SerializableDictionary<TKey, TValue>.KeyValue>)binder, idx);
            
                        // var field = UI.ListItemDefault(binder, idx);

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

        public static Element CreateDictionaryItemDefault<TKey, TValue>(
            IBinder<SerializableDictionary<TKey, TValue>.KeyValue> binder, int idx)
        {
            // キーが１行で表示できるならFoldのヘッダーに表示する
            var isKeySingleLine = TypeUtility.IsSingleLine(typeof(TKey));

            if (!isKeySingleLine)
            {
                return UI.Fold($"Item {idx}", UI.Field(null, binder));
            }
            
            // Valueが１行で表示できない場合、ラベルは表示しない
            var isValueSingleLine = TypeUtility.IsSingleLine(typeof(TValue));
            var valueFieldLabel = isValueSingleLine ? (LabelElement)"Value" : null;
            
            var fold = UI.Fold(
                // UI.Row()いらないけどflex-graw:1を効かせるためのハックとして入れている
                UI.Row(
                    UI.Field($"Key {idx}", () => binder.Get().key, v =>
                    {
                        var kv = binder.Get();
                        kv.key = v;
                        binder.Set(kv);
                    })
                ),
                new[]
                {
                    UI.Field(valueFieldLabel, () => binder.Get().value, v =>
                    {
                        var kv = binder.Get();
                        kv.value = v;
                        binder.Set(kv);
                    })
                }
            );

            return fold;
        }
    }
}