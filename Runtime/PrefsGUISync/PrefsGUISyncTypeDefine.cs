using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

#pragma warning disable 0618  

namespace PrefsGUI
{
    public partial class PrefsGUISync : NetworkBehaviour
    {
        public interface ISyncListKeyObj
        {
            int Count { get; }
        }

        public struct KeyBool { public string key; public bool value; }
        public struct KeyInt { public string key; public int value; }
        public struct KeyUInt { public string key; public uint value; }
        public struct KeyFloat { public string key; public float value; }
        public struct KeyString { public string key; public string value; }
        public struct KeyColor { public string key; public Color value; }
        public struct KeyVector2 { public string key; public Vector2 value; }
        public struct KeyVector3 { public string key; public Vector3 value; }
        public struct KeyVector4 { public string key; public Vector4 value; }
        public struct KeyVector2Int { public string key; public Vector2Int_ value; } // UNET don't support Vector2Int
        public struct KeyVector3Int { public string key; public Vector3Int_ value; }
        public struct KeyRect { public string key; public Rect value; }
        public struct KeyBounds { public string key; public Bounds_ value; }
        public struct KeyBoundsInt { public string key; public BoundsInt_ value; }


        public class SyncListKeyBool : SyncListStruct<KeyBool>, ISyncListKeyObj { }
        public class SyncListKeyInt : SyncListStruct<KeyInt>, ISyncListKeyObj { }
        public class SyncListKeyUInt : SyncListStruct<KeyUInt>, ISyncListKeyObj { }
        public class SyncListKeyFloat : SyncListStruct<KeyFloat>, ISyncListKeyObj { }
        public class SyncListKeyString : SyncListStruct<KeyString>, ISyncListKeyObj { }
        public class SyncListKeyColor : SyncListStruct<KeyColor>, ISyncListKeyObj { }
        public class SyncListKeyVector2 : SyncListStruct<KeyVector2>, ISyncListKeyObj { }
        public class SyncListKeyVector3 : SyncListStruct<KeyVector3>, ISyncListKeyObj { }
        public class SyncListKeyVector4 : SyncListStruct<KeyVector4>, ISyncListKeyObj { }
        public class SyncListKeyVector2Int : SyncListStruct<KeyVector2Int>, ISyncListKeyObj { }
        public class SyncListKeyVector3Int : SyncListStruct<KeyVector3Int>, ISyncListKeyObj { }
        public class SyncListKeyRect : SyncListStruct<KeyRect>, ISyncListKeyObj { }
        public class SyncListKeyBounds : SyncListStruct<KeyBounds>, ISyncListKeyObj { }
        public class SyncListKeyBoundsInt : SyncListStruct<KeyBoundsInt>, ISyncListKeyObj { }


        public struct Vector2Int_
        {
            public int x, y;

            static public Vector2Int_ ConvertFrom(Vector2Int v) => new Vector2Int_() { x = v.x, y = v.y, };
            public Vector2Int ConvertTo() => new Vector2Int(x, y);
        }

        public struct Vector3Int_
        {
            public int x, y, z;

            static public Vector3Int_ ConvertFrom(Vector3Int v) => new Vector3Int_() { x = v.x, y = v.y, z = v.z };
            public Vector3Int ConvertTo() => new Vector3Int(x, y, z);
        }

        public struct Bounds_
        {
            public Vector3 center, size;


            static public Bounds_ ConvertFrom(Bounds v) => new Bounds_() { center = v.center, size = v.size };
            public Bounds ConvertTo() => new Bounds(center, size);
        }

        public struct BoundsInt_
        {
            public Vector3Int_ position, size;

            static public BoundsInt_ ConvertFrom(BoundsInt v) => new BoundsInt_() { position = Vector3Int_.ConvertFrom(v.position), size = Vector3Int_.ConvertFrom(v.size) };
            public BoundsInt ConvertTo() => new BoundsInt(position.ConvertTo(), size.ConvertTo());
        }
    }



