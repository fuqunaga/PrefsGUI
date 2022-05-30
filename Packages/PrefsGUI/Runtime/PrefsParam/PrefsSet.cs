using System.Diagnostics;
using UnityEngine.Assertions;

namespace PrefsGUI
{
    /// <summary>
    /// Combination of PrefsParams
    /// </summary>
    public abstract class PrefsSet<TPrefs0, TPrefs1, TOuter0, TOuter1>
        where TPrefs0 : PrefsParamOuter<TOuter0>
        where TPrefs1 : PrefsParamOuter<TOuter1>
    {
        public readonly string key;
        public readonly string[] paramNames;
        public readonly TPrefs0 prefs0;
        public readonly TPrefs1 prefs1;
        
        protected virtual string GenerateParamKey(string keyString, string paramName) => $"{keyString}_{paramName}";

        protected PrefsSet(string key, TOuter0 default0, TOuter1 default1, string paramName0, string paramName1)
        {
            this.key = key;
            prefs0 = Construct<TPrefs0, TOuter0>(key, paramName0, default0);
            prefs1 = Construct<TPrefs1, TOuter1>(key, paramName1, default1);

            paramNames = new[] {paramName0, paramName1};
        }

        TPrefs Construct<TPrefs, TOuter>(string keyString, string postfix, TOuter defaultValue)
        {
            var ctor = typeof(TPrefs).GetConstructor(new[] { typeof(string), typeof(TOuter) });
            Assert.IsNotNull(ctor);

            return (TPrefs)ctor.Invoke(new object[] { GenerateParamKey(keyString, postfix), defaultValue });
        }
    }
}