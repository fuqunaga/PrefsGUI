using System;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI.Kvs;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace PrefsGUI
{
    /// <summary>
    /// Origin of Prefs*
    /// </summary>
    public abstract partial class PrefsParam : ISerializationCallbackReceiver
    {
        [SerializeField]
        [FormerlySerializedAs("key")]
        private string _key;
        
        public string key
        {
            get => _key;
            set
            {
                if (_key != value && !string.IsNullOrEmpty(value))
                {
                    var old = _key;
                    _key = value;
                    OnKeyChanged(old, _key);
                }
            }
        }


        protected PrefsParam(string key) => this.key = key;

        public virtual void Delete() => PrefsKvs.DeleteKey(key);


        protected virtual void OnKeyChanged(string oldKey, string newKey)
        {
            if (!string.IsNullOrEmpty(oldKey))
            {
                AllDic.Remove(oldKey);
            }

            Register();
        }

        #region abstract

        public abstract Type GetInnerType();
        public abstract bool IsDefault { get; }
        public abstract void SetCurrentToDefault();
        public abstract void ResetToDefault();
        public abstract void RegisterValueChangedCallback(Action callback);
        public abstract void UnregisterValueChangedCallback(Action callback);
        public abstract IPrefsInnerAccessor<T> GetInnerAccessor<T>();

        #endregion


        #region RegistAllInstance

        public static IReadOnlyCollection<PrefsParam> all => All;
        public static IReadOnlyDictionary<string, PrefsParam> allDic => AllDic;

        private static readonly HashSet<PrefsParam> All = new();
        private static readonly Dictionary<string, PrefsParam> AllDic = new();

        
        #region ISerializationCallbackReceiver
        
        public void OnBeforeSerialize() {}

        // To Register Array/List In Inspector. constructor is not called.
        // Inspectorで値を変えても呼ばれる。Key、DefaultValueが更新されてる可能性がある
        public virtual void OnAfterDeserialize()
        {
            // _key が書き換えられてる場合があるのでOnKeyChange()を直接呼ぶ
            OnKeyChanged(null, _key);
        }
        
        #endregion
        

        private void Register()
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (AllDic.TryGetValue(key, out var prev))
                {
                    All.Remove(prev);
                }

                var alreadyExist = !All.Add(this);
                if (alreadyExist)
                {
                    using var _ = ListPool<string>.Get(out var keys);
                    keys.AddRange(AllDic.Where(pair => pair.Value == this).Select(pair => pair.Key));

                    foreach(var removeKey in keys)
                    {
                        AllDic.Remove(removeKey);
                    }
                }

                AllDic[key] = this;
            }
        }

        #endregion
    }
}