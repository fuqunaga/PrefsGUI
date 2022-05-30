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
        private event Action onValueChanged;

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

        protected bool _Set(TInner v)
        {
            var updateValue = (false == Equals(v, _Get()));
            if (updateValue)
            {
                PrefsKvs.Set(key, v);
                hasCachedOuter = false;
                hasCachedInner = false;

                onValueChanged?.Invoke();
            }

            return updateValue;
        }

        protected virtual bool Equals(TInner lhs, TInner rhs) => EqualityComparer<TInner>.Default.Equals(lhs, rhs);

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

        public override bool Set(TOuter v) => _Set(ToInner(v));

        public override Type GetInnerType() => typeof(TInner);

        public override bool IsDefault => Equals(GetDefaultInner(), _Get());

        public override void SetCurrentToDefault()
        {
            defaultValue = Get();
            hasDefaultInner = false;
        }

        public override void RegisterValueChangedCallback(Action callback) => onValueChanged += callback;
        public override void UnregisterValueChangedCallback(Action callback) => onValueChanged -= callback;


        public override IPrefsInnerAccessor<T> GetInnerAccessor<T>()
        {
            Assert.AreEqual(typeof(T), typeof(TInner));
            _prefsInnerAccessor ??= new(this);
            return (IPrefsInnerAccessor<T>) _prefsInnerAccessor;
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
                _prefs.UnregisterValueChangedCallback(UpdateSyncedFlag);
                
                var ret =  _prefs._Set(value);
                _prefs.Synced = true;
                
                _prefs.RegisterValueChangedCallback(UpdateSyncedFlag);

                return ret;

                void UpdateSyncedFlag()
                {
                    _prefs.Synced = Equals(value, Get());
                }
            }

            public bool Equals(TInner lhs, TInner rhs) => _prefs.Equals(lhs, rhs);
            
            #endregion
        }

        #endregion
    }
}