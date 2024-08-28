using System.Collections;
using System.Collections.Generic;
using PrefsGUI.Utility;
using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionDictionary
    {
        #region Type Define
        
        public enum DuplicateKeyType
        {
            Unique,
            Previous,
            Duplicate
        }

        public class PrefsDictionaryData<TKey, TValue>
        {
            private readonly PrefsDictionary<TKey, TValue> _prefs;
            private readonly Dictionary<int, DuplicateKeyType> duplicateKeyDictionary = new();

            private bool NeedUpdate { get; set; }
            
            
            public PrefsDictionaryData(PrefsDictionary<TKey, TValue> prefs)
            {
                _prefs = prefs;
                _prefs.RegisterValueChangedCallbackAndCallOnce(() => NeedUpdate = true);
            }
            
            public DuplicateKeyType GetDuplicateKeyType(int idx)
            {
                if (NeedUpdate) UpdateDuplicateKeyDictionary();
                
                return duplicateKeyDictionary.TryGetValue(idx, out var type) 
                    ? type 
                    : DuplicateKeyType.Unique;
            }
            
            private void UpdateDuplicateKeyDictionary()
            {
                duplicateKeyDictionary.Clear();
                var sameKeyIndexGroup =  _prefs.Get().GetSameKeyIndexGroups();
                foreach (var group in sameKeyIndexGroup)
                {
                    var first = true;
                    foreach(var i in group)
                    {
                        var keyType = DuplicateKeyType.Duplicate;
                        if (first)
                        {
                            first = false;
                            keyType = DuplicateKeyType.Previous;
                        }

                        duplicateKeyDictionary[i] = keyType;
                    }
                }

                NeedUpdate = false;
            }
        }
        
        #endregion
        
        public static Element CreateElement<TKey, TValue>(this PrefsDictionary<TKey, TValue> prefs, in ListViewOption option) 
            => prefs.CreateElement(null, option);
        
        public static Element CreateElement<TKey, TValue>(this PrefsDictionary<TKey, TValue> prefs, LabelElement label = null, in ListViewOption? option = null)
        {
            var prefsDictionaryData = new PrefsDictionaryData<TKey, TValue>(prefs);

            return prefs.CreateElement(label, option, (binder, index) => CreateDictionaryItemDefault(
                    prefsDictionaryData,
                    (IBinder<SerializableDictionary<TKey, TValue>.KeyValue>)binder,
                    index
                )
            );
        }

        /// <summary>
        /// Dictionaryの要素のUIを作成する
        /// 
        /// UI.Field(label, ...)では、FloatFieldのラベルドラッグなど
        /// Labelのイベントが反映されてFoldのオープンクローズが反応しない場合があるので
        /// UI.Field()のラベルは使わず、UI.Label()を別途使用している
        /// </summary>
        public static Element CreateDictionaryItemDefault<TKey, TValue>(
            PrefsDictionaryData<TKey, TValue> prefsDictionaryData,
            IBinder<SerializableDictionary<TKey, TValue>.KeyValue> binder, 
            int idx)
        {
            
            var isKeySingleLine = TypeUtility.IsSingleLine(typeof(TKey));

            // キーが複数行ならFoldのヘッダーには表示しない
            if (!isKeySingleLine)
            {
                return UI.Fold(
                    UI.ClipboardContextMenu(
                        UI.Label(() => $"Item {idx} {GetCurrentKeyTypeString()}"),
                        binder
                    ),
                    new[]
                    {
                        UI.Field(null, binder)
                    }
                );
            }
            
            // キーが１行で表示できるならFoldのヘッダーに表示する
            
            // Valueが１行で表示できない型の場合ラベルにnullを指定してFoldで囲わない
            // ただしIListは除く
            var needLabel = TypeUtility.IsSingleLine(typeof(TValue)) || typeof(IList).IsAssignableFrom(typeof(TValue));
            var valueFieldLabel = needLabel ? (LabelElement)"Value" : null;

            var keyBinder = new PropertyOrFieldBinder<SerializableDictionary<TKey, TValue>.KeyValue, TKey>(binder,
                nameof(SerializableDictionary<TKey, TValue>.KeyValue.key));
            
            var fold = UI.Fold(
                // UI.Row()はflex-grow:1を効かせるためにも必要
                // 現状UI.Row()の子供にはflex-growを設定しているがデフォルトのスタイルとしては設定されていない
                UI.ClipboardContextMenu(
                    UI.Row(
                        UI.Label(() => $"Key {idx}  {GetCurrentKeyTypeString()}", LabelType.Prefix),
                        // mark,
                        UI.Field(null, keyBinder)
                    ),
                    keyBinder
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

            string GetCurrentKeyTypeString()
            {
                var keyType = prefsDictionaryData.GetDuplicateKeyType(idx);
                return keyType switch
                {
                    DuplicateKeyType.Unique => null,
                    // ReSharper disable once StringLiteralTypo
                    DuplicateKeyType.Previous => "<b><size=-3><color=#4780FD>previous key</color></size></b>",
                    DuplicateKeyType.Duplicate => "<b><size=-3><color=#E04B50>duplicate key</color></size></b>",
                    _ => null
                };
            }
        }
    }
}