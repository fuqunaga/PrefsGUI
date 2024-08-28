using System.Collections;

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
    }
}