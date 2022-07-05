using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PrefsGUI.Kvs
{
    /// <summary>
    /// Avoid boxing/unboxing object cache
    /// </summary>
    public class KvsCache : IEnumerable<(string, object)>
    {
        private readonly Dictionary<string, (IList, int)> keyToListAndIndex = new();
        private readonly Dictionary<Type, IList> typeToList = new();

        public bool ContainsKey(string key) => keyToListAndIndex.ContainsKey(key);
        
        public void Set<T>(string key, T value)
        {
            if (keyToListAndIndex.TryGetValue(key, out var pair))
            {
                var (list, index) = pair;
                ((List<T>) list)[index] = value;
            }
            else
            {
                if (!typeToList.TryGetValue(typeof(T), out var iList))
                {
                    iList = new List<T>();
                    typeToList[typeof(T)] = iList;
                }

                ((List<T>) iList).Add(value);
                pair = (iList, iList.Count - 1);
                keyToListAndIndex[key] = pair;
            }
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (!keyToListAndIndex.TryGetValue(key, out var pair))
            {
                value = default;
                return false;
            }

            var (list, index) = pair;
            value = ((List<T>) list)[index];
            return true;
        }

        public bool Remove(string key) => keyToListAndIndex.Remove(key);


        public void Clear()
        {
            keyToListAndIndex.Clear();
            typeToList.Clear();
        }


        public IEnumerator<(string, object)> GetEnumerator()
        {
            foreach (var (key, pair) in keyToListAndIndex)
            {
                var (list, index) = pair;
                yield return (key, list[index]);
            }
        }
        
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}