using System;
using System.Collections.Generic;
using UnityEngine;


namespace PrefsGUI.Utility
{
    /// <summary>
    /// JsonUtility extension to convert any type.
    /// The original JsonUtility converts public fields of objects to Json,
    /// so primitive types, etc. are not supported.
    /// </summary>
    public static class JsonUtilityEx
    {
        public static T FromJson<T>(string json) => (T)FromJson(json, typeof(T));

        public static object FromJson(string json, Type type)
        {
            object ret = null;

            if (ValueWrapper.NeedWrap(type))
            {
                var wrapperType = ValueWrapper.GetWrapperType(type);
                var wrapper = JsonUtility.FromJson(json, wrapperType);

                ret = (wrapper as ValueWrapper)?.obj;
            }
            else
            {
                ret = JsonUtility.FromJson(json, type);
            }
            
            return ret;
        }

        public static string ToJson(object obj, bool prettyPrint = false)
        {
            return JsonUtility.ToJson(WrapObject(obj), prettyPrint);
        }

        static object WrapObject(object obj)
        {
            if ( obj != null)
            {
                var type = obj.GetType();
                if (ValueWrapper.NeedWrap(type))
                {   
                    obj = Activator.CreateInstance(ValueWrapper.GetWrapperType(type), obj);
                }
            }

            return obj;
        }


        internal abstract class ValueWrapper
        {
            private static readonly HashSet<Type> needWrapTypes = new()
            {
                typeof(string),
                typeof(Vector2Int), typeof(Vector3Int),
                typeof(Rect), typeof(RectOffset),
                typeof(Bounds), typeof(BoundsInt)
            };
            
            private static readonly Dictionary<Type, bool> needWrapTable = new();


            public static Type GetWrapperType(Type type) => typeof(ValueWrapper<>).MakeGenericType(type);
            
            public static bool NeedWrap(Type type)
            {
                if (!needWrapTable.TryGetValue(type, out var ret))
                {
                    ret = type.IsPrimitive
                          || needWrapTypes.Contains(type)
                          || type.IsArray
                          || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));

                    needWrapTable[type] = ret;
                }

                return ret;
            }

            public abstract object obj { get; }
        }

        private class ValueWrapper<T> : ValueWrapper
        {
            // [public/not readonly] to be target to JsonUtility serialization
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            public T value;
            public override object obj => value;

            public ValueWrapper(T value) { this.value = value; }
        }
    }
}