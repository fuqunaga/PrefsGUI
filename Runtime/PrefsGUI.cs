using PrefsGUI.KVS;
using RapidGUI;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace PrefsGUI
{
    #region static class
    public class Prefs
    {
        public static void Save() { PrefsKVS.Save(); }
        public static void Load() { PrefsKVS.Load(); }
        public static void DeleteAll() { PrefsKVS.DeleteAll(); }
    }
    #endregion


    [Serializable]
    public class PrefsString : PrefsParam<string>
    {
        public PrefsString(string key, string defaultValue = "") : base(key, defaultValue) { }
    }

    [Serializable]
    public class PrefsBool : PrefsParam<bool>
    {
        public PrefsBool(string key, bool defaultValue = default(bool)) : base(key, defaultValue) { }

        public bool DoGUIToggle(string label = null)
        {
            return DoGUIStrandard((v) => GUILayout.Toggle(v, label ?? key));
        }
    }

    [Serializable]
    public class PrefsColor : PrefsParam<Color>
    {
        public PrefsColor(string key, Color defaultValue = default) : base(key, defaultValue) { }
    }

    [Serializable]
    public class PrefsInt : PrefsParam<int>
    {
        public PrefsInt(string key, int defaultValue = default) : base(key, defaultValue) { }

        public bool DoGUISlider(string label = null) => DoGUISlider(0, 100, label);
        public bool DoGUISlider(int max, string label = null) => DoGUISlider(0, max, label);
        public bool DoGUISlider(int min, int max, string label = null)
        {
            return DoGUIStrandard((v) => RGUI.Slider(v, min, max, label ?? key));
        }
    }

    [Serializable]
    public class PrefsFloat : PrefsParam<float>
    {
        public PrefsFloat(string key, float defaultValue = default) : base(key, defaultValue) { }

        public bool DoGUISlider(string label = null) => DoGUISlider(0f, 1f, label);
        public bool DoGUISlider(float max, string label = null) => DoGUISlider(0f, max, label);
        public bool DoGUISlider(float min, float max, string label = null)
        {
            return DoGUIStrandard((v) => RGUI.Slider(v, min, max, label ?? key));
        }
    }

    [Serializable]
    public class PrefsVector2 : PrefsVector<Vector2>
    {
        public PrefsVector2(string key, Vector2 defaultValue = default) : base(key, defaultValue) { }

        public static implicit operator Vector3(PrefsVector2 v) => v.Get();
        public static implicit operator Vector4(PrefsVector2 v) => v.Get();
    }

    [Serializable]
    public class PrefsVector3 : PrefsVector<Vector3>
    {
        public PrefsVector3(string key, Vector3 defaultValue = default) : base(key, defaultValue) { }

        public static implicit operator Vector2(PrefsVector3 v) => v.Get();
        public static implicit operator Vector4(PrefsVector3 v) => v.Get();
    }

    [Serializable]
    public class PrefsVector4 : PrefsVector<Vector4>
    {
        public PrefsVector4(string key, Vector4 defaultValue = default) : base(key, defaultValue) { }

        public static implicit operator Vector2(PrefsVector4 v) => v.Get();
        public static implicit operator Vector3(PrefsVector4 v) => v.Get();
        public static implicit operator Color(PrefsVector4 v)   => v.Get();
    }

    [Serializable]
    public class PrefsVector2Int : PrefsVector<Vector2Int>
    {
        public PrefsVector2Int(string key, Vector2Int defaultValue = default) : base(key, defaultValue) { }

        protected override Vector2Int defaultMax => base.defaultMax * 100;

        public static implicit operator Vector2(PrefsVector2Int v) => v.Get();
    }

    [Serializable]
    public class PrefsVector3Int : PrefsVector<Vector3Int>
    {
        public PrefsVector3Int(string key, Vector3Int defaultValue = default) : base(key, defaultValue) { }

        protected override Vector3Int defaultMax => base.defaultMax * 100;

        public static implicit operator Vector3(PrefsVector3Int v) => v.Get();
    }


    [Serializable]
    public class PrefsRect : PrefsSlider<Rect>
    {
        public PrefsRect(string key, Rect defaultValue = default) : base(key, defaultValue) { }

        protected override Rect defaultMax => new Rect(Vector2.one * 100f, Vector2.one * 100f);
        protected override Rect defaultMin => default;
    }

    [Serializable]
    public class PrefsBounds : PrefsSlider<Bounds>
    {
        public PrefsBounds(string key, Bounds defaultValue = default) : base(key, defaultValue)
        {
        }

        protected override Bounds defaultMin => default;

        protected override Bounds defaultMax => new Bounds(Vector3.one * 100f, Vector3.one * 100f);
    }

    [Serializable]
    public class PrefsBoundsInt : PrefsSlider<BoundsInt>
    {
        public PrefsBoundsInt(string key, BoundsInt defaultValue = default) : base(key, defaultValue)
        {
        }

        protected override BoundsInt defaultMin => default;

        protected override BoundsInt defaultMax => new BoundsInt(Vector3Int.one * 100, Vector3Int.one * 100);
    }


    [Serializable]
    public class PrefsIPEndPoint : PrefsSet<PrefsString, PrefsInt, string, int>
    {
        static string[] _paramNames = new[] { "address", "port" };
        protected override string[] paramNames => _paramNames;


        public string address => prefs0.Get();
        public int port => prefs1.Get();

        public PrefsIPEndPoint(string key, string hostname = "localhost", int port = 10000) : base(key, hostname, port) { }

        public static implicit operator IPEndPoint(PrefsIPEndPoint me) => CreateIPEndPoint(me.address, me.port);

        public static IPEndPoint CreateIPEndPoint(string address, int port)
        {
            var ip = FindFromHostName(address);
            return (ip != IPAddress.None) ? new IPEndPoint(ip, port) : null;
        }

        public static IPAddress FindFromHostName(string hostname)
        {
            var address = Dns.GetHostAddresses(hostname).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return address ?? IPAddress.None;
        }
    }
}
