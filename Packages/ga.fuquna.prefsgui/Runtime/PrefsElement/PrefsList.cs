using System;
using System.Collections;
using System.Collections.Generic;

namespace PrefsGUI
{
    /// <summary>
    /// List style PrefsGUI
    /// </summary>
    [Serializable]
    public class PrefsList<T> : PrefsListBase<List<T>, List<T>>, IList<T>, IList
    {
        public PrefsList(string key, List<T> defaultValue = default) : base(key, defaultValue)
        {
        }


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
        
        protected void UpdateValue(Action<List<T>> action)
        {
            var v = Get();
            action(v);
            Set(v);
        }
        
        
        #region List Method

        public void RemoveAll(Predicate<T> predicate) => UpdateValue(v => v.RemoveAll(predicate));

        #endregion
        
        
        #region PrefsListBase<T>
        
        protected override IListAccessor<List<T>> CreateListAccessor() => new ListAccessor(this);
        
        public override int DefaultValueCount => defaultValue?.Count ?? 0;
        
        public override bool IsDefaultAt(int idx)
        {
            if (idx < DefaultValueCount)
            {
                var current = Get()[idx];
                return PrefsAnyUtility.IsEqual(current, defaultValue[idx]);
            }

            return false;
        }

        public override void ResetToDefaultAt(int idx)
        {
            if (idx < DefaultValueCount)
            {
                var list = Get();
                list[idx] = defaultValue[idx];
                Set(list);
            }
        }
        
        #endregion

        
        #region IList<T>
        
        public T this[int index]
        {
            get => Get()[index];
            set { UpdateValue((v) => v[index] = value); }
        }

        public int IndexOf(T item) => Get().IndexOf(item);
        public void Insert(int index, T item) => UpdateValue((v) => v.Insert(index, item));
        public void RemoveAt(int index) => UpdateValue((v) => v.RemoveAt(index));
        
        #endregion
        
        
        #region ICollection<T>
        
        public int Count => Get()?.Count ?? 0;
        bool ICollection<T>.IsReadOnly => (Get() as ICollection<T>)?.IsReadOnly ?? false;
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
        
        #endregion
        
        
        #region IEnumerable<T>
        
        public IEnumerator<T> GetEnumerator() => Get().GetEnumerator();
        
        #endregion

        
        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => Get().GetEnumerator();
        
        #endregion

        
        #region IList

        bool IList.IsFixedSize => (Get() as IList)?.IsFixedSize ?? false;
        bool IList.IsReadOnly => (Get() as IList)?.IsReadOnly ?? false;
        
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        int IList.Add(object value) => (Get() as IList)?.Add(value) ?? -1;
        bool IList.Contains(object value) => (Get() as IList)?.Contains(value) ?? false;
        int IList.IndexOf(object value) => (Get() as IList)?.IndexOf(value) ?? -1;
        void IList.Insert(int index, object value) => (Get() as IList)?.Insert(index, value);
        void IList.Remove(object value) => (Get() as IList)?.Remove(value);
        
        #endregion
        
        
        #region ICollection

        bool ICollection.IsSynchronized => (Get() as ICollection)?.IsSynchronized ?? false;
        object ICollection.SyncRoot => (Get() as ICollection)?.SyncRoot ?? this;
        void ICollection.CopyTo(Array array, int arrayIndex) => (Get() as ICollection)?.CopyTo(array, arrayIndex);
        
        #endregion

        
        private class ListAccessor : IListAccessor<List<T>>
        {
            private readonly PrefsList<T> prefs;
            
            public ListAccessor(PrefsList<T> prefs) => this.prefs = prefs;

            public List<T> InnerList
            {
                get => prefs.Get();
                set => prefs.Set(value);
            }
        }
    }
}