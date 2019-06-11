using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PrefsGUI
{
    /// <summary>
    /// JsonUtility exntend.
    /// support primitive type, List and Array
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

                ret = ((ValueWrapper)wrapper).obj;
            }
            else
            {
                ret = JsonUtility.FromJson(json, type);
            }
            
            return ret;
        }

        //public static void FromJsonOverwrite(string json, object objectToOverwrite);
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

        
        
        protected abstract class ValueWrapper
        {
            public static Type GetWrapperType(Type type) => typeof(ValueWrapper<>).MakeGenericType(type);
            
            public static bool IsSupport(Type type) => type.IsPrimitive || (type == typeof(string));

            public abstract object obj { get; }
        }

        protected class ValueWrapper<T> : ValueWrapper
        {
            public T value;
            public override object obj => value;

            public ValueWrapper(T value) { this.value = value; }
        }
    }
}