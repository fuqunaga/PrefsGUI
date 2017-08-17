using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class GUIUtil
{
    #region Field()
    public static T Field<T>(T v, string label = "", params GUILayoutOption[] options) { string s = null; return Field(v, ref s, label, options); }
    public static T Field<T>(T v, ref string unparsedStr, string label, params GUILayoutOption[] options)
    {
        var type = typeof(T);
        T ret = default(T);

        using (var h = new GUILayout.HorizontalScope())
        {
            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label);

            ret = (T)(_typeFuncTable.ContainsKey(type)
                ? _typeFuncTable[type](v, ref unparsedStr, options)
                : ((type.IsEnum)
                    ? EnumField(v, options)
                    : StandardField(v, ref unparsedStr, options)
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
        else
        {
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
    delegate object FieldFunc(object v, ref string unparsedStr, params GUILayoutOption[] options);
    static object FieldFuncBool(object v, ref string unparsedStr, params GUILayoutOption[] options) { return GUILayout.Toggle(Convert.ToBoolean(v), "", options); }
    static object FieldFuncRect(object v, ref string unparsedStr, params GUILayoutOption[] options)
    {
        const int elementNum = 4;
        var strs = SplitUnparsedStr(unparsedStr, elementNum);

        var rect = (Rect)v;
        rect.x = Field(rect.x, ref strs[0], "x", options);
        rect.y = Field(rect.y, ref strs[1], "y", options);
        rect.width = Field(rect.width, ref strs[2], "w", options);
        rect.height = Field(rect.height, ref strs[3], "h", options);

        unparsedStr = JoinUnparsedStr(strs);
        return rect;
    }

    static object FieldFuncVector<T>(object v, ref string unparsedStr, params GUILayoutOption[] options)
    {
        var elementNum = AbstractVector.GetElementNum<T>();
        var strs = SplitUnparsedStr(unparsedStr, elementNum);
        for (var i = 0; i < elementNum; ++i)
        {
            var elem = Field(AbstractVector.GetAtIdx<T>(v, i), ref strs[i], "", options);
            v = AbstractVector.SetAtIdx<T>(v, i, elem);
        }
        unparsedStr = JoinUnparsedStr(strs);
        return v;
    }

    static readonly Dictionary<Type, FieldFunc> _typeFuncTable = new Dictionary<Type, FieldFunc>()
    {
        {typeof(bool),  FieldFuncBool },
        {typeof(Rect), FieldFuncRect },
        {typeof(Vector2), FieldFuncVector<Vector2> },
        {typeof(Vector3), FieldFuncVector<Vector3> },
        {typeof(Vector4), FieldFuncVector<Vector4> },
    };

    class ForcusChecker
    {
        int time;
        int mouseId;
        int keyboardId;
        bool changed;

        public bool IsChanged()
        {
            if (time != Time.frameCount)
            {
                time = Time.frameCount;

                var currentMouse = GUIUtility.hotControl;
                var currentKeyboard = GUIUtility.keyboardControl;

                changed = (keyboardId != currentKeyboard) || (mouseId != currentMouse);
                if ( changed ) {
                    keyboardId = currentKeyboard;
                    mouseId = currentMouse;
                }
            }

            return changed;
        }
    }

    static ForcusChecker _forcusChecker = new ForcusChecker();

    static object StandardField<T>(T v, ref string unparsedStr, params GUILayoutOption[] options)
    {
        object ret = v;

        var type = typeof(T);

        // validate when unfocused (unparsedStr=null then v.ToString will to be set)）
        if ( _forcusChecker.IsChanged() )
        {
            unparsedStr = null;
        }

        var hasUnparsedStr = !string.IsNullOrEmpty(unparsedStr);
        var canParse = false;
        try
        {
            canParse = Convert.ChangeType(unparsedStr, type).ToString() == unparsedStr;
        }
        catch (Exception) { }

        var color = (hasUnparsedStr && !canParse) ? Color.red : GUI.color;

        using (var cs = new ColorScope(color))
        {
            unparsedStr = GUILayout.TextField(hasUnparsedStr ? unparsedStr : v.ToString(), options.Concat(new[] { GUILayout.MinWidth(70f) }).ToArray());
            try
            {
                ret = Convert.ChangeType(unparsedStr, type);
                if (ret.ToString() == unparsedStr)
                {
                    unparsedStr = null;
                }
            }
            catch (Exception)
            {
            }
        }
        return ret;
    }

    static object EnumField<T>(T v, params GUILayoutOption[] options)
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
                    has = GUILayout.Toggle(has, value.ToString(), options);

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
#endregion

#region Slider
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
            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label, GUILayout.ExpandWidth(false));
            ret = GUILayout.HorizontalSlider((float)v, (float)min, (float)max, GUILayout.MinWidth(200));
            ret = (float)StandardField(ret, ref unparsedStr, GUILayout.MaxWidth(100f));
        }

        return ret;
    }

    static readonly string[] defaultElemLabelsRect = new[] { "x", "y", "w", "h" };
    static object SliderFuncRect(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null)
    {
        const int elementNum = 4;
        var eLabels = elemLabels ?? defaultElemLabelsRect;

        using (var h0 = new GUILayout.HorizontalScope())
        {
            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label);
            using (var vertical = new GUILayout.VerticalScope())
            {
                var strs = SplitUnparsedStr(unparsedStr, elementNum);
                var rect = (Rect)v;
                var rectMin = (Rect)min;
                var rectMax = (Rect)max;

                rect.x = Slider(rect.x, rectMin.x, rectMax.x, ref strs[0], eLabels[0]);
                rect.y = Slider(rect.y, rectMin.y, rectMax.y, ref strs[1], eLabels[1]);
                rect.width = Slider(rect.width, rectMin.width, rectMax.width, ref strs[2], eLabels[2]);
                rect.height = Slider(rect.height, rectMin.height, rectMax.height, ref strs[3], eLabels[3]);

                v = rect;

                unparsedStr = JoinUnparsedStr(strs);
            }
        }

        return v;
    }


    static readonly string[] defaultElemLabelsVector = new[] { "x", "y", "z", "w" };
    static object SliderFuncVector<T>(object v, object min, object max, ref string unparsedStr, string label = "", string[] elemLabels = null)
    {
        var elementNum = AbstractVector.GetElementNum<T>();
        var eLabels = elemLabels ?? defaultElemLabelsVector;

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
                        v = AbstractVector.SetAtIdx<T>(v, i, elem);
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
        {typeof(Rect), SliderFuncRect },
        {typeof(Vector2), SliderFuncVector<Vector2> },
        {typeof(Vector3), SliderFuncVector<Vector3> },
        {typeof(Vector4), SliderFuncVector<Vector4> },
    };

#endregion

#endregion

    public static int IntButton(int v, string label = "")
    {
        using (var h = new GUILayout.HorizontalScope())
        {
            v = Field(v, label);
            const float width = 20f;
            if (GUILayout.Button("+", GUILayout.Width(width))) v++;
            if (GUILayout.Button("-", GUILayout.Width(width))) v--;
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
