using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PrefsGUI
{
    public partial class PrefsGUISync : NetworkBehaviour
    {
        public struct KeyObj { public string key; public object _value; }


        public struct KeyBool { public string key; public bool _value; }
        public struct KeyInt { public string key; public int _value; }
        public struct KeyUInt { public string key; public uint _value; }
        public struct KeyFloat { public string key; public float _value; }
        public struct KeyString { public string key; public string _value; }
        public struct KeyVector2 { public string key; public Vector2 _value; }
        public struct KeyVector3 { public string key; public Vector3 _value; }
        public struct KeyVector4 { public string key; public Vector4 _value; }
        public struct KeyVector2Int { public string key; public int _x; public int _y; } // UNET don't support Vector2Int
        public struct KeyVector3Int { public string key; public int _x; public int _y; public int _z; }


        public interface ISyncListKeyObj
        {
            int Count { get; }
            void Add(string key, object obj);
            void Set(int idx, object obj);
            KeyObj Get(int idx);
        }

        public class SyncListKeyBool : SyncListStruct<KeyBool>, ISyncListKeyObj { public void Add(string key, object obj) { this._Add(key, obj); } public KeyObj Get(int idx) { return this._Get(idx); } public void Set(int idx, object obj) { this._Set(idx, obj); } }
        public class SyncListKeyInt : SyncListStruct<KeyInt>, ISyncListKeyObj { public void Add(string key, object obj) { this._Add(key, obj); } public KeyObj Get(int idx) { return this._Get(idx); } public void Set(int idx, object obj) { this._Set(idx, obj); } }
        public class SyncListKeyUInt : SyncListStruct<KeyUInt>, ISyncListKeyObj { public void Add(string key, object obj) { this._Add(key, obj); } public KeyObj Get(int idx) { return this._Get(idx); } public void Set(int idx, object obj) { this._Set(idx, obj); } }
        public class SyncListKeyFloat : SyncListStruct<KeyFloat>, ISyncListKeyObj { public void Add(string key, object obj) { this._Add(key, obj); } public KeyObj Get(int idx) { return this._Get(idx); } public void Set(int idx, object obj) { this._Set(idx, obj); } }
        public class SyncListKeyString : SyncListStruct<KeyString>, ISyncListKeyObj { public void Add(string key, object obj) { this._Add(key, obj); } public KeyObj Get(int idx) { return this._Get(idx); } public void Set(int idx, object obj) { this._Set(idx, obj); } }
        public class SyncListKeyVector2 : SyncListStruct<KeyVector2>, ISyncListKeyObj { public void Add(string key, object obj) { this._Add(key, obj); } public KeyObj Get(int idx) { return this._Get(idx); } public void Set(int idx, object obj) { this._Set(idx, obj); } }
        public class SyncListKeyVector3 : SyncListStruct<KeyVector3>, ISyncListKeyObj { public void Add(string key, object obj) { this._Add(key, obj); } public KeyObj Get(int idx) { return this._Get(idx); } public void Set(int idx, object obj) { this._Set(idx, obj); } }
        public class SyncListKeyVector4 : SyncListStruct<KeyVector4>, ISyncListKeyObj { public void Add(string key, object obj) { this._Add(key, obj); } public KeyObj Get(int idx) { return this._Get(idx); } public void Set(int idx, object obj) { this._Set(idx, obj); } }
        public class SyncListKeyVector2Int : SyncListStruct<KeyVector2Int>, ISyncListKeyObj
        {
            public void Add(string key, object obj)
            {
                var val = (Vector2Int)obj;
                Add(new KeyVector2Int()
                {
                    key = key,
                    _x = val.x,
                    _y = val.y,
                });
            }
            public KeyObj Get(int idx)
            {
                var val = this[idx];
                return new KeyObj()
                {
                    key = val.key,
                    _value = new Vector2Int(val._x, val._y)
                };
            }

            public void Set(int idx, object obj)
            {
                var vec = (Vector2Int)obj;
                var me = this[idx];
                if (vec != new Vector2Int(me._x, me._y))
                {
                    this[idx] = new KeyVector2Int()
                    {
                        key = me.key,
                        _x = vec.x,
                        _y = vec.y
                    };
                }
            }
        }

        public class SyncListKeyVector3Int : SyncListStruct<KeyVector3Int>, ISyncListKeyObj
        {
            public void Add(string key, object obj)
            {
                var val = (Vector3Int)obj;
                Add(new KeyVector3Int()
                {
                    key = key,
                    _x = val.x,
                    _y = val.y,
                    _z = val.z
                });
            }

            public KeyObj Get(int idx)
            {
                var val = this[idx];
                return new KeyObj()
                {
                    key = val.key,
                    _value = new Vector3Int(val._x, val._y, val._z)
                };
            }

            public void Set(int idx, object obj)
            {
                var vec = (Vector3Int)obj;
                var me = this[idx];
                if (vec != new Vector3Int(me._x, me._y, me._z))
                {
                    this[idx] = new KeyVector3Int()
                    {
                        key = me.key,
                        _x = vec.x,
                        _y = vec.y,
                        _z = vec.z
                    };
                }
            }

        }
    }
}
