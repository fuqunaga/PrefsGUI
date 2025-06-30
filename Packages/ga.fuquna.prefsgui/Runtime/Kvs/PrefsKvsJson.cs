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
        
        private static string Path => System.IO.Path.Combine(PrefsKvsPathSelector.Path,
            string.IsNullOrEmpty(PrefsArguments.FileName)
                ? "Prefs.json"
                : PrefsArguments.FileName);
        
        
        public event Action<string, Type, string, Exception> onJsonParseFailed;
        
        private readonly KvsCache _kvsCache = new();
        private Dictionary<string, string> _jsonDic = new();


        public PrefsKvsJson()
        {
            Load();
        }


        public void Save()
        {
            foreach(var (key,obj) in _kvsCache)
            {
                var json = JsonUtilityEx.ToJson(obj);
                _jsonDic[key] = json;
            }

            var list = _jsonDic.Select(pair => new KeyValue() { key = pair.Key, value = pair.Value }).ToList();
            var str = JsonUtilityEx.ToJson(new ListWrapper() { list = list }, true);

            var p = Path;
            var dir = System.IO.Path.GetDirectoryName(p);
            if (dir == null)
            {
                Debug.LogWarning($"PrefsKvsJson: Directory is null for path [{p}]");
                return;
            }
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(p, str);
        }

        public void Load()
        {
            if (File.Exists(Path))
            {
                var str = File.ReadAllText(Path);
                var kvList = JsonUtility.FromJson<ListWrapper>(str).list;
                _jsonDic = kvList?.ToDictionary(kv => kv.key, kv => kv.value);

                _kvsCache.Clear();
            }
        }


        public bool HasKey(string key) => _kvsCache.ContainsKey(key) || _jsonDic.ContainsKey(key);

        public void DeleteKey(string key)
        {
            _kvsCache.Remove(key);
            _jsonDic.Remove(key);
        }

        public void DeleteAll()
        {
            _kvsCache.Clear();
            _jsonDic.Clear();
        }


        public T Get<T>(string key, T defaultValue)
        {
            if (_kvsCache.TryGetValue<T>(key, out var value))
            {
                return value;
            }

            value = defaultValue;
            if (_jsonDic.TryGetValue(key, out var json))
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
            
            _kvsCache.Set(key, value);

            return value;
        }
        
        public void OnJsonParseFailedDefault(string key, Type type, string json, Exception e)
        {
            Debug.LogError($"Json parse failed. key[{key}] type[{type.Name}] json[{json}]\n Exception[{e}]");
        }

        public void Set<T>(string key, T v)
        {
            _kvsCache.Set(key, v);
        }
    }
}
