using System;
using System.Collections.Generic;
using PrefsGUI.Kvs;
using UnityEngine.Assertions;

namespace PrefsGUI
{
    /// <summary>
    /// Basic implementation of TOuter and TInner
    /// デフォルト値は各インスタンスごとに固有で持つが、Get(),Set()はKvsと透過的に行う（＝同一キーなら同一の値）
    /// </summary>
    public abstract class PrefsParamOuterInner<TOuter, TInner> : PrefsParamOuter<TOuter>
    {
        #region Type Define
        
        public class CachedValue<T>
        {
            private bool hasValue;
            private T value;

            public bool HasValue => hasValue;
            
            public bool TryGet(out T v)
            {
                v = value;
                return hasValue;
            }

            public void Set(T v)
            {
                value = v;
                hasValue = true;
            }

            public void Clear() => hasValue = false;
        }
        
        public class OuterInnerCache
        {
            public readonly CachedValue<TOuter> outer = new();
            public readonly CachedValue<TInner> inner = new();

            public void Clear()
            {
                outer.Clear();
                inner.Clear();
            }
        }
        
        #endregion
        

        private static readonly Dictionary<string, OuterInnerCache> keyToCache = new();
        

        private CachedValue<TInner> _defaultValueInnerCache = new();
        private OuterInnerCache _cache;
        private PrefsInnerAccessor _prefsInnerAccessor;
        
        protected PrefsParamOuterInner(string key, TOuter defaultValue = default) : base(key, defaultValue)
        {
        }

        protected virtual bool Equals(TInner lhs, TInner rhs) => EqualityComparer<TInner>.Default.Equals(lhs, rhs);
        
        protected TInner GetDefaultInner()
        {
            if (!_defaultValueInnerCache.TryGet(out var value))
            {
                value = ToInner(defaultValue);
                _defaultValueInnerCache.Set(value);
            }
            
            return value;
        }

        private TInner GetInner()
        {
            if (!_cache.inner.TryGet(out var value))
            {
                value = PrefsKvs.Get(key, GetDefaultInner());
                _cache.inner.Set(value);
            }

            return value;
        }
        
        protected bool SetInner(TInner v)
        {
            var updateValue = !Equals(v, GetInner());
            if (updateValue)
            {
                PrefsKvs.Set(key, v);
                _cache.Clear();

                OnValueChanged();
            }

            return updateValue;
        }


        #region abstract

        protected abstract TOuter ToOuter(TInner inner);
        protected abstract TInner ToInner(TOuter outer);

        #endregion


        #region override
        
        public override void ClearCache()
        {
            base.ClearCache();
            _cache.Clear();
        }
        
        protected override void OnKeyChanged(string oldKey, string newKey)
        {
            base.OnKeyChanged(oldKey, newKey);
            if (!keyToCache.TryGetValue(newKey, out _cache))
            {
                keyToCache[newKey] = _cache = new ();
            }
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            
            // defaultValueがInspectorで書き換えられてる可能性がある
            _defaultValueInnerCache ??= new();
            _defaultValueInnerCache.Clear();
        }

        public override TOuter Get()
        {
            if (!_cache.outer.TryGet(out var value))
            {
                value = ToOuter(GetInner());
                _cache.outer.Set(value);
            }

            return value;
        }

        public override bool Set(TOuter v) => SetInner(ToInner(v));

        public override Type GetInnerType() => typeof(TInner);

        public override bool IsDefault => Equals(GetDefaultInner(), GetInner());

        public override void SetCurrentToDefault()
        {
            // TOuterがクラスだと、defaultValueと今後Get()で返ってくるインスタンスが同一なってしまい、
            // 外部でdefaultValueの値を書き換えることが可能になってしまう。したがって次のコードはまずい
            //
            // 　defaultValue = Get();
            //
            // ToOuter(GetInner())で新しいインスタンスを作る
            // TInnerがクラスでToOuter()で何もしない処理だと同様の問題があるが、現状TInnerがクラスなのはstringのみなので大丈夫
            defaultValue = ToOuter(GetInner());
            _defaultValueInnerCache.Clear();
        }

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

            public bool IsAlreadyGet => _prefs._cache.outer.HasValue || _prefs._cache.inner.HasValue;
            public TInner Get() => _prefs.GetInner();

            public bool SetSyncedValue(TInner value)
            {
                _prefs.UnregisterValueChangedCallback(UpdateSyncedFlag);
                
                var ret =  _prefs.SetInner(value);
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