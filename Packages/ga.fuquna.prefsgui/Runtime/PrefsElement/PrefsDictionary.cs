using System;
using System.Collections;
using System.Collections.Generic;
using PrefsGUI.Utility;
using UnityEngine.Assertions;

namespace PrefsGUI
{
    /// <summary>
    /// List style PrefsGUI
    /// </summary>
    [Serializable]
    public class PrefsDictionary<TKey, TValue> : PrefsListBase<SerializableDictionary<TKey, TValue>, List<SerializableDictionary<TKey, TValue>.KeyValue>>, IDictionary<TKey, TValue>
    {
        public PrefsDictionary(string key) : base(key, new SerializableDictionary<TKey, TValue>())
        {
        }

        public PrefsDictionary(string key, IDictionary<TKey, TValue> defaultValue ) : base(key, new SerializableDictionary<TKey, TValue>(defaultValue))
        {
        }
        
        protected void UpdateValue(Action<SerializableDictionary<TKey, TValue>> action)
        {
            var value = Get();
            action(value);
            Set(value);
        }
        
        protected bool UpdateValueIfSuccess(Func<SerializableDictionary<TKey, TValue>, bool> func)
        {
            var value = Get();
            var success = func(value);
            {
                Set(value);
            }

            return success;
        }
        
        
        #region Dictionary Methods

        public bool ContainsValue(TValue value) => Get().ContainsValue(value);
        public bool TryAdd(TKey dictionaryKey, TValue value) => UpdateValueIfSuccess(d => d.TryAdd(dictionaryKey, value));
        
        #endregion
        
        
        #region PrefsListBase<T>
        
        public override int DefaultValueCount => defaultValue.Count;
        public override bool IsDefaultAt(int idx)
        {
            var defaultList = defaultValue.SerializeList;
            if (idx >= defaultList.Count) return false;
            
            var list = Get().SerializeList;
            
            return PrefsAnyUtility.IsEqual(list[idx], defaultList[idx]);
        }

        public override void ResetToDefaultAt(int idx)
        {
            var defaultList = defaultValue.SerializeList;
            if (idx >= defaultList.Count) return;

            var serializableDictionary = Get();
            var list = serializableDictionary.SerializeList;
            list[idx] = defaultList[idx];
            
            Set(serializableDictionary);
        }

        protected override IListAccessor<List<SerializableDictionary<TKey, TValue>.KeyValue>> CreateListAccessor()
            => new ListAccessor(this);
        
        #endregion
        
        
        #region IEnumerable
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        #endregion

        
        #region IEnumerable<KeyValuePair<TKey, TValue>>,

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Get().GetEnumerator();

        #endregion


        #region ICollection<KeyValuePair<TKey, TValue>>
        
        public int Count => Get().Count;
        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)Get()).IsReadOnly;
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        public void Clear() => UpdateValue(d => d.Clear());
        public bool Contains(KeyValuePair<TKey, TValue> item) => Get().Contains(item);
        
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)Get()).CopyTo(array, arrayIndex);
        }

        public bool Remove(TKey dictionaryKey) => UpdateValueIfSuccess(d => d.Remove(dictionaryKey));
        
        #endregion


        
        #region IDictionary<TKey, TValue>

        public TValue this[TKey dictionaryKey]
        {
            get => Get()[dictionaryKey];
            set => UpdateValue(d => d[dictionaryKey] = value);
        }

        public ICollection<TKey> Keys => Get().Keys;
        public ICollection<TValue> Values => Get().Values;
        public void Add(TKey dictionaryKey, TValue value) => UpdateValue(d => d.Add(dictionaryKey, value));
        public bool ContainsKey(TKey dictionaryKey) => Get().ContainsKey(dictionaryKey);
        public bool Remove(KeyValuePair<TKey, TValue> item) => UpdateValueIfSuccess(d => ((ICollection<KeyValuePair<TKey, TValue>>)d).Remove(item));
        public bool TryGetValue(TKey dictionaryKey, out TValue value) => Get().TryGetValue(dictionaryKey, out value);

        #endregion
        
        
        private class ListAccessor : IListAccessor<List<SerializableDictionary<TKey, TValue>.KeyValue>>
        {
            private readonly PrefsDictionary<TKey, TValue> prefs;
            
            public ListAccessor(PrefsDictionary<TKey, TValue> prefs) => this.prefs = prefs;

            public List<SerializableDictionary<TKey, TValue>.KeyValue> InnerList
            {
                get => prefs.Get().SerializeList;
                
                // set が呼ばれることでUI側の変更を通知してもらう
                set
                {
                    var serializedDictionary = prefs.Get();
                    serializedDictionary.SerializeList = value;
                    
                    // SerializeListでPrefsのInner(string)を更新する
                    prefs.Set(serializedDictionary);

                    // prefs.Set()でPrefsのOuterが更新される可能性を考慮して念のため再度Get()
                    // 現状の作りでは更新はないはずだが
                    serializedDictionary = prefs.Get();
                    
                    // SerializedDictionaryのDictionaryをSerializeListで更新する
                    serializedDictionary.OnAfterDeserialize();
                }
            }
        }
    }
}