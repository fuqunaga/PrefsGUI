using System;
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
        public readonly TPrefs0 prefs0;
        public readonly TPrefs1 prefs1;

        public abstract string[] paramNames { get; }

        protected virtual string GenerateParamKey(string key, string paramName) => key + "_" + paramName;

        public PrefsSet(string key, TOuter0 default0 = default, TOuter1 default1 = default)
        {
            this.key = key;
            prefs0 = Construct<TPrefs0, TOuter0>(key, paramNames[0], default0);
            prefs1 = Construct<TPrefs1, TOuter1>(key, paramNames[1], default1);
        }

        TPrefs Construct<TPrefs, TOuter>(string key, string postfix, TOuter defaultValue)
        {
            var ctor = typeof(TPrefs).GetConstructor(new[] { typeof(string), typeof(TOuter) });
            Assert.IsNotNull(ctor);

            return (TPrefs)ctor.Invoke(new object[] { GenerateParamKey(key, postfix), defaultValue });
        }
    }
}