using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PrefsGUI.KVS.Json
{
    public class PrefsKVSJson : IKVS
    {
        #region JsonUtility

        [Serializable]
        public struct KeyValue
        {
            public string key;
            public string value;
        }

        public struct ListWrapper
        {
            public List<KeyValue> list;
        }

        #endregion

        string path => PrefsKVSPathSelector.path + "/Prefs.json";


        Dictionary<string, object> cachedObj = new Dictionary<string, object>();
        Dictionary<string, string> jsonDic = new Dictionary<string, string>();

        public PrefsKVSJson()
        {
            Load();
        }


        public void Save()
        {
            cachedObj.ToList().ForEach(pair =>
            {
                var json = JsonUtilityEx.ToJson(pair.Value);
                jsonDic[pair.Key] = json;
            });

            var list = jsonDic.Select(pair => new KeyValue() { key = pair.Key, value = pair.Value }).ToList();
            var str = JsonUtilityEx.ToJson(new ListWrapper() { list = list }, true);

            var p = path;
            var dir = Path.GetDirectoryName(p);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(p, str);
        }

        public void Load()
        {
            if (File.Exists(path))
            {
                var str = File.ReadAllText(path);
                var kvList = JsonUtility.FromJson<ListWrapper>(str).list;
                jsonDic = kvList?.ToDictionary(kv => kv.key, kv => kv.value);

                cachedObj.Clear();
            }
        }


        public bool HasKey(string key) => cachedObj.ContainsKey(key) || jsonDic.ContainsKey(key);

        public void DeleteKey(string key)
        {
            cachedObj.Remove(key);
            jsonDic.Remove(key);
        }

        public void DeleteAll()
        {
            cachedObj.Clear();
            jsonDic.Clear();
        }


        public T Get<T>(string key, T defaultValue)
        {
            object obj;
            if (!cachedObj.TryGetValue(key, out obj))
            {
                if (jsonDic.TryGetValue(key, out var json))
                {
                    obj = JsonUtilityEx.FromJson<T>(json);
                    cachedObj[key] = obj;
                }
            }

            if ( obj != null)
            {
                return (T)obj;
            }

            Set(key, defaultValue);
            return defaultValue;
        }

        public void Set<T>(string key, T v)
        {
            cachedObj[key] = v;
        }
    }

#if false

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

        public static bool HasKey(string key)
        {
            return JSONData.Instance.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            JSONData.Instance.DeleteKey(key);
        }

        public static object Get(string key, object defaultValue)
        {
            if (!HasKey(key)) Set(key, defaultValue);

            var ret = JSONData.Instance.Get(key);
            return (typeof(T).IsEnum ? (T)Enum.Parse(typeof(T), ret.ToString()) : Convert.ChangeType(ret, typeof(T)));
        }

        public static void Set(string key, object val)
        {
            JSONData.Instance.Set(key, val);
        }
    }

#endif

}
