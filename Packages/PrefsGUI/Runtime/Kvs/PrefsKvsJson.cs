using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrefsGUI.Utility;
using UnityEngine;

namespace PrefsGUI.Kvs.Json
{
    public class PrefsKvsJson : IKvs
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

        
        private readonly KvsCache ksvCache = new();
        private Dictionary<string, string> jsonDic = new();
        
        private string path => PrefsKvsPathSelector.path + "/Prefs.json";
        

        public PrefsKvsJson()
        {
            Load();
        }


        public void Save()
        {
            foreach(var (key,obj) in ksvCache)
            {
                var json = JsonUtilityEx.ToJson(obj);
                jsonDic[key] = json;
            }

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

                ksvCache.Clear();
            }
        }


        public bool HasKey(string key) => ksvCache.ContainsKey(key) || jsonDic.ContainsKey(key);

        public void DeleteKey(string key)
        {
            ksvCache.Remove(key);
            jsonDic.Remove(key);
        }

        public void DeleteAll()
        {
            ksvCache.Clear();
            jsonDic.Clear();
        }


        public T Get<T>(string key, T defaultValue)
        {
            if (!ksvCache.TryGetValue<T>(key, out var value))
            {
                value = jsonDic.TryGetValue(key, out var json)
                    ? JsonUtilityEx.FromJson<T>(json)
                    : defaultValue;

                ksvCache.Set(key, value);
            }
            
            return value;
        }

        public void Set<T>(string key, T v)
        {
            ksvCache.Set(key, v);
        }
    }
}
