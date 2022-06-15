using System;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI.Kvs;
using UnityEngine;
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

        static readonly HashSet<PrefsParam> All = new();
        static readonly Dictionary<string, PrefsParam> AllDic = new();

        public void OnBeforeSerialize() { }

        // To Register Array/List In Inspector. constructor is not called.
        public void OnAfterDeserialize() => Register();

        void Register()
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
                    foreach(var removeKey in AllDic.Where(pair => pair.Value == this).Select(pair => pair.Key).ToArray())
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