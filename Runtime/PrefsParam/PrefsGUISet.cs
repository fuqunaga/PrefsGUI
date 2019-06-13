using RapidGUI;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace PrefsGUI
{



    /// <summary>
    /// Combination of PrefsParams
    /// </summary>
    public abstract class PrefsSet<Prefs0, Prefs1, Outer0, Outer1>
        where Prefs0 : PrefsParamOuter<Outer0>
        where Prefs1 : PrefsParamOuter<Outer1>
    {
        protected string key;
        public Prefs0 prefs0;
        public Prefs1 prefs1;

        protected abstract string[] paramNames { get; }

        protected virtual string GenerateParamKey(string key, string paramName) => key + "_" + paramName;

        public PrefsSet(string key, Outer0 default0 = default, Outer1 default1 = default)
        {
            this.key = key;
            prefs0 = Construct<Prefs0, Outer0>(key, paramNames[0], default0);
            prefs1 = Construct<Prefs1, Outer1>(key, paramNames[1], default1);
        }

        T Construct<T, U>(string key, string postfix, U defaultValue)
        {
            var ctor = typeof(T).GetConstructor(new Type[] { typeof(string), typeof(U) });
            Assert.IsNotNull(ctor);

            return (T)ctor.Invoke(new object[] { GenerateParamKey(key, postfix), defaultValue });
        }

        public void DoGUI(string label = null)
        {
            using (new GUILayout.HorizontalScope())
            {
                RGUI.PrefixLabel(label ?? key);

                using ( new GUILayout.VerticalScope())
                {
                    prefs0.DoGUI(paramNames[0]);
                    prefs1.DoGUI(paramNames[1]);
                }
            }
        }
    }
}