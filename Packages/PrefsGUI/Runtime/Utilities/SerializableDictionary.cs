using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrefsGUI.Utility
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        private struct KeyValue
        {
            public TKey key;
            public TValue value;

            public KeyValue(TKey key, TValue value) => (this.key, this.value) = (key, value);
        }

        [SerializeField] private List<KeyValue> _list;

        private readonly Dictionary<TKey, TValue> _dictionary = new();

        private bool _isDirty;


        protected void SetDirty() => _isDirty = true;


        #region IDictionary<TKey, TValue>

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                _dictionary[key] = value;
                SetDirty();
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            SetDirty();
        }

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public bool Remove(TKey key)
        {
            var success = _dictionary.Remove(key);
            SetDirty();
            return success;
        }

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item.Key, item.Value);
            SetDirty();
        }

        public void Clear()
        {
            _dictionary.Clear();
            SetDirty();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);


        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item))
                return false;

            SetDirty();
            return true;
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

        #endregion


        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {
            if (!_isDirty) return;
            _isDirty = false;

            _list.Clear();
            _list.AddRange(_dictionary.Select(kv => new KeyValue(kv.Key, kv.Value)));
        }

        public void OnAfterDeserialize()
        {
            _dictionary.Clear();
            foreach (var item in _list.Where(kv => kv.key != null))
            {
                _dictionary.TryAdd(item.key, item.value);
            }
        }

        #endregion
    }
}