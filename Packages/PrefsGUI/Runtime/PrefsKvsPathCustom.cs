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
        #region Type Define
        
        /// <summary>
        /// Platform flags
        /// RuntimePlatform is too much, define it myself
        /// https://docs.unity3d.com/ScriptReference/RuntimePlatform.html
        /// </summary>
        [Flags]
        public enum Platform
        {
            OSXEditor      = (1 << 0),
            OSXPlayer      = (1 << 1),
            WindowsPlayer  = (1 << 2),
            WindowsEditor  = (1 << 3),
            IPhonePlayer   = (1 << 4),
            Android        = (1 << 5),
            LinuxPlayer    = (1 << 6),
            LinuxEditor    = (1 << 7),
            WebGLPlayer    = (1 << 8),
            WSAPlayerX86   = (1 << 9),
            WSAPlayerX64   = (1 << 10),
            WSAPlayerARM   = (1 << 11),
            PS4            = (1 << 12),
            XboxOne        = (1 << 13),
            tvOS           = (1 << 14),
            Switch         = (1 << 15),
            Stadia         = (1 << 16),
            CloudRendering = (1 << 17),
            PS5            = (1 << 18),
            LinuxServer    = (1 << 19),
            WindowsServer  = (1 << 20),
            OSXServer      = (1 << 21),
        }
        
        #endregion
        
        
        #region Static

        static bool IsPlatformActive(Platform platform)
        {
            var name = Enum.GetName(typeof(RuntimePlatform), Application.platform);
            Platform current;
            try
            {
                current = Enum.Parse<Platform>(name);
            }
            catch
            {
                return false;
            }

            return platform.HasFlag(current);
        }

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
        public Platform platform = (Platform.WindowsEditor | Platform.WindowsPlayer);        

        [SerializeField]
        protected string _path = "%dataPath%/../../%productName%Prefs";

        protected virtual string rawPath => _path;


        public string path =>
            IsPlatformActive(platform)
                ? ReplaceMagicWord(rawPath)
                : null;
    }
}