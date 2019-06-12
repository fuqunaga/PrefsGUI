using PrefsGUI.KVS;
using RapidGUI;
using System;
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
    }

    [Serializable]
    public class PrefsColor : PrefsParam<Color>
    {
        public PrefsColor(string key, Color defaultValue = default) : base(key, defaultValue) { }
    }

s    [Serializable]
    public class PrefsInt : PrefsParam<int>
    {
        public PrefsInt(string key, int defaultValue = default) : base(key, defaultValue) { }

        public bool OnGUISlider(string label = null) => OnGUISlider(0, 100, label);
        public bool OnGUISlider(int min, int max, string label = null)
        {
            return DoGUIStrandard((v) => RGUI.Slider(v, min, max, label ?? key));
        }
    }

    [Serializable]
    public class PrefsFloat : PrefsParam<float>
    {
        public PrefsFloat(string key, float defaultValue = default) : base(key, defaultValue) { }

        public bool OnGUISlider(string label = null) => OnGUISlider(0f, 1f, label);
        public bool OnGUISlider(float min, float max, string label = null)
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


    #region abstract classes

    public abstract class PrefsVector<T> : PrefsSlider<T, T>
    {
        public PrefsVector(string key, T defaultValue = default) : base(key, defaultValue) { }

        static Lazy<T> zero = new Lazy<T>(() => (T)typeof(T).GetProperty("zero").GetValue(null));
        static Lazy<T> one = new Lazy<T>(() => (T)typeof(T).GetProperty("one").GetValue(null));

        protected override T defaultMin => zero.Value;
        protected override T defaultMax => one.Value;


        protected override T ToOuter(T innerV) =>innerV;
        protected override T ToInner(T TouterV) => TouterV;
    }

    public class PrefsParam<T> : PrefsParamOuterInner<T, T>
    {
        public PrefsParam(string key, T defaultValue = default) : base(key, defaultValue) { }

        protected override T ToOuter(T innerV) => innerV;
        protected override T ToInner(T TouterV) => TouterV;
    }

    #endregion
}
