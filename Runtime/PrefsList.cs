using System;
using System.Collections;
using System.Collections.Generic;

namespace PrefsGUI
{
    /// <summary>
    /// List style PrefsGUI
    /// </summary>
    public class PrefsList<T> : PrefsAny<List<T>>, IList<T>
    {
        public PrefsList(string key, List<T> defaultValue = default) : base(key, defaultValue) { }


        #region IList<T>
        protected void UpdateValue(Action<List<T>> action) { var v = Get(); action(v); Set(v); }

        public int Count { get { return Get().Count; } }
        public bool IsReadOnly { get { return false; } }
        public T this[int index] { get { return Get()[index]; } set { UpdateValue((v) => v[index] = value); } }
        public int IndexOf(T item) { return Get().IndexOf(item); }
        public void Insert(int index, T item) { UpdateValue((v) => v.Insert(index, item)); }
        public void RemoveAt(int index) { UpdateValue((v) => v.RemoveAt(index)); }
        public void Add(T item) { UpdateValue((v) => v.Add(item)); }
        public void Clear() { UpdateValue((v) => v.Clear()); }
        public bool Contains(T item) { return Get().Contains(item); }
        public void CopyTo(T[] array, int arrayIndex) { Get().CopyTo(array, arrayIndex); }
        public bool Remove(T item)
        {
            var v = Get();
            var ret = v.Remove(item);
            Set(v);
            return ret;
        }
        public IEnumerator<T> GetEnumerator() { return Get().GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return Get().GetEnumerator(); }
        #endregion
    }
}
