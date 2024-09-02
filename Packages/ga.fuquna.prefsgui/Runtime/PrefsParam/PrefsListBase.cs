using System.Collections;
using System.Collections.Generic;

namespace PrefsGUI
{
    public abstract class PrefsListBase<TList, TListForUI> : PrefsAny<TList>
        where TListForUI : IList
        where TList : new()
    {
        private IListAccessor<TListForUI> _listAccessor;
        
        public abstract int DefaultValueCount { get; }
        public abstract bool IsDefaultAt(int idx);
        public abstract void ResetToDefaultAt(int idx);
        

        protected PrefsListBase(string key, TList defaultValue = default) : base(key, defaultValue)
        {
        }
        
        public IListAccessor<TListForUI> GetListAccessor() => _listAccessor ??= CreateListAccessor();
        
        protected abstract IListAccessor<TListForUI> CreateListAccessor();
        
        protected bool SetListItemIfNotEqual<T>(IList<T> list, int idx, T value)
        {
            if (idx >= list.Count) 
                return false;

            var listItemInner = PrefsAnyUtility.ToInner(list[idx]);
            var valueInner = PrefsAnyUtility.ToInner(value);

            if (listItemInner == valueInner)
                return false;
            
            // Tがクラスの場合、list[idx] = value; とすると同じオブジェクトを参照してしまう
            // ResetToDefaultAt()でdefaultValueの要素を参照してしまうとdefaultValueの値が編集できてしまいまずいので、
            // Inner型を経由してvalueを別のインスタンスにする
            list[idx] = PrefsAnyUtility.ToOuter<T>(valueInner);
            return true;
        }
    }
}