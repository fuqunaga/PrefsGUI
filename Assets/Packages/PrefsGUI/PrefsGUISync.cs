using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace PrefsGUI
{
    /// <summary>
    /// Sync PrefsGUI parameter over UNET
    /// </summary>
    public class PrefsGUISync : NetworkBehaviour
    {
        #region type define
        public struct KeyObj
        {
            public string key;
            public int value;
        }

        public class SyncListKeyObj : SyncListStruct<KeyObj>{}
        #endregion

        public Dictionary<string, int> _keyToIdx;
        public SyncListKeyObj _syncListKeyObj = new SyncListKeyObj();


        public override void OnStartServer()
        {
            base.OnStartServer();

            _keyToIdx = new Dictionary<string, int>();
            NetworkServer.SpawnObjects();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            ReadPrefs();
        }


        public void Start()
        {
            SendPrefs();
        }

        public void Update()
        {
            SendPrefs();
            ReadPrefs();
        }


        [ServerCallback]
        void SendPrefs()
        {
            PrefsParam.all.Values.ToList().ForEach(prefs =>
            {
                if (prefs.GetInnerType() == typeof(int))
                {
                    var key = prefs.key;
                    var intValue = (int)prefs.GetObject();

                    int idx = -1;
                    if (_keyToIdx.TryGetValue(key, out idx))
                    {
                        if ( _syncListKeyObj[idx].value != intValue )
                        {
                            _syncListKeyObj[idx] = new KeyObj()
                            {
                                key = key,
                                value = intValue
                            };
                        }
                    }
                    else {
                        _keyToIdx[prefs.key] = _syncListKeyObj.Count;
                        _syncListKeyObj.Add(new KeyObj()
                        {
                            key = key,
                            value = intValue
                        });
                    }
                }
            });
        }

        [ClientCallback]
        void ReadPrefs()
        {
            var all = PrefsParam.all;
            _syncListKeyObj.ToList().ForEach(keyObj =>
            {
                PrefsParam prefs;
                if ( all.TryGetValue(keyObj.key, out prefs))
                {
                    prefs.SetObject(keyObj.value);
                }
            });
        }
    }

}