using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PrefsGUI.Kvs
{
    public interface IPrefsKvsPath
    {
        string path { get; }
    }


    public static class PrefsKvsPathSelector
    {
        private static bool _first = true;
        private static string _path;

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

#if UNITY_EDITOR
        static PrefsKvsPathSelector()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                switch (state)
                {
                    case PlayModeStateChange.ExitingEditMode:
                    case PlayModeStateChange.ExitingPlayMode:
                        _first = true;
                        break;

                    case PlayModeStateChange.EnteredEditMode:
                    case PlayModeStateChange.EnteredPlayMode:
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            };
        }
#endif
    }
}