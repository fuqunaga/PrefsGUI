using RapidGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrefsGUI
{
    /// <summary>
    /// List style PrefsGUI
    /// </summary>
    public class PrefsList<T> : PrefsAny<List<T>>, IList<T>
    {
        public PrefsList(string key, List<T> defaultValue = default) : base(key, defaultValue) { }

        protected int defaultValueCount => defaultValue?.Count ?? 0;

        protected virtual bool IsEqual(T lhs, T rhs)
        {
            return PrefsAnyUtility.ToInner(lhs) == PrefsAnyUtility.ToInner(rhs);
        }

        public override bool DoGUI(string label = null)
        {
            return DoGUIStrandard(
                (v) => RGUI.ListField(v, label ?? key, (list, idx, elemLabel) => DoGUIAt_(list, idx, elemLabel)),
                false);
        }

        protected virtual T DoGUIAt_(List<T> list, int idx, string label)
        {
            using (new GUILayout.HorizontalScope())
            {
                var ret = list[idx];
                ret = RGUI.Field(ret, label);

                // defaultButton
                if (idx < defaultValueCount)
                {
                    var dv = defaultValue[idx];
                    if (DoGUIDefaultButton(IsEqual(ret, dv)))
                    {
                        ret = dv;
                    }
                }
                return ret;
            }
        }

        public virtual bool DoGUIAt(int idx, string label = null)
        {
            var list = Get();
            var prev = list[idx];

            var prevInner = PrefsAnyUtility.ToInner(prev);

            var next = DoGUIAt_(list, idx, label);
            var nextInner = PrefsAnyUtility.ToInner(next);

            var changed = (prevInner != nextInner);
            if (changed)
            {
                list[idx] = next;
                Set(list);
            }

            return changed;
        }



        #region IList<T>

        protected void UpdateValue(Action<List<T>> action) { var v = Get(); action(v); Set(v); }

        public int Count => Get().Count;
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
    }
}
