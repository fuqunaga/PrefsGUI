using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public static class GUIUtil
{
    #region Fold
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

    public class Folds
    {
        public class FoldData
        {
            public int _order;
            public Fold _fold;
        }

        Dictionary<string, FoldData> _dic = new Dictionary<string, FoldData>();
        bool _needUpdate = true;

        public void Add(string name, Action drawFunc, bool enableFirst = false)
        {
            Add(name, null, drawFunc, enableFirst);
        }

        public void Add(string name, Func<bool> checkEnableFunc, Action drawFunc, bool enableFirst = false)
        {
            Add(0, name, checkEnableFunc, drawFunc, enableFirst);
        }

        public void Add(int order, string name, Action drawFunc, bool enableFirst = false) { Add(order, name, null, drawFunc, enableFirst); }

        public void Add(int order, string name, Func<bool> checkEnableFunc, Action drawFunc, bool enableFirst = false)
        {
            FoldData foldData;
            if (_dic.TryGetValue(name, out foldData))
            {
                foldData._order = order;
                foldData._fold.Add(checkEnableFunc, drawFunc);
            }
            else {
                _dic.Add(name, new FoldData
                {
                    _order = order,
                    _fold = new Fold(name, checkEnableFunc, drawFunc, enableFirst)
                });
            }

            _needUpdate = true;
        }

        public void Remove(string name)
        {
            if ( _dic.ContainsKey(name))
            {
                _dic.Remove(name);
            }

            _needUpdate = true;
        }


        List<Fold> _folds = new List<Fold>();
        public void OnGUI()
        {
            if ( _needUpdate )
            {
                _folds = _dic.Values.OrderBy(of => of._order).Select(of => of._fold).ToList();
                _needUpdate = false;
            }

            using (var v = new GUILayout.VerticalScope())
            {
                _folds.ForEach(fold => fold.OnGUI());
            }
        }
    }

    public class Fold
    {
        bool _foldOpen;
        string _name;

        public class FuncData
        {
            public Func<bool> _checkEnable;
            public Action _draw;
        }
        List<FuncData> _funcDatas = new List<FuncData>();

        public Fold(string name, Action drawFunc, bool enableFirst = false) : this(name, null, drawFunc, enableFirst) { }

        public Fold(string name, Func<bool> checkEnableFunc, Action drawFunc, bool enableFirst = false)
        {
            _name = name;
            _foldOpen = enableFirst;
            Add(checkEnableFunc, drawFunc);
        }

        public void Add(Action drawFunc) { Add(null, drawFunc); }
        public void Add(Func<bool> checkEnableFunc, Action drawFunc)
        {
            _funcDatas.Add(new FuncData()
            {
                _checkEnable = checkEnableFunc,
                _draw = drawFunc
            });
        }

        public void OnGUI()
        {
            var drawFuncs = _funcDatas.Where(fd => fd._checkEnable == null || fd._checkEnable()).Select(fd => fd._draw).ToList();

            if (drawFuncs.Any())
            {
                var foldStr = _foldOpen ? "▼" : "▶";

                _foldOpen ^= GUILayout.Button(foldStr + _name, Style.FoldoutPanelStyle);
                if (_foldOpen)
                {
                    using (var v = new GUILayout.VerticalScope("window"))
                    {
                        drawFuncs.ForEach(drawFunc => drawFunc());
                    }
                }
            }
        }
    }
    #endregion

    #region FoldUtil

    public interface IDebugMenu { void DebugMenu(); }

    public static void Add(this Folds folds, string name, params Type[] iDebugMenuTypes)
    {
        folds.Add(name, false, iDebugMenuTypes);
    }

    public static void Add(this Folds folds, string name, bool enableFirst, params Type[] iDebugMenuTypes)
    {
        folds.Add(0, name, enableFirst, iDebugMenuTypes);
    }

    public static void Add(this Folds folds, int order, string name, params Type[] iDebugMenuTypes)
    {
        folds.Add(order, name, false, iDebugMenuTypes);
    }

    public static void Add(this Folds folds, int order, string name, bool enableFirst, params Type[] iDebugMenuTypes)
    {
        Assert.IsTrue(iDebugMenuTypes.All(type => type.GetInterfaces().Contains(typeof(IDebugMenu))));

        var iDebugMenus = iDebugMenuTypes.Select(t => new LazyFindObject(t)).ToList() // exec once.
            .Select(lfo => lfo.GetObject()).Where(o => o != null).Cast<IDebugMenu>();   // exec every call.

        folds.Add(order, name, () => iDebugMenus.Any(), () => iDebugMenus.ToList().ForEach(idm => idm.DebugMenu()), enableFirst);
    }

    /// <summary>
    /// FindObjectOfTypeを呼びまくるのは重いので適度に散らす
    /// </summary>
    public class LazyFindObject
    {
        protected UnityEngine.Object _obj;
        protected Type _type;
        protected int _delayCount;
        const int _delayCountMax = 60;

        public LazyFindObject(Type type)
        {
            _type = type;
        }

        public UnityEngine.Object GetObject()
        {
            if ((Event.current.type == EventType.Layout) && _obj == null)
            {
                if (--_delayCount <= 0)
                {
                    _obj = UnityEngine.Object.FindObjectOfType(_type);
                    _delayCount = UnityEngine.Random.Range(0, _delayCountMax);
                }
            }
            return _obj;
        }
    }


    #endregion


    #region Field()
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
    static object FieldFuncRect(object v, ref string unparsedStr)
    {
        const int elementNum = 4;
        var strs = SplitUnparsedStr(unparsedStr, elementNum);

        var rect = (Rect)v;
        rect.x = Field(rect.x, ref strs[0], "x");
        rect.y = Field(rect.y, ref strs[1], "y");
        rect.width = Field(rect.width, ref strs[2], "w");
        rect.height = Field(rect.height, ref strs[3], "h");

        unparsedStr = JoinUnparsedStr(strs);
        return rect;
    }

    static object FieldFuncVector<T>(object v, ref string unparsedStr)
    {
        var elementNum = AbstractVector.GetElementNum<T>();
        var strs = SplitUnparsedStr(unparsedStr, elementNum);
        for (var i = 0; i < elementNum; ++i)
        {
            var elem = Field(AbstractVector.GetAtIdx<T>(v,i), ref strs[i]);
            v = AbstractVector.SetAtIdx<T>(v,i,elem);
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
            canParse = Convert.ChangeType(unparsedStr, type).ToString() == unparsedStr;
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
            if (!string.IsNullOrEmpty(label)) GUILayout.Label(label);
            ret = GUILayout.HorizontalSlider((float)v, (float)min, (float)max, GUILayout.MinWidth(200));
            ret = Field(ret, ref unparsedStr);
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

                rect.x      = Slider(rect.x,      rectMin.x,      rectMax.x,      ref strs[0], eLabels[0]);
                rect.y      = Slider(rect.y,      rectMin.y,      rectMax.y,      ref strs[1], eLabels[1]);
                rect.width  = Slider(rect.width,  rectMin.width,  rectMax.width,  ref strs[2], eLabels[2]);
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
