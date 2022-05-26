using System;
using System.Collections;
using System.Collections.Generic;

namespace PrefsGUI
{
    /// <summary>
    /// List style PrefsGUI
    /// </summary>
    [Serializable]
    public class PrefsList<T> : PrefsAny<List<T>>, IList<T>
    {
        public PrefsList(string key, List<T> defaultValue = default) : base(key, defaultValue) { }

        public int DefaultValueCount => defaultValue?.Count ?? 0;
        
        public bool IsDefaultCount => DefaultValueCount == Count;

        public void ResetToDefaultCount()
        {
            if (!IsDefaultCount)
            {
                var list = Get() ?? new();
                var listCount = list.Count;

                if (DefaultValueCount > listCount)
                {
                    list.AddRange(defaultValue.GetRange(listCount, DefaultValueCount - listCount));
                }
                else if (DefaultValueCount < listCount)
                {
                    list.RemoveRange(DefaultValueCount, listCount - DefaultValueCount);
                }
                
                Set(list);
            }
        }
        
        public bool IsDefaultAt(int idx)
        {
            if (idx < DefaultValueCount)
            {
                var current = Get()[idx];
                return PrefsAnyUtility.IsEqual(current, defaultValue[idx]);
            }

            return false;
        }

        public void ResetToDefaultAt(int idx)
        {
            if (idx < DefaultValueCount)
            {
                var list = Get();
                list[idx] = defaultValue[idx];
                Set(list);
            }
        }
 
        #region IList<T>

        protected void UpdateValue(Action<List<T>> action) { var v = Get(); action(v); Set(v); }

        public int Count => Get()?.Count ?? 0;
        public bool IsReadOnly => false;
        public T this[int index] { get => Get()[index]; set { UpdateValue((v) => v[index] = value); } }
        public int IndexOf(T item) => Get().IndexOf(item);
        public void Insert(int index, T item) => UpdateValue((v) => v.Insert(index, item));
        public void RemoveAt(int index) => UpdateValue((v) => v.RemoveAt(index));
        public void Add(T item) => UpdateValue((v) => v.Add(item));
        public void Clear() => UpdateValue((v) => v.Clear());
        public bool Contains(T item) => Get().Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => Get().CopyTo(array, arrayIndex);
        public bool Remove(T item)
        {
            var v = Get();
            var ret = v.Remove(item);
            Set(v);
            return ret;
        }
        public IEnumerator<T> GetEnumerator() => Get().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Get().GetEnumerator();

        #endregion
        
        #region List like
        public void RemoveAll(Predicate<T> predicate) => UpdateValue(v => v.RemoveAll(predicate));

        #endregion
    }
}
