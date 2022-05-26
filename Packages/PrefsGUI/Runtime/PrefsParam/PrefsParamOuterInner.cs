using System;
using System.Collections.Generic;
using PrefsGUI.Kvs;
using UnityEngine.Assertions;

namespace PrefsGUI
{
    /// <summary>
    /// Basic implementation of TOuter and TInner
    /// </summary>
    public abstract class PrefsParamOuterInner<TOuter, TInner> : PrefsParamOuter<TOuter>
    {
        protected bool hasCachedOuter;
        protected TOuter cachedOuter;

        protected bool hasCachedInner;
        protected TInner cachedInner;

        protected bool hasDefaultInner;
        protected TInner defaultInner;

        private PrefsInnerAccessor _prefsInnerAccessor;

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
            return PrefsKvs.Get(key, GetDefaultInner());
        }

        protected bool _Set(TInner v, bool syncedFlag = false)
        {
            var updateValue = (false == Equals(v, _Get()));
            if (updateValue)
            {
                PrefsKvs.Set(key, v);
                hasCachedOuter = false;
                hasCachedInner = false;
            }

            synced = syncedFlag;

            return updateValue;
        }

        protected virtual bool Equals(TInner lhs, TInner rhs) => EqualityComparer<TInner>.Default.Equals(lhs, rhs);

        // TODO: move out
        public virtual Dictionary<string, string> GetCustomLabel() => null;


        private TInner GetInner()
        {
            if (!hasCachedInner)
            {
                cachedInner = _Get();
                hasCachedInner = true;
            }

            return cachedInner;
        }


        #region abstract

        protected abstract TOuter ToOuter(TInner innerV);
        protected abstract TInner ToInner(TOuter outerV);

        #endregion


        #region override

        public override TOuter Get()
        {
            if (!hasCachedOuter)
            {
                cachedOuter = ToOuter(_Get());
                hasCachedOuter = true;
            }

            return cachedOuter;
        }

        public override void Set(TOuter v) => _Set(ToInner(v));

        public override Type GetInnerType() => typeof(TInner);

        public override bool IsDefault => Equals(GetDefaultInner(), _Get());

        public override void SetCurrentToDefault()
        {
            defaultValue = Get();
            hasDefaultInner = false;
        }

        public override IPrefsInnerAccessor<T> GetInnerAccessor<T>()
        {
            Assert.AreEqual(typeof(T), typeof(TInner));
            _prefsInnerAccessor ??= new(this);
            return (IPrefsInnerAccessor<T>) _prefsInnerAccessor;
        }

        #endregion


        #region GUI Implement

        public bool DoGUICheckChanged(Func<TOuter, TOuter> func)
        {
            var changed = false;
            if (!PrefsKvs.HasKey(key))
            {
                Set(defaultValue);
                changed = true;
            }

            var prev = Get();
            var prevInner = ToInner(prev);

            var next = func(prev);
            var nextInner = ToInner(next);

            if (!Equals(prevInner, nextInner))
            {
                _Set(nextInner);
                changed = true;
            }

            return changed;
        }

        #endregion


        #region InnerAccessor

        public class PrefsInnerAccessor : IPrefsInnerAccessor<TInner>
        {
            private readonly PrefsParamOuterInner<TOuter, TInner> _prefs;

            public PrefsInnerAccessor(PrefsParamOuterInner<TOuter, TInner> prefs)
            {
                _prefs = prefs;
            }

            #region IPrefsInnerAccessor

            public PrefsParam Prefs => _prefs;

            public bool IsAlreadyGet => _prefs.hasCachedInner || _prefs.hasCachedOuter;
            public TInner Get() => _prefs.GetInner();

            public bool SetSyncedValue(TInner value)
            {
                return _prefs._Set(value, true);
            }

            public bool Equals(TInner lhs, TInner rhs) => _prefs.Equals(lhs, rhs);

            #endregion
        }

        #endregion
    }
}