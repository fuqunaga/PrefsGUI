using System.Linq;
using UnityEngine;


namespace PrefsGUI.KVS
{
    interface IPrefsKVSPath
    {
        string path { get; }
    }


    public static class PrefsKVSPathSelector
    {
        static bool first = true;
        static IPrefsKVSPath _path;

        public static string path
        {
            get
            {
                if (first)
                {
                    first = false;
                    _path = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                        .Select(b => b.GetComponent<IPrefsKVSPath>())
                        .Where(o => o != null).FirstOrDefault();
                }
                return _path?.path ?? Application.persistentDataPath;
            }
        }

    }
}