using System;
using System.Collections.Generic;
using System.Linq;
using MaterialPropertyAccessor;
using PrefsGUI;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteAlways]
public class MaterialPropertyDebugMenu : MaterialPropertyBehaviour
{
    #region TypeDefine

    [Serializable]
    public class PrefsTexEnv : PrefsVector4
    {
        public PrefsTexEnv(string key, Vector2 tiling, Vector2 offset = default) : base(key, new Vector4(tiling.x, tiling.y, offset.x, offset.y)) { }
        public Vector2 GetScale() { var v = Get(); return new Vector2(v.x, v.y); }
        public Vector2 GetOffset() { var v = Get(); return new Vector2(v.z, v.w); }

        static Dictionary<string, string> customLabel = new Dictionary<string, string> {
            {"x", "Tiling.x" },
            {"y", "Tiling.y" },
            {"z", "Offset.x" },
            {"w", "Offset.y" }
        };

        public override Dictionary<string, string> GetCustomLabel() => customLabel;
    }

    #endregion

    public static bool update { get; set; }

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

    public List<PrefsColor> Colors => _colors;
    public List<PrefsVector4> Vectors => _vectors;
    public List<PrefsFloat> Floats => _floats;
    public List<PrefsFloat> Ranges => _ranges;
    public List<PrefsTexEnv> TexEnvs => _texEnvs;

    public bool IsEnable => _material != null && _propertySet.Any();

    string keyPrefix
    {
        get
        {
            Assert.IsNotNull(_material);
            return _material.name + "-";
        }
    }

    string PropertyNameToKey(string n) => keyPrefix + n;
    public string KeyToPropertyName(string key) => key.Replace(keyPrefix, "");

    public void Start()
    {
        UpdateMaterial();
    }


    public void Update()
    {
        if (update)
        {
            UpdateMaterial();
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        UpdatePrefs();
    }

    void UpdatePrefs()
    {
#if UNITY_EDITOR
        var colorKeys = _propertySet.colors.Select(PropertyNameToKey).ToList();
        var vectorKeys = _propertySet.vectors.Select(PropertyNameToKey).ToList();
        var floatKeys = _propertySet.floats.Select(PropertyNameToKey).ToList();
        var rangeKeys = _propertySet.ranges.Select(range => PropertyNameToKey(range.name)).ToList();
        var texEnvKeys = _propertySet.texEnvs.Select(PropertyNameToKey).ToList();


        _colors.RemoveAll(c => !colorKeys.Contains(c.key));
        _vectors.RemoveAll(v => !vectorKeys.Contains(v.key));
        _floats.RemoveAll(f => !floatKeys.Contains(f.key));
        _ranges.RemoveAll(r => !rangeKeys.Contains(r.key));
        _texEnvs.RemoveAll(t => !texEnvKeys.Contains(t.key));

        _colors.AddRange(colorKeys.Except(Colors.Select(c => c.key)).Select(n => new PrefsColor(n, _material.GetColor(KeyToPropertyName(n)))));
        _vectors.AddRange(vectorKeys.Except(_vectors.Select(v => v.key)).Select(n => new PrefsVector4(n, _material.GetVector(KeyToPropertyName(n)))));
        _floats.AddRange(floatKeys.Except(_floats.Select(f => f.key)).Select(n => new PrefsFloat(n, _material.GetFloat(KeyToPropertyName(n)))));
        _ranges.AddRange(rangeKeys.Except(_ranges.Select(r => r.key)).Select(n => new PrefsFloat(n, _material.GetFloat(KeyToPropertyName(n)))));
        _texEnvs.AddRange(texEnvKeys.Except(_texEnvs.Select(t => t.key)).Select(n =>
        {
            var pn = KeyToPropertyName(n);
            var tiling = _material.GetTextureScale(pn);
            var offset = _material.GetTextureOffset(pn);
            return new PrefsTexEnv(n, tiling, offset);
        }));
#endif
    }

    public void UpdateMaterial()
    {
        if (IsEnable)
        {
            Colors.ForEach(c => _material.SetColor(KeyToPropertyName(c.key), c.Get()));
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

    public static Dictionary<string, Func<PrefsVector4, string, bool>> customVectorGUI = new();
}
