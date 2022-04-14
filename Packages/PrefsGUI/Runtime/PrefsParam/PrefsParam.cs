using PrefsGUI.KVS;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField]
        [FormerlySerializedAs("key")]
        string _key;

        public string key
        {
            get => _key;
            set => ChangeKey(value);
        }

        public PrefsParam(string key)
        {
            _key = key;
            Regist();
        }

        public virtual void Delete() => PrefsKVS.DeleteKey(key);



        #region abstract

        public abstract Type GetInnerType();
        public abstract object GetObject();
        public abstract void SetSyncedObject(object obj, Action onIfAlreadyGet);

        public abstract bool DoGUI(string label = null);
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

        public void OnAfterDeserialize() { Regist(); } // To Regist Array/List In Inspector. Constructor not called.

        void Regist()
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
                    _allDic.Where(pair => pair.Value == this)
                        .Select(pair => pair.Key)
                        .ToList()
                        .ForEach(removeKey =>
                    {
                        _allDic.Remove(removeKey);
                    });
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
                Regist();
            }
        }

        #endregion
    }
}