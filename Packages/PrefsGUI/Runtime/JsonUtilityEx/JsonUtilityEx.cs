using System;
using System.Collections.Generic;
using UnityEngine;


namespace PrefsGUI
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

            if (ValueWrapper.IsSupport(type))
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

        
        public static string ToJson(object obj) => ToJson(obj, false);

        public static string ToJson(object obj, bool prettyPrint)
        {
            return JsonUtility.ToJson(WrapObject(obj), prettyPrint);
        }

        static object WrapObject(object obj)
        {
            if ( obj != null)
            {
                var type = obj.GetType();
                if (ValueWrapper.IsSupport(type))
                {   
                    obj = Activator.CreateInstance(ValueWrapper.GetWrapperType(type), obj);
                }
            }

            return obj;
        }


        internal abstract class ValueWrapper
        {
            public static Type GetWrapperType(Type type) => typeof(ValueWrapper<>).MakeGenericType(type);


            static HashSet<Type> supportedTypes = new HashSet<Type>()
            {
                typeof(string),
                typeof(Vector2Int), typeof(Vector3Int),
                typeof(Rect), typeof(RectOffset),
                typeof(Bounds), typeof(BoundsInt)
            };

            public static bool IsSupport(Type type)
            {
                return type.IsPrimitive
                    || supportedTypes.Contains(type)
                    || type.IsArray
                    || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                    ;
            }

            public abstract object obj { get; }
        }

        internal class ValueWrapper<T> : ValueWrapper
        {
            public T value;
            public override object obj => value;

            public ValueWrapper(T value) { this.value = value; }
        }
    }
}