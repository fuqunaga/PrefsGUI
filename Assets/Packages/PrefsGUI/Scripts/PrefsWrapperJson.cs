using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace PrefsWrapperJson
{
    public class JSONData
    {
        public static JSONData Instance = new JSONData();

        Dictionary<string, object> _dic = new Dictionary<string, object>();

        public JSONData() { Load(); }

        public bool HasKey(string key) { return _dic.ContainsKey(key); }
        public void DeleteKey(string key) { _dic.Remove(key); }

        public object Get(string key) { return _dic[key]; }

        public void Set(string key, object value) { _dic[key] = value; }

        string path { get { return Application.persistentDataPath + "/Prefs.json"; } }

        public void Save()
        {
            var str = MiniJSON.Json.Serialize(_dic);
            File.WriteAllText(path, str);
        }

        public void Load()
        {
            if (File.Exists(path))
            {
                var str = File.ReadAllText(path);
                _dic = (Dictionary<string, object>)MiniJSON.Json.Deserialize(str);
            }
        }

        public void DeleteAll()
        {
            _dic.Clear();
        }
    }

    class PlayerPrefsGlobal
    {
        public static void Save() { JSONData.Instance.Save(); }
        public static void Load() { JSONData.Instance.Load(); }
        public static void DeleteAll() { JSONData.Instance.DeleteAll(); }
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
            JSONData.Instance.Set(key, Convert.ChangeType(val, type));
        }
    }

}
