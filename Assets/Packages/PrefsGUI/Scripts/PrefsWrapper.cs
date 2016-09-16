using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace PrefsWrapper
{

    public static class PlayerPrefs<T>
    {
        #region mthodSet Impelment
        class MethodSet
        {
            public Func<string, bool> HasKey;
            public Action<string> DeleteKey;
            public Func<string, object, object> Get;
            public Action<string, object> Set;
        }

        static readonly Dictionary<Type, MethodSet> _typeMethodSetTable = new Dictionary<Type, MethodSet>()
        {
            {typeof(Vector2), new MethodSet { HasKey = PlayerPrefsVector<Vector2>.HasKey, DeleteKey = PlayerPrefsVector<Vector2>.DeleteKey, Get = PlayerPrefsVector<Vector2>.Get,  Set = PlayerPrefsVector<Vector2>.Set } },
            {typeof(Vector3), new MethodSet { HasKey = PlayerPrefsVector<Vector3>.HasKey, DeleteKey = PlayerPrefsVector<Vector3>.DeleteKey, Get = PlayerPrefsVector<Vector3>.Get,  Set = PlayerPrefsVector<Vector3>.Set } },
            {typeof(Vector4), new MethodSet { HasKey = PlayerPrefsVector<Vector4>.HasKey, DeleteKey = PlayerPrefsVector<Vector4>.DeleteKey, Get = PlayerPrefsVector<Vector4>.Get,  Set = PlayerPrefsVector<Vector4>.Set } },
        };

        static readonly MethodSet _methodSetStandard = new MethodSet()
        {
            HasKey = PlayerPrefsStrandard<T>.HasKey,
            DeleteKey = PlayerPrefsStrandard<T>.DeleteKey,
            Get = PlayerPrefsStrandard<T>.Get,
            Set = PlayerPrefsStrandard<T>.Set,
        };

        static MethodSet methodSet
        {
            get
            {
                MethodSet ret = null;
                if (!_typeMethodSetTable.TryGetValue(typeof(T), out ret))
                {
                    ret = _methodSetStandard;
                }
                return ret;
            }
        }
        #endregion

        public static bool HasKey(string key) { return methodSet.HasKey(key); }
        public static void DeleteKey(string key) { methodSet.DeleteKey(key); }
        public static T Get(string key, T defaultValue = default(T)) { return (T)methodSet.Get(key, defaultValue); }
        public static void Set(string key, T v) { methodSet.Set(key, v); }
    }



    class PlayerPrefsStrandard<T>
    {
        static Type type
        {
            get
            {
                return (typeof(T) == typeof(bool) || typeof(T).IsEnum)
                    ? typeof(int)
                    : typeof(T);
            }
        }

        static readonly Dictionary<Type, string> GetMethodTable = new Dictionary<Type, string>
            {
                { typeof(int), "GetInt"}
                ,{typeof(float), "GetFloat"}
                ,{typeof(string), "GetString"}
            };

        static readonly Dictionary<Type, string> SetMethodTable = new Dictionary<Type, string>
            {
                { typeof(int), "SetInt"}
                ,{typeof(float), "SetFloat"}
                ,{typeof(string), "SetString"}
            };

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public static object Get(string key, object defaultValue)
        {
            if (!HasKey(key)) Set(key, defaultValue);

            MethodInfo info = typeof(PlayerPrefs).GetMethod(GetMethodTable[type], new[] { typeof(string) });
            return typeof(T).IsEnum
                ? (T)info.Invoke(null, new[] { key })
                : (T)Convert.ChangeType(info.Invoke(null, new[] { key }), typeof(T));
        }

        public static void Set(string key, object val)
        {
            MethodInfo info = typeof(PlayerPrefs).GetMethod(SetMethodTable[type], new[] { typeof(string), type });
            info.Invoke(null, new object[] { key, Convert.ChangeType(val, type) });
        }
    }


    class PlayerPrefsVector<T>
    {
        static List<string> GenerateKeys(string key)
        {
            var num = AbstractVector.GetElementNum<T>();
            return Enumerable.Range(0, num).Select((i) => key + "_" + i + "_PlayerPrefsVector").ToList();
        }

        public static bool HasKey(string key)
        {
            return GenerateKeys(key).All(keyAtElement => PlayerPrefsStrandard<float>.HasKey(keyAtElement));
        }

        public static void DeleteKey(string key)
        {
            GenerateKeys(key).ForEach((k) => PlayerPrefsStrandard<float>.DeleteKey(k));
        }

        public static object Get(string key, object defaultValue)
        {
            object ret = default(T);

            var keys = GenerateKeys(key);
            for (var i = 0; i < keys.Count; ++i)
            {
                var elem = (float)PlayerPrefsStrandard<float>.Get(keys[i], AbstractVector.GetAtIdx<T>(defaultValue, i));
                AbstractVector.SetAtIdx<T>(ret, i, elem);
            }
            return ret;
        }

        public static void Set(string key, object val)
        {
            var keys = GenerateKeys(key);
            for (var i = 0; i < keys.Count; ++i)
            {
                PlayerPrefsStrandard<float>.Set(keys[i], AbstractVector.GetAtIdx<T>(val, i));
            }
        }
    }
}
