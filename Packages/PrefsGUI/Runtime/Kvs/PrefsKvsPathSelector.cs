using System.Linq;
using UnityEngine;

namespace PrefsGUI.Kvs
{
    public interface IPrefsKvsPath
    {
        string path { get; }
    }


    public static class PrefsKvsPathSelector
    {
        static bool _first = true;
        static string _path;

        public static string path
        {
            get
            {
                if (_first)
                {
                    _first = false;
                    _path = Resources
                        .FindObjectsOfTypeAll<GameObject>()
                        .SelectMany(go => go.GetComponents<IPrefsKvsPath>().Select(kvsPath => kvsPath.path))
                        .FirstOrDefault(str => str != null);
                }
                return _path ?? Application.persistentDataPath;
            }
        }
    }
}