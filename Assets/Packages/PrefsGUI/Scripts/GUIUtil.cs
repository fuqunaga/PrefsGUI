using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class GUIUtil
{
    public static class Style {
        // 参考 https://github.com/XJINE/XJUnity3D.GUI
        public static readonly GUIStyle FoldoutPanelStyle;

        static Style()
        {
            FoldoutPanelStyle = new GUIStyle(GUI.skin.label);
            FoldoutPanelStyle.normal.textColor = GUI.skin.toggle.normal.textColor;
            FoldoutPanelStyle.hover.textColor = GUI.skin.toggle.hover.textColor;

            var tex = new Texture2D(1, 1);
            tex.SetPixels(new[] { new Color(0.5f, 0.5f, 0.5f,0.5f)});
            tex.Apply();
            FoldoutPanelStyle.hover.background = tex;
        }
    }

    public class Folds : Dictionary<string, Fold>
    {
        public void Add(string name, Action action, bool enableFirst = false)
        {
            Fold fold;
            if (TryGetValue(name, out fold))
            {
                fold.Add(action);
            }
            else {
                Add(name, new Fold(name, action, enableFirst));
            }
        }

        public void OnGUI()
        {
            using (var v = new GUILayout.VerticalScope())
            {
                Values.ToList().ForEach(f => f.OnGUI());
            }
        }
    }

    public class Fold
    {
        bool foldOpen;
        string name;
        Action draw;

        public Fold(string n, Action action, bool enableFirst = false)
        {
            name = n;
            draw += action;
            foldOpen = enableFirst;
        }

        public void Add(Action action)
        {
            draw += action;
        }

        public void OnGUI()
        {
            var foldStr = foldOpen ? "▼" : "▶";

            foldOpen ^= GUILayout.Button(foldStr + name, Style.FoldoutPanelStyle);
            if (foldOpen)
            {
                using (var v = new GUILayout.VerticalScope("window"))
                {
                    draw();

                }
            }
        }
    }


    public static T Field<T>(T v, string label = "") { string s = null;  return Field(v, ref s, label); }
    public static T Field<T>(T v, ref string unparsedStr, string label = "")
    {
        var type = typeof(T);
        T ret = default(T);

        using (var h = new GUILayout.HorizontalScope())
        {
            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label);

            ret = (T)(_typeFuncTable.ContainsKey(type)
                ? _typeFuncTable[type](v, ref unparsedStr)
                : ((type.IsEnum)
                    ? EnumField(v)
                    : StandardField(v, ref unparsedStr)
                    )
                );

        }

        return ret;
    }


    #region UnparsedStr Utility
    const char UnparsedStrSeparator = '_';
    static string[] SplitUnparsedStr(string unparsedStr, int elementNum)
    {
        string[] ret = null;
        if (unparsedStr != null)
        {
            ret = unparsedStr.Split(UnparsedStrSeparator);
            Array.Resize(ref ret, elementNum);
        }
        else {
            ret = new string[elementNum];
        }
        return ret;
    }
    static string JoinUnparsedStr(string[] strs)
    {
        return string.Join(UnparsedStrSeparator.ToString(), strs);
    }
    #endregion

    #region Field() Implement
    delegate object FieldFunc(object v, ref string unparsedStr);
    static object FieldFuncBool(object v, ref string unparsedStr) { return GUILayout.Toggle(Convert.ToBoolean(v), ""); }

    static object FieldFuncVector<T>(object v, ref string unparsedStr)
    {
        var elementNum = AbstractVector.GetElementNum<T>();
        var strs = SplitUnparsedStr(unparsedStr, elementNum);
        for (var i = 0; i < elementNum; ++i)
        {
            var elem = Field(AbstractVector.GetAtIdx<T>(v,i), ref strs[i]);
            AbstractVector.SetAtIdx<T>(v,i,elem);
        }
        unparsedStr = JoinUnparsedStr(strs);
        return v;
    }

    static readonly Dictionary<Type, FieldFunc> _typeFuncTable = new Dictionary<Type, FieldFunc>()
    {
        {typeof(bool),  FieldFuncBool },
        {typeof(Vector2), FieldFuncVector<Vector2> },
        {typeof(Vector3), FieldFuncVector<Vector3> },
        {typeof(Vector4), FieldFuncVector<Vector4> },
    };

    static object StandardField<T>(T v, ref string unparsedStr)
    {
        object ret = v;

        var type = typeof(T);

        // フォーカスが外れたときにバリデーションしたい（unparsedStr=nullにすることでv.ToString()に更新される）
        // うまい実装がわからないので簡易的にタブ時に行う
        // マウスイベントも対応したいがこれより前のGUIでイベント食われたとき対応できないので一旦無しで
        if ( Event.current.keyCode== KeyCode.Tab)
        {
            unparsedStr = null;
        }


        var hasUnparsedStr = !string.IsNullOrEmpty(unparsedStr);
        var canParse = false;
        try
        {
            Convert.ChangeType(unparsedStr, type);
            canParse = true; // unparsedStr has value only if ChangeType successed.
        }
        catch (Exception) { }

        var color = (hasUnparsedStr && !canParse) ? Color.red : GUI.color;

        using (var cs = new ColorScope(color))
        {
            unparsedStr = GUILayout.TextField(hasUnparsedStr ? unparsedStr : v.ToString(), GUILayout.MinWidth(70f));
            try
            {
                ret = Convert.ChangeType(unparsedStr, type);
                if ( ret.ToString() == unparsedStr) {
                    unparsedStr = null;
                }
            }
            catch (Exception) {
            }
        }
        return ret;
    }

    static object EnumField<T>(T v)
    {
        var type = typeof(T);
        var enumValues = Enum.GetValues(type).OfType<T>().ToList();

        var isFlag = type.GetCustomAttributes(typeof(System.FlagsAttribute), true).Any();
        if (isFlag)
        {
            var flagV = Convert.ToUInt64(v);
            enumValues.ForEach(value =>
            {
                var flag = Convert.ToUInt64(value);
                if (flag > 0)
                {
                    var has = (flag & flagV) == flag;
                    has = GUILayout.Toggle(has, value.ToString());

                    flagV = has ? (flagV | flag) : (flagV & ~flag);
                }
            });

            v = (T)Enum.ToObject(type, flagV);
        }
        else
        {
            var valueNames = enumValues.Select(value => value.ToString()).ToArray();
            var idx = enumValues.IndexOf(v);
            idx = GUILayout.SelectionGrid(
                idx,
                valueNames,
                valueNames.Length);
            v = enumValues.ElementAtOrDefault(idx);
        }
        return v;
    }
    #endregion

    public static float Slider(float v, string label = "") { return Slider(v, 0f, 1f, label); }
    public static float Slider(float v, ref string unparsedStr, string label = "") { return Slider(v, 0f, 1f, ref unparsedStr, label); }

    public static T Slider<T>(T v, T min, T max, string label = "", string[] elementLabels = null)
    {
        string unparsedStr = null;
        return Slider(v, min, max, ref unparsedStr, label, elementLabels);
    }

    public static T Slider<T>(T v, T min, T max, ref string unparsedStr, string label = "", string[] elementLabels = null)
    {
        return (T)_typeSliderFuncTable[typeof(T)](v, min, max, ref unparsedStr, label, elementLabels);
    }

    #region Slider() Implement
    delegate object SliderFunc(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null);

    public static object SliderInt(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null)
    {
        return Mathf.FloorToInt((float)SliderFloat((float)(int)v, (float)(int)min, (float)(int)max, ref unparsedStr, label));
    }

    public static object SliderFloat(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null)
    {
        float ret = default(float);
        using (var h = new GUILayout.HorizontalScope())
        {
            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label);
            ret = GUILayout.HorizontalSlider((float)v, (float)min, (float)max, GUILayout.MinWidth(200));
            ret = Field(ret, ref unparsedStr);
        }

        return ret;
    }


    static readonly string[] defaultElemLabels = new[] { "x", "y", "z", "w" };
    static object SliderFuncVector<T>(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null)
    {
        var elementNum = AbstractVector.GetElementNum<T>();
        var eLabels = elemLabels ?? defaultElemLabels;

        using (var h0 = new GUILayout.HorizontalScope())
        {
            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label);
            using (var vertical = new GUILayout.VerticalScope())
            {
                var strs = SplitUnparsedStr(unparsedStr, elementNum);
                for (var i = 0; i < elementNum; ++i)
                {
                    using (var h1 = new GUILayout.HorizontalScope())
                    {
                        var elem = Slider(AbstractVector.GetAtIdx<T>(v, i), AbstractVector.GetAtIdx<T>(min, i), AbstractVector.GetAtIdx<T>(max, i), ref strs[i], eLabels[i]);
                        AbstractVector.SetAtIdx<T>(v, i, elem);
                    }
                }
                unparsedStr = JoinUnparsedStr(strs);
            }
        }

        return v;
    }

    static readonly Dictionary<Type, SliderFunc> _typeSliderFuncTable = new Dictionary<Type, SliderFunc>()
    {
        {typeof(int), SliderInt },
        {typeof(float), SliderFloat },
        {typeof(Vector2), SliderFuncVector<Vector2> },
        {typeof(Vector3), SliderFuncVector<Vector3> },
        {typeof(Vector4), SliderFuncVector<Vector4> },
    };

    #endregion


    public static int IntButton(int v, string label = "")
    {
        using (var h = new GUILayout.HorizontalScope())
        {
            v = Field(v, label);
            if (GUILayout.Button("+")) v++;
            if (GUILayout.Button("-")) v--;
        }
        return v;
    }


    public static void Indent(Action action) { Indent(1, action); }
    public static void Indent(int level, Action action)
    {
        const int TAB = 20;
        using (var h = new GUILayout.HorizontalScope())
        {
            GUILayout.Space(TAB * level);
            using (var v = new GUILayout.VerticalScope())
            {
                action();
            }
        }
    }

    public class ColorScope : IDisposable
    {
        Color _color;
        public ColorScope(Color color)
        {
            _color = GUI.color;
            GUI.color = color;
        }

        public void Dispose()
        {
            GUI.color = _color;
        }
    }
}
