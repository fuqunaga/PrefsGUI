using System;
using System.Net;

namespace PrefsGUI
{
    [Serializable]
    [Obsolete("PrefsIPEndPoint is obsolete, consider using PrefsIPAddressAndPort instead.")]
    public class PrefsIPEndPoint : PrefsSet<PrefsString, PrefsInt, string, int>
    {
        public string address => prefs0.Get();
        public int port => prefs1.Get();

        public PrefsIPEndPoint(string key, string hostname = "localhost", int port = 10000) : base(key, hostname, port, "address", "port") { }

        public static implicit operator IPEndPoint(PrefsIPEndPoint me) => CreateIPEndPoint(me.address, me.port);

        public static IPEndPoint CreateIPEndPoint(string address, int port) =>
            IPEndPointUtility.CreateIPEndPoint(address, port);

        public static IPAddress FindFromHostName(string hostname) => IPEndPointUtility.FindFromHostName(hostname);
    }

    
    [Serializable]
    public class PrefsIPAddressAndPort : PrefsParam<AddressAndPort>
    {
        public PrefsIPAddressAndPort(string key, string address = "localhost", int port = 10000) : this(key, new AddressAndPort {address = address, port = port})
        {}
            
        public PrefsIPAddressAndPort(string key, AddressAndPort defaultValue) : base(key, defaultValue)
        {
        }

        public static implicit operator IPEndPoint(PrefsIPAddressAndPort me) => IPEndPointUtility.CreateIPEndPoint(me.Get());
    }
}