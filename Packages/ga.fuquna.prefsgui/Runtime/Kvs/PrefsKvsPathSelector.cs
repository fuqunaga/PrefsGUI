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
        [Obsolete("Use IPrefsKvsPath.Path instead.")]
        string path => Path;
        string Path { get; }
    }


    public static class PrefsKvsPathSelector
    {
        private static bool _first = true;
        private static string _path;

        [Obsolete("Use PrefsKvsPathSelector.Path instead.")]
        public static string path => Path;
        public static string Path
        {
            get
            {
                if (_first)
                {
                    _first = false;
                    if (!string.IsNullOrEmpty(PrefsArguments.FolderPath))
                    {
                        _path = PrefsArguments.FolderPath;
                    }
                    else
                    {
                        _path = Resources
                            .FindObjectsOfTypeAll<GameObject>()
                            .SelectMany(go => go.GetComponents<IPrefsKvsPath>().Select(kvsPath => kvsPath.Path))
                            .FirstOrDefault(str => str != null);
                    }
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