using UnityEngine;
using MaterialPropertyAccessor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using PrefsGUI;


[ExecuteInEditMode]
public class MaterialPropertyDebugMenu : MaterialPropertyBehaviour
{
    [SerializeField]
    List<PrefsVector4> _colors = new List<PrefsVector4>();
    [SerializeField]
    List<PrefsVector4> _vectors = new List<PrefsVector4>();
    [SerializeField]
    List<PrefsFloat> _floats = new List<PrefsFloat>();
    [SerializeField]
    List<PrefsFloat> _ranges = new List<PrefsFloat>();

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

#if UNITY_EDITOR
    public override void Update()
    {
        base.Update();
        UpdatePrefs();
    }
#endif

    void UpdatePrefs()
    {
        var colorKeys  = _propertySet.colors.Select(name => PropertyNameToKey(name)).ToList();
        var vectorKeys = _propertySet.vectors.Select(name => PropertyNameToKey(name)).ToList();
        var floatKeys  = _propertySet.floats.Select(name => PropertyNameToKey(name)).ToList();
        var rangeKeys  = _propertySet.ranges.Select(range => PropertyNameToKey(range.name)).ToList();

        _colors.RemoveAll ( c => !colorKeys.Contains  ( c.key )) ;
        _vectors.RemoveAll( v => !vectorKeys.Contains ( v.key )) ;
        _floats.RemoveAll ( f => !floatKeys.Contains  ( f.key )) ;
        _ranges.RemoveAll ( r => !rangeKeys.Contains  ( r.key )) ;

        _colors.AddRange (colorKeys.Except (_colors.Select (c => c.key)).Select(n => new PrefsVector4(n, _material.GetColor(KeyToPropertyName(n)))));
        _vectors.AddRange(vectorKeys.Except(_vectors.Select(v => v.key)).Select(n => new PrefsVector4(n, _material.GetVector(KeyToPropertyName(n)))));
        _floats.AddRange (floatKeys.Except (_floats.Select (f => f.key)).Select(n => new PrefsFloat  (n, _material.GetFloat(KeyToPropertyName(n)))));
        _ranges.AddRange (rangeKeys.Except (_ranges.Select (r => r.key)).Select(n => new PrefsFloat  (n, _material.GetFloat(KeyToPropertyName(n)))));
    }

    void UpdateMaterial()
    {
        if ((_material != null) && _propertySet.Any())
        {
            _colors.ForEach(c => _material.SetColor(KeyToPropertyName(c.key), c.Get()));
            _vectors.ForEach(v => _material.SetVector(KeyToPropertyName(v.key), v.Get()));
            _floats.ForEach(f => _material.SetFloat(KeyToPropertyName(f.key), f.Get()));
            _ranges.ForEach(r => _material.SetFloat(KeyToPropertyName(r.key), r.Get()));
        }
    }

    public static Dictionary<string, System.Action<PrefsVector4, string>> customVectorGUI = new Dictionary<string, System.Action<PrefsVector4, string>>();
    
    public void DebugMenu()
    {
        if ((_material != null) && _propertySet.Any())
        {
            GUILayout.Label(_material.name);
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
            });

            UpdateMaterial();
        }
    }
}
