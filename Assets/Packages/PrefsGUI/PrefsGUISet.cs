using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Assertions;

namespace PrefsGUI
{
    [Serializable]
    public class PrefsIPEndPoint : PrefsSet<PrefsString, PrefsInt, string, int>
    {
        static string[] _paramNames = new[] { "address", "port" };
        protected override string[] paramNames => _paramNames;

        public PrefsIPEndPoint(string key, string hostname = "localhost", int port = 10000) : base(key, hostname, port) { }

        public static implicit operator IPEndPoint(PrefsIPEndPoint me) => CreateIPEndPoint(me.prefs0.Get(), me.prefs1.Get());

        public static IPEndPoint CreateIPEndPoint(string hostname, int port) => new IPEndPoint(FindFromHostName(hostname), port);
        
        public static IPAddress FindFromHostName(string hostname)
        {
            var address = Dns.GetHostAddresses(hostname).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            return address ?? IPAddress.None;
        }
    }



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

        public PrefsSet(string key, Outer0 default0 = default(Outer0), Outer1 default1 = default(Outer1))
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

        public void OnGUI(string label = null)
        {
            using (var h = new GUILayout.HorizontalScope())
            {
                GUIUtil.PrefixLabel(label ?? key);

                using (var v = new GUILayout.VerticalScope())
                {
                    prefs0.OnGUI(paramNames[0]);
                    prefs1.OnGUI(paramNames[1]);
                }
            }
        }
    }
}