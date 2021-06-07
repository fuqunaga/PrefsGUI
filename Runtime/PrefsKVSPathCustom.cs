using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PrefsGUI.KVS
{
    /// <summary>
    /// Custom File Path for PrefsKVS
    /// Rerative to Application.dataPath
    /// you can use magic path
    /// - %dataPath% -> Application.dataPath
    /// - %companyName% -> Application.companyName
    /// - %productName% -> Application.productName
    /// - other %[word]% -> System.Environment.GetEnvironmentVariable([word])
    /// </summary>
    public class PrefsKVSPathCustom : MonoBehaviour, IPrefsKVSPath
    {
        #region Static

        public static ulong MakePlatformMask(params RuntimePlatform[] platforms) => platforms.Aggregate((ulong)0, (mask, platform) => mask | ((ulong)1 << (int)platform));

        public string ReplaceMagicWord(string path)
        {
            var ret = path
                .Replace("%dataPath%", Application.dataPath)
                .Replace("%companyName%", Application.companyName)
                .Replace("%productName%", Application.productName);

            var matches = Regex.Matches(ret, @"%\w+?%").Cast<Match>();
            foreach (var m in matches)
            {
                ret = ret.Replace(m.Value, Environment.GetEnvironmentVariable(m.Value.Trim('%')));
            }

            return ret;
        }

        #endregion


        [SerializeField]
        protected ulong platformMask = MakePlatformMask(RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer);


        [SerializeField]
        protected string _path = "%dataPath%/../../%productName%Prefs";

        protected virtual string rawPath => _path;


        public string path
        {
            get
            {
                string ret = null;

                var enable = (platformMask & MakePlatformMask(Application.platform)) != 0;
                if (enable)
                {
                    ret = ReplaceMagicWord(rawPath);
                }

                return ret;
            }
        }
    }
}