    public static class ISyncListKeyObjExtenion
    {
        static object CreateInstance(string key, object obj, Type keyObjeType)
        {
            var ret = Activator.CreateInstance(keyObjeType);
            var kvField = GetField(keyObjeType);

            kvField.keyField.SetValue(ret, key);

            obj = kvField.valueConvertFrom?.Invoke(null, new[] { obj }) ?? obj;
            kvField.valueField.SetValue(ret, obj);

            return ret;
        }



        public static void Set(this PrefsGUISync.ISyncListKeyObj me, int idx, object obj)
        {
            var listObj = new ListObj(me);

            var keyValue = new KVObj(listObj[idx]);
            if (false == keyValue.value.Equals(obj))
            {
                listObj[idx] = CreateInstance(keyValue.key, obj, listObj.elementType);
            }
        }

        public static void Add(this PrefsGUISync.ISyncListKeyObj me, string key, object obj)
        {
            var listObj = new ListObj(me);
            listObj.Add(CreateInstance(key, obj, listObj.elementType));
        }

        public static (string, object) Get(this PrefsGUISync.ISyncListKeyObj me, int idx)
        {
            var listObj = new ListObj(me);
            var kvObj = new KVObj(listObj[idx]);

            return (kvObj.key, kvObj.value);
        }



        #region Reflection Utility

        public class KVField
        {
            public FieldInfo keyField;
            public FieldInfo valueField;
            public MethodInfo valueConvertFrom;
            public MethodInfo valueConvertTo;
        }

        public class ListInfo
        {
            public Type elementType;
            public PropertyInfo indexer;
            public MethodInfo add;
        }

        protected class KVObj
        {
            object obj;
            KVField info;

            public KVObj(object obj)
            {
                this.obj = obj;
                info = GetField(obj.GetType());
            }

            public string key
            {
                get { return (string)info.keyField.GetValue(obj); }
                set { info.keyField.SetValue(obj, value); }
            }
            public object value
            {
                get
                {
                    var ret = info.valueField.GetValue(obj);
                    return info.valueConvertTo?.Invoke(ret, null) ?? ret;
                }
            }
        }

        protected class ListObj
        {
            object obj;
            ListInfo info;
            public Type elementType => info.elementType;

            public ListObj(object obj)
            {
                this.obj = obj;
                info = GetListInfo(obj.GetType());
            }

            public object this[int idx]
            {
                get => info.indexer.GetValue(obj, new object[] { idx });
                set => info.indexer.SetValue(obj, value, new object[] { idx });
            }

            public void Add(object value)
            {
                info.add.Invoke(obj, new[] { value });
            }
        }


        static Dictionary<Type, KVField> typeToField = new Dictionary<Type, KVField>();
        static Dictionary<Type, ListInfo> typeToListInfo = new Dictionary<Type, ListInfo>();

        static KVField GetField(Type type)
        {
            if (!typeToField.TryGetValue(type, out var kvField))
            {
                var valueField = type.GetField("value");
                var valueType = valueField.FieldType;
                typeToField[type] = kvField = new KVField()
                {
                    keyField = type.GetField("key"),
                    valueField = valueField,
                    valueConvertFrom = valueType.GetMethod("ConvertFrom", BindingFlags.Public | BindingFlags.Static),
                    valueConvertTo = valueType.GetMethod("ConvertTo"),
                };
            }
            return kvField;
        }

        static ListInfo GetListInfo(Type type)
        {
            if (!typeToListInfo.TryGetValue(type, out var info))
            {
                Assert.IsTrue(type.BaseType.IsGenericType);

                typeToListInfo[type] = info = new ListInfo()
                {
                    elementType = type.BaseType.GetGenericArguments()[0],
                    indexer = type.GetProperty("Item"),
                    add = type.GetMethod("Add")
                };
            }

            return info;
        }

        #endregion
    }
}
