using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrefsGUI.Utility
{
    public interface ISerializableDictionaryForUI
    {
        IEnumerable<int> GetDuplicatedKeyIndices();
        int SerializableItemCount { get; }
    }
    
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver, ISerializableDictionaryForUI
    {
        [Serializable]
        public struct KeyValue
        {
            public TKey key;
            public TValue value;

            public KeyValue(TKey key, TValue value) => (this.key, this.value) = (key, value);
        }

        [SerializeField] 
        private List<KeyValue> _list = new();

        private readonly Dictionary<TKey, TValue> _dictionary;

        private bool _isDirty = true;

        internal List<KeyValue> SerializeList => _list;
        
        
        public SerializableDictionary() => _dictionary = new Dictionary<TKey, TValue>();

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            => _dictionary = new Dictionary<TKey, TValue>(dictionary);

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            => _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);

        public SerializableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            => _dictionary = new Dictionary<TKey, TValue>(collection);

        public SerializableDictionary(
            IEnumerable<KeyValuePair<TKey, TValue>> collection,
            IEqualityComparer<TKey> comparer)
            => _dictionary = new Dictionary<TKey, TValue>(collection, comparer);

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            => _dictionary = new Dictionary<TKey, TValue>(comparer);
        
        
        protected void SetDirty() => _isDirty = true;

        #region Dictionary Methods

        public bool ContainsValue(TValue value) => _dictionary.ContainsValue(value);
        
        public bool TryAdd(TKey key, TValue value)
        {
            var success = _dictionary.TryAdd(key, value);
            if (success)
            {
                SetDirty();
            }

            return success;
        }
        
        #endregion
        
        
        #region IEnumerable
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        #endregion

        
        #region IEnumerable<KeyValuePair<TKey, TValue>>,

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        #endregion
        
        
        #region ICollection<KeyValuePair<TKey, TValue>>
        
        public int Count => _dictionary.Count;
        
        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

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
        
        #endregion
        
        
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
            if (_list == null) return;
            foreach (var item in _list.Where(kv => kv.key != null))
            {
                _dictionary.TryAdd(item.key, item.value);
            }
        }

        #endregion
        
        
        #region ISerializableDictionary

        public IEnumerable<int> GetDuplicatedKeyIndices()
        {
            return _list.Select((kv, index) => (kv.key, index))
                .GroupBy(kv => kv.key, _dictionary.Comparer)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .Select(kv => kv.index);
        }
        
        public int SerializableItemCount => _list.Count;
        
        #endregion
    }
}