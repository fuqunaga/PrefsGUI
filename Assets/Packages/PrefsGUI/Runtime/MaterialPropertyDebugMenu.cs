using UnityEngine;
using MaterialPropertyAccessor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using PrefsGUI;


[ExecuteInEditMode]
public class MaterialPropertyDebugMenu : MaterialPropertyBehaviour
{
    #region TypeDefine
    [System.Serializable]
    public class PrefsTexEnv : PrefsVector4
    {
        public PrefsTexEnv(string key, Vector2 tiling, Vector2 offset = default(Vector2)) : base(key, new Vector4(tiling.x, tiling.y, offset.x, offset.y)) { }
        public Vector2 GetScale() { var v = Get();  return new Vector2(v.x, v.y); }
        public Vector2 GetOffset() { var v = Get(); return new Vector2(v.z, v.w); }
    }
    #endregion

    static bool _update;
    public static bool update { get { return _update; } set { _update = value; }  }

    [SerializeField]
    List<PrefsColor> _colors = new List<PrefsColor>();
    [SerializeField]
    List<PrefsVector4> _vectors = new List<PrefsVector4>();
    [SerializeField]
    List<PrefsFloat> _floats = new List<PrefsFloat>();
    [SerializeField]
    List<PrefsFloat> _ranges = new List<PrefsFloat>();
    [SerializeField]
    List<PrefsTexEnv> _texEnvs = new List<PrefsTexEnv>();


    string keyPrefix { get
        {
            Assert.IsNotNull(_material);
            return _material.name + "-";
        }
    }

    string PropertyNameToKey(string n){ return keyPrefix + n; }
    string KeyToPropertyName(string key){ return key.Replace(keyPrefix, "");  }

    public void Start()
    {
        UpdateMaterial();
    }


    public override void Update()
    {
        base.Update();

#if UNITY_EDITOR
        UpdatePrefs();
#endif

        if ( update)
        {
            UpdateMaterial();
        }
    }

    void UpdatePrefs()
    {
        var colorKeys  = _propertySet.colors.Select(name => PropertyNameToKey(name)).ToList();
        var vectorKeys = _propertySet.vectors.Select(name => PropertyNameToKey(name)).ToList();
        var floatKeys  = _propertySet.floats.Select(name => PropertyNameToKey(name)).ToList();
        var rangeKeys  = _propertySet.ranges.Select(range => PropertyNameToKey(range.name)).ToList();
        var texEnvKeys = _propertySet.texEnvs.Select(name => PropertyNameToKey(name)).ToList();


        _colors.RemoveAll ( c => !colorKeys.Contains  ( c.key ));
        _vectors.RemoveAll( v => !vectorKeys.Contains ( v.key ));
        _floats.RemoveAll ( f => !floatKeys.Contains  ( f.key ));
        _ranges.RemoveAll ( r => !rangeKeys.Contains  ( r.key ));
        _texEnvs.RemoveAll( t => !texEnvKeys.Contains(t.key));

        _colors.AddRange (colorKeys.Except (_colors.Select (c => c.key)).Select(n => new PrefsColor(n, _material.GetColor(KeyToPropertyName(n)))));
        _vectors.AddRange(vectorKeys.Except(_vectors.Select(v => v.key)).Select(n => new PrefsVector4(n, _material.GetVector(KeyToPropertyName(n)))));
        _floats.AddRange (floatKeys.Except (_floats.Select (f => f.key)).Select(n => new PrefsFloat  (n, _material.GetFloat(KeyToPropertyName(n)))));
        _ranges.AddRange (rangeKeys.Except (_ranges.Select (r => r.key)).Select(n => new PrefsFloat  (n, _material.GetFloat(KeyToPropertyName(n)))));
        _texEnvs.AddRange(texEnvKeys.Except(_texEnvs.Select(t => t.key)).Select(n =>
        {
            var pn = KeyToPropertyName(n);
            var tiling = _material.GetTextureScale(pn);
            var offset = _material.GetTextureOffset(pn);
            return new PrefsTexEnv(n, tiling, offset);
        }));
    }

    void UpdateMaterial()
    {
        if ((_material != null) && _propertySet.Any())
        {
            _colors.ForEach(c => _material.SetColor(KeyToPropertyName(c.key), c.Get()));
            _vectors.ForEach(v => _material.SetVector(KeyToPropertyName(v.key), v.Get()));
            _floats.ForEach(f => _material.SetFloat(KeyToPropertyName(f.key), f.Get()));
            _ranges.ForEach(r => _material.SetFloat(KeyToPropertyName(r.key), r.Get()));
            _texEnvs.ForEach(t =>
            {
                var pn = KeyToPropertyName(t.key);
                _material.SetTextureScale(pn, t.GetScale());
                _material.SetTextureOffset(pn, t.GetOffset());
            });
        }
    }

    public static Dictionary<string, System.Func<PrefsVector4, string, bool>> customVectorGUI = new Dictionary<string, System.Func<PrefsVector4, string, bool>>();

    public void DebugMenu(bool labelEnable = true)
    {
        if ((_material != null) && _propertySet.Any())
        {
            if (labelEnable) GUILayout.Label(_material.name);
            GUIUtil.Indent(() =>
            {
                _colors.ForEach(c => c.OnGUISlider(KeyToPropertyName(c.key)));
                _vectors.ForEach(v =>
                {
                    var n = KeyToPropertyName(v.key);
                    if (customVectorGUI.ContainsKey(n))
                    {
                        customVectorGUI[n](v, n);
                    }
                    else {
                        v.OnGUISlider(n);
                    }
                });
                _floats.ForEach(f => f.OnGUISlider(KeyToPropertyName(f.key)));
                _ranges.ForEach(range =>
                {
                    var n = KeyToPropertyName(range.key);
                    var mr = _propertySet.ranges.Find(r => r.name == n);

                    range.OnGUISlider(mr.min, mr.max, n);
                });

                _texEnvs.ForEach(t =>
                {
                    var label = KeyToPropertyName(t.key);
                    t.OnGUISlider(Vector4.zero, new Vector4(10, 10, 1, 1), label, new[] { "Tiling.x", "Tiling.y", "Offset.x", "Offset.y" });
                });
            });

            UpdateMaterial();
        }
    }
}
