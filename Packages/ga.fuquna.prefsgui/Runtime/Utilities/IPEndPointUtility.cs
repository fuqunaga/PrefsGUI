using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace PrefsGUI
{
    [Serializable]
    public struct AddressAndPort
    {
        public string address;
        public int port;
            
        public void Deconstruct(out string a, out int p)
        {
            a = address;
            p = port;
        }
    }

    
    public static class IPEndPointUtility
    {
        public static IPEndPoint CreateIPEndPoint(AddressAndPort addressAndPort)
        {
            var (address, port) = addressAndPort;
            return CreateIPEndPoint(address, port);
        }
        
        public static IPEndPoint CreateIPEndPoint(string address, int port)
        {
            var ip = FindFromHostName(address);
            return (!Equals(ip, IPAddress.None)) ? new IPEndPoint(ip, port) : null;
        }

        public static IPAddress FindFromHostName(string hostname)
        {
            var address = Dns.GetHostAddresses(hostname).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return address ?? IPAddress.None;
        }
    }
}