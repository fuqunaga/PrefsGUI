using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using PrefsWrapper;


namespace PrefsGUI
{
    [Serializable]
    public class PrefsString : PrefsParam<string>
    {
        public PrefsString(string key, string defaultValue = default(string)) : base(key, defaultValue) { }
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
            return OnGUIStrandardStyle((float v, ref string unparsedStr) =>
            {
                GUILayout.Label(label ?? key);
                return GUIUtil.Slider(v, min, max, ref unparsedStr);
            });
        }
    }

    [Serializable]
    public class PrefsBool : PrefsParam<bool>
    {
        public PrefsBool(string key, bool defaultValue = default(bool)) : base(key, defaultValue) { }
    }

    [Serializable]
    public class PrefsVector2 : PrefsVector<Vector2>
    {
        public PrefsVector2(string key, Vector2 defaultValue = default(Vector2)) : base(key, defaultValue) { }
    }

    [Serializable]
    public class PrefsVector3 : PrefsVector<Vector3>
    {
        public PrefsVector3(string key, Vector3 defaultValue = default(Vector3)) : base(key, defaultValue) { }
    }

    [Serializable]
    public class PrefsVector4 : PrefsVector<Vector4>
    {
        public PrefsVector4(string key, Vector4 defaultValue = default(Vector4)) : base(key, defaultValue) { }
    }

    [Serializable]
    public class PrefsColor : PrefsTuple<Color, Vector4>
    {
        public PrefsColor(string key, Color defaultValue = default(Color)) : base(key, defaultValue) { }

        protected override Vector4 defaultMin { get { return Vector4.zero; } }
        protected override Vector4 defaultMax { get { return Vector4.one; } }

        static readonly string[] _defaultElementLabels = new[] { "H", "S", "V", "A" };
        protected override string[] defaultEelementLabels
        {
            get
            {
                return _defaultElementLabels;
            }
        }

        protected override void OnGUISliderRight(Vector4 v)
        {
            var c = ToOuter(v);
            using (var cs = new GUIUtil.ColorScope(c))
            {
                GUILayout.Label("■■■");
            }
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
    }


    #region abstract classes
    public abstract class PrefsVector<T> : PrefsTuple<T, T>
    {
        public PrefsVector(string key, T defaultValue = default(T)) : base(key, defaultValue) { }

        protected override T defaultMin { get { return (T)typeof(T).InvokeMember("zero", BindingFlags.Static | BindingFlags.GetProperty, null, null, null); } }
        protected override T defaultMax { get { return (T)typeof(T).InvokeMember("one", BindingFlags.Static | BindingFlags.GetProperty, null, null, null); } }

        static readonly string[] _defaultElementLabels = new[] { "x", "y", "z", "w" };
        protected override string[] defaultEelementLabels
        {
            get
            {
                return _defaultElementLabels;
            }
        }
        protected override T ToOuter(T innerV) { return innerV; }
        protected override T ToInner(T TouterV) { return TouterV; }
    }

    public abstract class PrefsTuple<OuterT, InnerT> : PrefsParam<OuterT, InnerT>
    {
        bool foldOpen;

        #region abstract
        protected abstract string[] defaultEelementLabels { get; }
        protected abstract InnerT defaultMin { get; }
        protected abstract InnerT defaultMax { get; }
        #endregion abstract

        public PrefsTuple(string key, OuterT defaultValue = default(OuterT)) : base(key, defaultValue) { }

        public bool OnGUISlider(string label = null)
        {
            return OnGUISlider(defaultMin, defaultMax, label);
        }

        public bool OnGUISlider(OuterT min, OuterT max, string label = null, string[] elementLabels = null)
        {
            return OnGUISlider(ToInner(min), ToInner(max), label, elementLabels);
        }

        protected bool OnGUISlider(InnerT min, InnerT max, string label = null, string[] elementLabels = null)
        {
            return OnGUIStrandardStyle((InnerT v, ref string unparsedStr) =>
            {
                elementLabels = elementLabels ?? defaultEelementLabels;

                using (var h = new GUILayout.HorizontalScope())
                {
                    var foldStr = foldOpen ? "▼" : "▶";

                    foldOpen ^= GUILayout.Button(foldStr + (label ?? key), GUIUtil.Style.FoldoutPanelStyle);

                    v = foldOpen
                        ? GUIUtil.Slider(v, min, max, ref unparsedStr, "", elementLabels)
                        : GUIUtil.Field(v, ref unparsedStr);

                    OnGUISliderRight(v);
                    //GUILayout.FlexibleSpace();
                }

                return v;
            });
        }

        protected virtual void OnGUISliderRight(InnerT v) { }
    }


    public class PrefsParam<T> : PrefsParam<T, T>
    {
        public PrefsParam(string key, T defaultValue = default(T)) : base(key, defaultValue) { }
        protected override T ToOuter(T innerV) { return innerV; }
        protected override T ToInner(T TouterV) { return TouterV; }
    }

    public abstract class PrefsParam<OuterT, InnerT> : _PrefsParam
    {
        [SerializeField]
        protected OuterT defaultValue;

        public PrefsParam(string key, OuterT defaultValue = default(OuterT)) : base(key)
        {
            this.defaultValue = defaultValue;
        }

        public static implicit operator OuterT(PrefsParam<OuterT, InnerT> me)
        {
            return me.Get();
        }

        public virtual OuterT Get() { return ToOuter(_Get()); }
        protected virtual InnerT _Get() { return PlayerPrefs<InnerT>.Get(key, ToInner(defaultValue)); }

        public virtual void Set(OuterT v) { _Set(ToInner(v)); }
        protected virtual void _Set(InnerT v) { PlayerPrefs<InnerT>.Set(key, v); }

        public void SetWithDefault(OuterT v) { Set(v); defaultValue = v; }

        public virtual void Delete() { PlayerPrefs.DeleteKey(key); }

        public bool OnGUI(string label = null)
        {
            return OnGUIStrandardStyle((InnerT v, ref string unparsedStr) => GUIUtil.Field<InnerT>(v, ref unparsedStr, label ?? key));
        }


        #region abstract
        protected abstract OuterT ToOuter(InnerT innerV);
        protected abstract InnerT ToInner(OuterT outerV);
        #endregion


        #region GUI Implement
        protected bool OnGUIStrandardStyle(GUIFunc guiFunc)
        {
            return OnGUIwithButton(() => OnGUIWithUnparsedStr(key, guiFunc));
        }

        protected bool OnGUIwithButton(Func<bool> onGUIFunc)
        {
            var changed = false;
            using (var h = new GUILayout.HorizontalScope())
            {
                changed = onGUIFunc();

                if (GUILayout.Button("default", GUILayout.Width(60f)))
                {
                    Set(defaultValue);
                    changed = true;
                }
            }

            return changed;
        }

        static Dictionary<string, string> _unparsedStrTable = new Dictionary<string, string>();
        protected delegate InnerT GUIFunc(InnerT v, ref string unparsedStr);

        protected bool OnGUIWithUnparsedStr(string key, GUIFunc guiFunc)
        {
            var changed = false;
            if (!PlayerPrefs<InnerT>.HasKey(key))
            {
                Set(defaultValue);
                changed = true;
            }

            var hasUnparsedStr = _unparsedStrTable.ContainsKey(key);
            var unparsedStr = hasUnparsedStr ? _unparsedStrTable[key] : null;

            var prev = _Get();
            var next = guiFunc(prev, ref unparsedStr);
            if (!prev.Equals(next))
            {
                _Set(next);
                changed = true;
            }

            if (unparsedStr != null) _unparsedStrTable[key] = unparsedStr;
            else if (hasUnparsedStr) _unparsedStrTable.Remove(key);

            return changed;
        }
        #endregion
    }

    public abstract class _PrefsParam
    {
        public string key;

        public _PrefsParam(string key) { this.key = key; }
    }
    #endregion
}
