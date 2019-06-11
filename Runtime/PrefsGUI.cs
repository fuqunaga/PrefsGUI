using PrefsGUI.Wrapper;
using RapidGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace PrefsGUI
{
    #region static class
    public class Prefs
    {
        public static void Save() { PrefsGlobal.Save(); }
        public static void Load() { PrefsGlobal.Load(); }
        public static void DeleteAll() { PrefsGlobal.DeleteAll(); }
    }
    #endregion


    [Serializable]
    public class PrefsString : PrefsParam<string>
    {
        public PrefsString(string key, string defaultValue = "") : base(key, defaultValue) { }
    }

    [Serializable]
    public class PrefsInt : PrefsParam<int>
    {
        public PrefsInt(string key, int defaultValue = default(int)) : base(key, defaultValue) { }
    }

    [Serializable]
    public class PrefsFloat : PrefsParam<float>
    {
        public PrefsFloat(string key, float defaultValue = default(float)) : base(key, defaultValue) { }

        public bool OnGUISlider(string label = null) { return OnGUISlider(0f, 1f, label); }
        public bool OnGUISlider(float min, float max, string label = null)
        {
            return DoGUIStrandard((float v) =>
            {
                RGUI.PrefixLabel(label ?? key);
                return RGUI.Slider(v, min, max);//, ref unparsedStr);
            });
        }
    }

    [Serializable]
    public class PrefsBool : PrefsParam<bool>
    {
        public PrefsBool(string key, bool defaultValue = default(bool)) : base(key, defaultValue) { }
        public bool OnGUIToggle(string label = null)
        {
            return DoGUIStrandard((bool v) =>
            {
                return GUILayout.Toggle(v, label);
            });
        }
    }

    [Serializable]
    public class PrefsVector2 : PrefsVector<Vector2>
    {
        public PrefsVector2(string key, Vector2 defaultValue = default(Vector2)) : base(key, defaultValue) { }

        public static implicit operator Vector3(PrefsVector2 v) { return v.Get(); }
        public static implicit operator Vector4(PrefsVector2 v) { return v.Get(); }
    }

    [Serializable]
    public class PrefsVector3 : PrefsVector<Vector3>
    {
        public PrefsVector3(string key, Vector3 defaultValue = default(Vector3)) : base(key, defaultValue) { }

        public static implicit operator Vector2(PrefsVector3 v) { return v.Get(); }
        public static implicit operator Vector4(PrefsVector3 v) { return v.Get(); }
    }

    [Serializable]
    public class PrefsVector4 : PrefsVector<Vector4>
    {
        public PrefsVector4(string key, Vector4 defaultValue = default(Vector4)) : base(key, defaultValue) { }

        public static implicit operator Vector2(PrefsVector4 v) { return v.Get(); }
        public static implicit operator Vector3(PrefsVector4 v) { return v.Get(); }
        public static implicit operator Color(PrefsVector4 v) { return v.Get(); }
    }

    [Serializable]
    public class PrefsVector2Int : PrefsVector<Vector2Int>
    {
        public PrefsVector2Int(string key, Vector2Int defaultValue = default(Vector2Int)) : base(key, defaultValue) { }

        protected override Vector2Int defaultMax { get { return base.defaultMax * 100; } }

        public static implicit operator Vector2Int(PrefsVector2Int v) { return v.Get(); }
    }

    [Serializable]
    public class PrefsVector3Int : PrefsVector<Vector3Int>
    {
        public PrefsVector3Int(string key, Vector3Int defaultValue = default(Vector3Int)) : base(key, defaultValue) { }

        protected override Vector3Int defaultMax { get { return base.defaultMax * 100; } }

        public static implicit operator Vector3Int(PrefsVector3Int v) { return v.Get(); }
    }


    [Serializable]
    public class PrefsColor : PrefsSlider<Color, Vector4>
    {
        public PrefsColor(string key, Color defaultValue = default(Color)) : base(key, defaultValue) { }

        protected override Vector4 defaultMin { get { return Vector4.zero; } }
        protected override Vector4 defaultMax { get { return Vector4.one; } }


        protected override bool Compare(Vector4 lhs, Vector4 rhs)
        {
            return lhs == rhs;
        }

        protected override Color ToOuter(Vector4 v4)
        {
            var c = Color.HSVToRGB(v4.x, v4.y, v4.z);
            c.a = v4.w;
            return c;
        }

        protected override Vector4 ToInner(Color c)
        {
            Vector4 v4 = default(Vector4);
            Color.RGBToHSV(c, out v4.x, out v4.y, out v4.z);
            v4.w = c.a;
            return v4;
        }

        public static implicit operator Vector4(PrefsColor c)
        {
            return c.Get();
        }
    }


    [Serializable]
    public class PrefsRect : PrefsSlider<Rect, Vector4>
    {
        public PrefsRect(string key, Rect defaultValue = default(Rect)) : base(key, defaultValue) { }

        protected override Vector4 defaultMax { get { return Vector4.one; } }
        protected override Vector4 defaultMin { get { return Vector4.zero; } }

        protected override Vector4 ToInner(Rect outerV)
        {
            return new Vector4(outerV.x, outerV.y, outerV.width, outerV.height);
        }

        protected override Rect ToOuter(Vector4 innerV)
        {
            return new Rect(innerV.x, innerV.y, innerV.z, innerV.w);
        }
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
