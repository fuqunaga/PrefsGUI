using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PrefsGUI.Kvs
{
    /// <summary>
    /// Custom File Path for PrefsKvs
    /// Relative to Application.dataPath
    /// you can use magic path
    /// - %dataPath% -> Application.dataPath
    /// - %companyName% -> Application.companyName
    /// - %productName% -> Application.productName
    /// - other %[word]% -> System.Environment.GetEnvironmentVariable([word])
    /// </summary>
    public class PrefsKvsPathCustom : MonoBehaviour, IPrefsKvsPath
    {
        #region Static

        public static ulong MakePlatformMask(params RuntimePlatform[] platforms) => platforms.Aggregate((ulong)0, (mask, platform) => mask | ((ulong)1 << (int)platform));

        public string ReplaceMagicWord(string rawString)
        {
            var ret = rawString
                .Replace("%dataPath%", Application.dataPath)
                .Replace("%companyName%", Application.companyName)
                .Replace("%productName%", Application.productName);

            var matches = Regex.Matches(ret, @"%\w+?%").Cast<Match>();

            return matches.Aggregate(ret, (current, m) => current.Replace(m.Value, Environment.GetEnvironmentVariable(m.Value.Trim('%'))));
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