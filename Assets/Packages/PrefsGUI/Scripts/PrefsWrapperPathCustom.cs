using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;

namespace PrefsGUI.Wrapper
{
    /// <summary>
    /// Custom File Path for PrefsWrapper
    /// Rerative to Application.dataPath
    /// you can use magic path
    /// - %dataPath% -> Application.dataPath
    /// - %companyName% -> Application.companyName
    /// - %productName% -> Application.productName
    /// - other %[word]% -> System.Environment.GetEnvironmentVariable([word])
    /// </summary>
    public class PrefsWrapperPathCustom : MonoBehaviour, IPrefsWrapperPath
    {
        [SerializeField]
        protected string _path = "%dataPath%/../../%productName%Prefs";

        public string path
        {
            get
            {
                var p = _path
                    .Replace("%dataPath%", Application.dataPath)
                    .Replace("%companyName%", Application.companyName)
                    .Replace("%productName%", Application.productName);

                var matches = Regex.Matches(p, @"%\w+?%").Cast<Match>();
                if ( matches.Any())
                {
                    matches.ToList().ForEach(m => p = p.Replace(m.Value, Environment.GetEnvironmentVariable(m.Value.Trim('%'))));
                }


                return p;
            }
        }
    }
}