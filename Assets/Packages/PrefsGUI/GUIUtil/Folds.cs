using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public static partial class GUIUtil
{
    #region Fold
    public static class Style
    {
        // 参考 https://github.com/XJINE/XJUnity3D.GUI
        public static readonly GUIStyle FoldoutPanelStyle;

        static Style()
        {
            FoldoutPanelStyle = new GUIStyle(GUI.skin.label);
            FoldoutPanelStyle.normal.textColor = GUI.skin.toggle.normal.textColor;
            FoldoutPanelStyle.hover.textColor = GUI.skin.toggle.hover.textColor;

            var tex = new Texture2D(1, 1);
            tex.SetPixels(new[] { new Color(0.5f, 0.5f, 0.5f, 0.5f) });
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

        public Fold Add(string name, Action drawFunc, bool enableFirst = false) => Add(name, null, drawFunc, enableFirst);

        public Fold Add(string name, Func<bool> checkEnableFunc, Action drawFunc, bool enableFirst = false) => Add(0, name, checkEnableFunc, drawFunc, enableFirst);

        public Fold Add(int order, string name, Action drawFunc, bool enableFirst = false)  => Add(order, name, null, drawFunc, enableFirst);

        public Fold Add(int order, string name, Func<bool> checkEnableFunc, Action drawFunc, bool enableFirst = false)
        {
            Fold ret;
            FoldData foldData;
            if (_dic.TryGetValue(name, out foldData))
            {
                foldData._order = order;
                foldData._fold.Add(checkEnableFunc, drawFunc);
                ret = foldData._fold;
            }
            else
            {
                ret = new Fold(name, checkEnableFunc, drawFunc, enableFirst);
                _dic.Add(name, new FoldData
                {
                    _order = order,
                    _fold = ret,
                });
            }

            _needUpdate = true;

            return ret;
        }

        public void Remove(string name)
        {
            if (_dic.ContainsKey(name))
            {
                _dic.Remove(name);
            }

            _needUpdate = true;
        }


        List<Fold> _folds = new List<Fold>();
        public void OnGUI()
        {
            if (_needUpdate)
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
        Action _titleAction;

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

        public void SetTitleAction(Action titleAction) => _titleAction = titleAction;

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

                using (var h = new GUILayout.HorizontalScope())
                {
                    _foldOpen ^= GUILayout.Button(foldStr + _name, Style.FoldoutPanelStyle);
                    _titleAction?.Invoke();
                }
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

}