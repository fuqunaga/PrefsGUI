using System.Collections.Generic;
using UnityEngine;
using System;

namespace PrefsWrapperNative
{
    class PlayerPrefsGlobal
    {
        public static void Save() { PlayerPrefs.Save(); }
        public static void Load() { /* always loaded. */}
        public static void DeleteAll() { PlayerPrefs.DeleteAll(); }
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

        static readonly Dictionary<Type, Func<string, object>> GetMethodTable = new Dictionary<Type, Func<string, object>>
            {
                { typeof(int), (key) => PlayerPrefs.GetInt(key) }
                ,{typeof(float),  (key) => PlayerPrefs.GetFloat(key)}
                ,{typeof(string), (key) => PlayerPrefs.GetString(key)}
            };

        static readonly Dictionary<Type, Action<string, object>> SetMethodTable = new Dictionary<Type, Action<string, object>>
            {
                { typeof(int),    (key, o) => PlayerPrefs.SetInt   (key, (int)o) }
                ,{typeof(float),  (key, o) => PlayerPrefs.SetFloat (key, (float)o) }
                ,{typeof(string), (key, o) => PlayerPrefs.SetString(key, (string)o) }
            };

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }

        public static void Load()
        {
            // PlayerPrefs is always loaded
        }

        public static object Get(string key, object defaultValue)
        {
            if (!HasKey(key)) Set(key, defaultValue);

            var ret = GetMethodTable[type](key);
            return (typeof(T).IsEnum ? (T)ret : Convert.ChangeType(ret, typeof(T)));
        }

        public static void Set(string key, object val)
        {
            SetMethodTable[type](key, Convert.ChangeType(val, type));
        }
    }

}
