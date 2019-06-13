using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

#pragma warning disable 0618  

namespace PrefsGUI
{
    /// <summary>
    /// Sync PrefsGUI parameter over UNET
    /// </summary>
    public partial class PrefsGUISync : NetworkBehaviour
    {
        #region Type Define

        public class TypeAndIdx
        {
            public Type type;
            public int idx;
        }

        #endregion


        #region Sync

        SyncListKeyBool syncListKeyBool = new SyncListKeyBool();
        /*
        SyncListKeyInt syncListKeyInt = new SyncListKeyInt();
        SyncListKeyUInt syncListKeyUInt = new SyncListKeyUInt();
        SyncListKeyFloat syncListKeyFloat = new SyncListKeyFloat();
        SyncListKeyString syncListKeyString = new SyncListKeyString();
        //SyncListKeyColor syncListKeyColor = new SyncListKeyColor();
        SyncListKeyVector2 syncListKeyVector2 = new SyncListKeyVector2();
        SyncListKeyVector3 syncListKeyVector3 = new SyncListKeyVector3();
        SyncListKeyVector4 syncListKeyVector4 = new SyncListKeyVector4();
        SyncListKeyVector2Int syncListKeyVector2Int = new SyncListKeyVector2Int();
        SyncListKeyVector3Int syncListKeyVector3Int = new SyncListKeyVector3Int();
        */

        [SyncVar]
        bool materialPropertyDebugMenuUpdate;

        #endregion

        Dictionary<Type, ISyncListKeyObj> typeToSyncList;
        Dictionary<string, TypeAndIdx> keyToTypeIdx = new Dictionary<string, TypeAndIdx>();

        public List<string> ignoreKeys = new List<string>(); // want use HashSet but use List so it will be serialized on Inspector


        public void Awake()
        {
            typeToSyncList = new Dictionary<Type, ISyncListKeyObj>()
            {
                { typeof(bool),    syncListKeyBool    },
                { typeof(int),     syncListKeyInt     },
                { typeof(uint),    syncListKeyUInt    },
                { typeof(float),   syncListKeyFloat   },
                { typeof(string),  syncListKeyString  },
                //{ typeof(Color),   syncListKeyColor   },
                { typeof(Vector2), syncListKeyVector2 },
                { typeof(Vector3), syncListKeyVector3 },
                { typeof(Vector4), syncListKeyVector4 },
                { typeof(Vector2Int), syncListKeyVector2Int },
                { typeof(Vector3Int), syncListKeyVector3Int },
            };
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.SpawnObjects();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            ReadPrefs(true);
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
            PrefsParam.all.Values
                .Where(prefs => !ignoreKeys.Contains(prefs.key))
                .ToList().ForEach(prefs =>
                {
                    var key = prefs.key;
                    var obj = prefs.GetObject();
                    if (obj != null)
                    {
                        var type = prefs.GetInnerType();
                        if (type.IsEnum)
                        {
                            type = typeof(int);
                            obj = Convert.ToInt32(obj);
                        }

                        if (keyToTypeIdx.TryGetValue(key, out var ti))
                        {
                            var iSynList = typeToSyncList[type];
                            iSynList.Set(ti.idx, obj);
                        }
                        else
                        {
                            Assert.IsTrue(typeToSyncList.ContainsKey(type), string.Format($"type [{type}] is not supported."));

                            var iSynList = typeToSyncList[type];
                            var idx = iSynList.Count;
                            iSynList.Add(key, obj);
                            keyToTypeIdx[key] = new TypeAndIdx() { type = type, idx = idx };
                        }
                    }

                });

            materialPropertyDebugMenuUpdate = MaterialPropertyDebugMenu.update;
        }

        [ClientCallback]
        void ReadPrefs(bool checkAlreadyGet = false)
        {
            // ignore at "Host"
            if (!NetworkServer.active)
            {
                var all = PrefsParam.all;
                typeToSyncList.Values.ToList().ForEach(sl =>
                {
                    for (var i = 0; i < sl.Count; ++i)
                    {
                        var keyObj = sl.Get(i);

                        if (all.TryGetValue(keyObj.key, out var prefs))
                        {
                            prefs.SetSyncedObject(keyObj._value, () =>
                            {
                                Debug.LogWarning($"key:[{prefs.key}] Get() before synced. before:[{prefs.GetObject()}] sync:[{keyObj._value}]");
                            });
                        }
                    }
                });
            }

            MaterialPropertyDebugMenu.update = materialPropertyDebugMenuUpdate;
        }
    }

}
