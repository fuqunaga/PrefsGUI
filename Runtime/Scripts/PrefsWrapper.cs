using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
//using PrefsWrapperNative;
using PrefsGUI.Wrapper.Json;

namespace PrefsGUI.Wrapper
{
    public static class PrefsGlobal
    {
        public static void Save() { PlayerPrefsGlobal.Save(); }
        public static void Load() { PlayerPrefsGlobal.Load(); }
        public static void DeleteAll() { PlayerPrefsGlobal.DeleteAll(); }
    }


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
            /*
            {typeof(Vector2), new MethodSet { HasKey = PlayerPrefsVector<Vector2, float>.HasKey, DeleteKey = PlayerPrefsVector<Vector2, float>.DeleteKey, Get = PlayerPrefsVector<Vector2, float>.Get,  Set = PlayerPrefsVector<Vector2, float>.Set } },
            {typeof(Vector3), new MethodSet { HasKey = PlayerPrefsVector<Vector3, float>.HasKey, DeleteKey = PlayerPrefsVector<Vector3, float>.DeleteKey, Get = PlayerPrefsVector<Vector3, float>.Get,  Set = PlayerPrefsVector<Vector3, float>.Set } },
            {typeof(Vector4), new MethodSet { HasKey = PlayerPrefsVector<Vector4, float>.HasKey, DeleteKey = PlayerPrefsVector<Vector4, float>.DeleteKey, Get = PlayerPrefsVector<Vector4, float>.Get,  Set = PlayerPrefsVector<Vector4, float>.Set } },
            {typeof(Vector2Int), new MethodSet { HasKey = PlayerPrefsVector<Vector2Int, int>.HasKey, DeleteKey = PlayerPrefsVector<Vector2Int, int>.DeleteKey, Get = PlayerPrefsVector<Vector2Int, int>.Get,  Set = PlayerPrefsVector<Vector2Int, int>.Set } },
            {typeof(Vector3Int), new MethodSet { HasKey = PlayerPrefsVector<Vector3Int, int>.HasKey, DeleteKey = PlayerPrefsVector<Vector3Int, int>.DeleteKey, Get = PlayerPrefsVector<Vector3Int, int>.Get,  Set = PlayerPrefsVector<Vector3Int, int>.Set } },
            */
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


    /*
    class PlayerPrefsVector<T, ElemType>
    {
        static Dictionary<string, List<string>> _keyCache = new Dictionary<string, List<string>>();

        static List<string> GenerateKeys(string key)
        {
            List<string> ret;
            if (!_keyCache.TryGetValue(key, out ret))
            {
                var num = AbstractVector.GetElementNum<T>();
                ret = Enumerable.Range(0, num).Select((i) => key + "_" + i + "_PlayerPrefsVector").ToList();
                _keyCache[key] = ret;
            }

            return ret;
        }

        public static bool HasKey(string key)
        {
            return GenerateKeys(key).All(keyAtElement => PlayerPrefsStrandard<ElemType>.HasKey(keyAtElement));
        }

        public static void DeleteKey(string key)
        {
            GenerateKeys(key).ForEach((k) => PlayerPrefsStrandard<ElemType>.DeleteKey(k));
        }

        public static object Get(string key, object defaultValue)
        {
            object ret = default(T);
            var defaultVec = new AbstractVector(defaultValue);
            var retVec = new AbstractVector(ret);

            var keys = GenerateKeys(key);
            for (var i = 0; i < keys.Count; ++i)
            {
                var elem = (ElemType)PlayerPrefsStrandard<ElemType>.Get(keys[i], defaultVec[i]);
                retVec[i] = elem;
            }
            return ret;
        }

        public static void Set(string key, object val)
        {
            var vec = new AbstractVector(val);

            var keys = GenerateKeys(key);
            for (var i = 0; i < keys.Count; ++i)
            {
                PlayerPrefsStrandard<ElemType>.Set(keys[i], vec[i]);
            }
        }
    }
    */
}
