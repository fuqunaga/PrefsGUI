using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PrefsWrapper
{
    interface IPrefsWrapperPath
    {
        string path { get; }
    }


    public static class PrefsWrapperPathSelector
    {
        static bool first = true;
        static IPrefsWrapperPath _path;

        public static string path
        {
            get
            {
                if (first)
                {
                    first = false;
                    _path = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                        .Select(b => b.GetComponent<IPrefsWrapperPath>())
                        .Where(o => o != null).FirstOrDefault();
                }
                return _path?.path ?? Application.persistentDataPath;
            }
        }

    }
}