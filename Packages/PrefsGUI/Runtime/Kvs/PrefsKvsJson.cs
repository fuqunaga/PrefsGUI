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

        
        public Action<string, Type, string, Exception> onJsonParseFailed;
        
        private readonly KvsCache kvsCache = new();
        private Dictionary<string, string> jsonDic = new();
        
        private string path => PrefsKvsPathSelector.path + "/Prefs.json";
        

        public PrefsKvsJson()
        {
            Load();
        }


        public void Save()
        {
            foreach(var (key,obj) in kvsCache)
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

                kvsCache.Clear();
            }
        }


        public bool HasKey(string key) => kvsCache.ContainsKey(key) || jsonDic.ContainsKey(key);

        public void DeleteKey(string key)
        {
            kvsCache.Remove(key);
            jsonDic.Remove(key);
        }

        public void DeleteAll()
        {
            kvsCache.Clear();
            jsonDic.Clear();
        }


        public T Get<T>(string key, T defaultValue)
        {
            if (kvsCache.TryGetValue<T>(key, out var value))
            {
                return value;
            }

            value = defaultValue;
            if (jsonDic.TryGetValue(key, out var json))
            {
                try
                {
                    value = JsonUtilityEx.FromJson<T>(json);
                }
                catch (Exception e)
                {
                    var callback = onJsonParseFailed ?? OnJsonParseFailedDefault;
                    callback(key, typeof(T), json, e);
                }
            }
            
            kvsCache.Set(key, value);

            return value;
        }
        
        public void OnJsonParseFailedDefault(string key, Type type, string json, Exception e)
        {
            Debug.LogError($"Json parse failed. key[{key}] type[{type.Name}] json[{json}]\n Exception[{e}]");
        }

        public void Set<T>(string key, T v)
        {
            kvsCache.Set(key, v);
        }
    }
}
