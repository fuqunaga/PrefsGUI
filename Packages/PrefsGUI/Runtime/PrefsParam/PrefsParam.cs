using System;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI.KVS;
using UnityEngine;
using UnityEngine.Serialization;

namespace PrefsGUI
{
    /// <summary>
    /// Origin of Prefs*
    /// </summary>
    public abstract class PrefsParam : ISerializationCallbackReceiver
    {
        public static Color syncedColor = new Color32(255, 143, 63, 255);

        public event Action<bool> onSyncedChanged;
        
        [SerializeField]
        [FormerlySerializedAs("key")]
        string _key;

        public string key
        {
            get => _key;
            set => ChangeKey(value);
        }

        private bool _synced;
        public bool synced { get => _synced;
            protected set
            {
                if (_synced != value)
                {
                    _synced = value;
                    onSyncedChanged?.Invoke(_synced);
                }
            }
        }

        protected PrefsParam(string key)
        {
            _key = key;
            Register();
        }

        public virtual void Delete() => PrefsKVS.DeleteKey(key);



        #region abstract

        public abstract Type GetInnerType();
        public abstract object GetObject();
        public abstract void SetSyncedObject(object obj, Action onIfAlreadyGet = null);

        public abstract bool IsDefault { get; }
        public abstract void SetCurrentToDefault();
        public abstract void ResetToDefault();

        #endregion


        #region RegistAllInstance

        public static IReadOnlyCollection<PrefsParam> all => _all;
        public static IReadOnlyDictionary<string, PrefsParam> allDic => _allDic;

        static readonly HashSet<PrefsParam> _all = new HashSet<PrefsParam>();
        static readonly Dictionary<string, PrefsParam> _allDic = new Dictionary<string, PrefsParam>();

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize() { Register(); } // To Register Array/List In Inspector. Constructor not called.

        void Register()
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (_allDic.TryGetValue(key, out var prev))
                {
                    _all.Remove(prev);
                }

                var alreadyExist = !_all.Add(this);
                if (alreadyExist)
                {
                    foreach(var removeKey in _allDic.Where(pair => pair.Value == this).Select(pair => pair.Key).ToArray())
                    {
                        _allDic.Remove(removeKey);
                    }
                }

                _allDic[key] = this;
            }
        }

        void ChangeKey(string newKey)
        {
            if (key != newKey && !string.IsNullOrEmpty(newKey))
            {
                _allDic.Remove(key);

                _key = newKey;
                Register();
            }
        }

        #endregion
    }
}