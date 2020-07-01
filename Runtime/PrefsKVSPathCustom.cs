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
        [SerializeField]
        protected ulong platformMask = MakePlatformMask(RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer);


        [SerializeField]
        protected string _path = "%dataPath%/../../%productName%Prefs";


        static ulong MakePlatformMask(params RuntimePlatform[] platforms)
        {
            return platforms.Aggregate((ulong)0, (mask, platform) => mask | ((ulong)1 << (int)platform));
        }


        public string path
        {
            get
            {
                var enable = (platformMask & MakePlatformMask(Application.platform)) != 0;
                string ret = null;

                if (enable)
                {
                    ret = _path
                        .Replace("%dataPath%", Application.dataPath)
                        .Replace("%companyName%", Application.companyName)
                        .Replace("%productName%", Application.productName);

                    var matches = Regex.Matches(ret, @"%\w+?%").Cast<Match>();
                    if (matches.Any())
                    {
                        matches.ToList().ForEach(m => ret = ret.Replace(m.Value, Environment.GetEnvironmentVariable(m.Value.Trim('%'))));
                    }
                }

                return ret;
            }
        }
    }
}