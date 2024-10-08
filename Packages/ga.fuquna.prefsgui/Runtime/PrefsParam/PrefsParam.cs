﻿using System;
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
        #region Static
        
        private static readonly HashSet<PrefsParam> All = new();
        private static readonly Dictionary<string, PrefsParam> AllDic = new();
        private static readonly Dictionary<string, Action> KeyToOnValueChangedCallback = new();

        
        public static event Action<PrefsParam> onRegisterPrefsParam;
        
        public static IReadOnlyCollection<PrefsParam> all => All;
        public static IReadOnlyDictionary<string, PrefsParam> allDic => AllDic;
        
        #endregion
        
        
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

        public virtual void Delete()
        {
            ClearCache();
            PrefsKvs.DeleteKey(key);
        }

        // 保持している一時変数などを初期化する
        // EditorのPlayModeが変化したときに呼ばれる
        public virtual void Reset()
        {
            ClearSync();
            ClearCache();
        }

        public virtual void ClearCache()
        {
        }

        protected virtual void OnKeyChanged(string oldKey, string newKey)
        {
            if (!string.IsNullOrEmpty(oldKey))
            {
                AllDic.Remove(oldKey);
            }

            Register();
        }

        protected virtual void OnValueChanged()
        {
            if (KeyToOnValueChangedCallback.TryGetValue(key, out var action))
            {
                action?.Invoke();
            }
        }

        public void RegisterValueChangedCallback(Action callback)
        {
            KeyToOnValueChangedCallback.TryGetValue(key, out var action);
            KeyToOnValueChangedCallback[key] = action + callback;
        }

        public void UnregisterValueChangedCallback(Action callback)
        {
            if (!KeyToOnValueChangedCallback.TryGetValue(key, out var action)) return;
            KeyToOnValueChangedCallback[key] = action - callback;
        }

        #region abstract

        public abstract Type GetInnerType();
        public abstract bool IsDefault { get; }
        public abstract void SetCurrentToDefault();
        public abstract void ResetToDefault();
        public abstract IPrefsInnerAccessor<T> GetInnerAccessor<T>();

        #endregion


        #region RegistAllInstance

        
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
            if (string.IsNullOrEmpty(key)) return;
            
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
            
            onRegisterPrefsParam?.Invoke(this);
        }

        #endregion
    }
}