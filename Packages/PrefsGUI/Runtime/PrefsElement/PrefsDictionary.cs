using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI.Utility;

namespace PrefsGUI
{
    /// <summary>
    /// List style PrefsGUI
    /// </summary>
    [Serializable]
    public class PrefsDictionary<TKey, TValue> : PrefsAny<SerializableDictionary<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        public PrefsDictionary(string key, SerializableDictionary<TKey, TValue> defaultValue = default) : base(key, defaultValue)
        {
        }
        
        protected void UpdateValue(Action<SerializableDictionary<TKey, TValue>> action)
        {
            var value = Get();
            action(value);
            Set(value);
        }
        
        protected ICollection<KeyValuePair<TKey, TValue>> GetAsCollection() => Get();

        
        #region IEnumerable
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        #endregion

        
        #region  IEnumerable<out T>
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Get().GetEnumerator();
        
        #endregion
        
        
        #region ICollection<KeyValuePair<TKey, TValue>>
        
        protected void UpdateValue(Action<ICollection<KeyValuePair<TKey, TValue>>> action)
        {
            var value = Get();
            action(value);
            Set(value);
        }

        public int Count => Get().Count;
        public bool IsReadOnly => GetAsCollection().IsReadOnly;
        
        public void Add(KeyValuePair<TKey, TValue> item) => UpdateValue(d => d.Add(item));

        public void Clear() => UpdateValue(d => d.Clear());
        
        public bool Contains(KeyValuePair<TKey, TValue> item) => Get().Contains(item);
        
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => GetAsCollection().CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var ret = false;
            UpdateValue(d => ret = d.Remove(item));
            return ret;
        }
        
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

        public bool Remove(TKey dictionaryKey)
        {
            var ret = false;
            UpdateValue(d => ret = d.Remove(dictionaryKey));
            return ret;
        }

        public bool TryGetValue(TKey dictionaryKey, out TValue value) => Get().TryGetValue(dictionaryKey, out value);
        
        #endregion
    }
}