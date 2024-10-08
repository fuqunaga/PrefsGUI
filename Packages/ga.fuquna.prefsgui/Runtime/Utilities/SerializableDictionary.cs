﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrefsGUI.Utility
{
    public interface ISerializableDictionaryForUI
    {
        public IEnumerable<IEnumerable<int>> GetSameKeyIndexGroups();
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

        private bool _isDictionaryDirty = true;

        internal List<KeyValue> SerializeList
        {
            get => _list;
            set
            {
                _list = value;
                UpdateDictionaryFromList();
            }
        }
        
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
        
        
        protected void SetDictionaryDirty() => _isDictionaryDirty = true;

        #region Dictionary Methods

        public bool ContainsValue(TValue value) => _dictionary.ContainsValue(value);
        
        public bool TryAdd(TKey key, TValue value)
        {
            var success = _dictionary.TryAdd(key, value);
            if (success)
            {
                SetDictionaryDirty();
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
            SetDictionaryDirty();
        }

        public void Clear()
        {
            _dictionary.Clear();
            SetDictionaryDirty();
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

            SetDictionaryDirty();
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
                SetDictionaryDirty();
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            SetDictionaryDirty();
        }

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public bool Remove(TKey key)
        {
            var success = _dictionary.Remove(key);
            SetDictionaryDirty();
            return success;
        }

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        #endregion

        
        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize() => UpdateDictionaryFromList();

        public void OnAfterDeserialize() => UpdateDictionaryFromList();

        #endregion
        
        
        #region ISerializableDictionaryForUI

        public IEnumerable<IEnumerable<int>> GetSameKeyIndexGroups()
        {
            return _list.Select((kv, index) => (kv.key, index))
                .GroupBy(kv => kv.key, _dictionary.Comparer)
                .Where(g => g.Count() > 1)
                .Select(g => g.Select(kv => kv.index).OrderBy(index => index));
        }
        
        public int SerializableItemCount => _list.Count;
        
        #endregion

        
        private void UpdateDictionaryFromList()
        {
            _dictionary.Clear();
            if (_list == null) return;
            foreach (var item in _list.Where(kv => kv.key != null))
            {
                _dictionary.TryAdd(item.key, item.value);
            }
        }
        
        private void UpdateListFromDictionary()
        {
            if (!_isDictionaryDirty) return;
            _isDictionaryDirty = false;
            
            _list.Clear();
            _list.AddRange(_dictionary.Select(kv => new KeyValue(kv.Key, kv.Value)));
        }
    }
}