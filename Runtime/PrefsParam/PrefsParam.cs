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
        public string key;
        public static Color syncedColor = new Color32(255, 143, 63, 255);
        public static bool enableWarning = true;

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

        public static Dictionary<string, PrefsParam> all = new Dictionary<string, PrefsParam>();

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize() { Regist(); } // To Regist Array/List In Inspector. Constructor not called.

        void Regist() { all[key] = this; }

        #endregion
    }
}