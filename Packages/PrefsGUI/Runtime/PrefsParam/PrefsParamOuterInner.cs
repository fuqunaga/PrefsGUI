using System;
using System.Collections.Generic;
using PrefsGUI.KVS;

namespace PrefsGUI
{
    /// <summary>
    /// Basic implementation of OuterT and InnnerT
    /// </summary>
    public abstract class PrefsParamOuterInner<TOuter, TInner> : PrefsParamOuter<TOuter>
    {
        protected bool isCachedOuter;
        protected TOuter cachedOuter;

        protected bool isCachedObj;
        protected object cachedObj;

        public bool synced { get; protected set; }

        protected bool hasDefaultInner;
        protected TInner defaultInner;


        protected PrefsParamOuterInner(string key, TOuter defaultValue = default) : base(key, defaultValue)
        {
        }

        protected TInner GetDefaultInner()
        {
            if (!hasDefaultInner)
            {
                defaultInner = ToInner(defaultValue);
                hasDefaultInner = true;
            }

            return defaultInner;
        }

        protected TInner _Get()
        {
            return PrefsKVS.Get(key, GetDefaultInner());
        }

        protected void _Set(TInner v, bool syncedFlag = false, Action onIfAlreadyGet = null)
        {
            if (false == Compare(v, _Get()))
            {
                if (onIfAlreadyGet != null && !this.synced && syncedFlag)
                {
                    if (isCachedOuter || isCachedObj)
                    {
                        onIfAlreadyGet();
                    }
                }

                PrefsKVS.Set(key, v);
                isCachedOuter = false;
                isCachedObj = false;
            }

            synced = syncedFlag;
        }

        public virtual bool Compare(TInner lhs, TInner rhs) => EqualityComparer<TInner>.Default.Equals(lhs, rhs);

        public virtual Dictionary<string, string> GetCustomLabel() => null;



        #region abstract

        protected abstract TOuter ToOuter(TInner innerV);
        protected abstract TInner ToInner(TOuter outerV);

        #endregion



        #region override

        public override TOuter Get()
        {
            if (!isCachedOuter)
            {
                cachedOuter = ToOuter(_Get());
                isCachedOuter = true;
            }
            return cachedOuter;
        }

        public override void Set(TOuter v) { _Set(ToInner(v)); }

        public override Type GetInnerType()
        {
            return typeof(TInner);
        }
        public override object GetObject()
        {
            if (!isCachedObj)
            {
                cachedObj = _Get();
                isCachedObj = true;
            }

            return cachedObj;
        }
        public override void SetSyncedObject(object obj, Action onIfAlreadyGet = null)
        {
            _Set((TInner)obj, true, onIfAlreadyGet);
        }

        public override bool IsDefault => Compare(GetDefaultInner(), _Get());

        public override void SetCurrentToDefault()
        {
            defaultValue = Get();
            hasDefaultInner = false;
        }

        #endregion



        #region GUI Implement
        
        public bool DoGUICheckChanged(Func<TOuter, TOuter> func)
        {
            var changed = false;
            if (!PrefsKVS.HasKey(key))
            {
                Set(defaultValue);
                changed = true;
            }

            var prev = Get();
            var prevInner = ToInner(prev);

            var next = func(prev);
            var nextInner = ToInner(next);

            if (!Compare(prevInner, nextInner))
            {
                _Set(nextInner);
                changed = true;
            }

            return changed;
        }

        #endregion
    }
}