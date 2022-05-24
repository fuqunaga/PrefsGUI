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
        static bool first = true;
        static IPrefsKvsPath _path;

        public static string path
        {
            get
            {
                if (first)
                {
                    first = false;
                    _path = Resources
                        .FindObjectsOfTypeAll<MonoBehaviour>()
                        .Select(b => b.GetComponent<IPrefsKvsPath>())
                        .FirstOrDefault(o => o != null);
                }
                return _path?.path ?? Application.persistentDataPath;
            }
        }

    }
}