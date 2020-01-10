using PrefsGUI.KVS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrefsGUI
{
    /// <summary>
    /// Origin of Prefs*
    /// </summary>
    public abstract class PrefsParam : ISerializationCallbackReceiver
    {
        public static Color syncedColor = new Color32(255, 143, 63, 255);

        public string key;

        public PrefsParam(string key)
        {
            this.key = key;
            Regist();
        }

        public virtual void Delete() { PrefsKVS.DeleteKey(key); }



        #region abstract

        public abstract Type GetInnerType();
        public abstract object GetObject();
        public abstract void SetSyncedObject(object obj, Action onIfAlreadyGet);

        public abstract bool DoGUI(string label = null);
        public abstract bool IsDefault { get; }
        public abstract void SetCurrentToDefault();

        #endregion


        #region RegistAllInstance

        public static readonly List<PrefsParam> all = new List<PrefsParam>();
        public static readonly Dictionary<string, PrefsParam> allDic = new Dictionary<string, PrefsParam>();

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize() { Regist(); } // To Regist Array/List In Inspector. Constructor not called.

        void Regist()
        {
            if ( allDic.TryGetValue(key, out var prev) )
            {
                all.Remove(prev);
            }

            allDic[key] = this;
            all.Add(this);
        }

        #endregion
    }
}