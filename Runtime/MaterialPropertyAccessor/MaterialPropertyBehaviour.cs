#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MaterialPropertyAccessor
{

    [ExecuteInEditMode]
    public class MaterialPropertyBehaviour : MonoBehaviour
    {
        #region TypeDefine

        [System.Serializable]
        public class PropertySet
        {
            [System.Serializable]
            public class RangeData
            {
                public string name;
                public float min;
                public float max;
            }

            public List<string> colors = new List<string>();
            public List<string> vectors = new List<string>();
            public List<string> floats = new List<string>();
            public List<RangeData> ranges = new List<RangeData>();
            public List<string> texEnvs = new List<string>();

            public bool Any() { return colors.Any() || vectors.Any() || floats.Any() || ranges.Any() || texEnvs.Any(); }
             
            public void Clear()
            {
                colors.Clear();
                vectors.Clear();
                floats.Clear();
                ranges.Clear();
                texEnvs.Clear();
            }
        }

        #endregion

        public Material _material;
        public bool _ignoreTexEnv = true;
        public List<string> _ignoreProperties;
        public PropertySet _propertySet = new PropertySet();



        public virtual void Update()
        {
#if UNITY_EDITOR
            UpdatePropertySet();
#endif
        }

        void UpdatePropertySet()
        {
#if UNITY_EDITOR
            _propertySet.Clear();
            if (_material != null)
            {
                var shader = _material.shader;
                var count = ShaderUtil.GetPropertyCount(shader);
                for (var i = 0; i < count; ++i)
                {
                    var name = ShaderUtil.GetPropertyName(shader, i);
                    if (!_ignoreProperties.Contains(name))
                    {
                        var type = ShaderUtil.GetPropertyType(shader, i);
                        switch (type)
                        {
                            case ShaderUtil.ShaderPropertyType.Color: _propertySet.colors.Add(name); break;
                            case ShaderUtil.ShaderPropertyType.Vector: _propertySet.vectors.Add(name); break;
                            case ShaderUtil.ShaderPropertyType.Float: _propertySet.floats.Add(name); break;
                            case ShaderUtil.ShaderPropertyType.Range:
                                {
                                    var rangeData = new PropertySet.RangeData()
                                    {
                                        name = name,
                                        min = ShaderUtil.GetRangeLimits(shader, i, 1),
                                        max = ShaderUtil.GetRangeLimits(shader, i, 2),
                                    };
                                    _propertySet.ranges.Add(rangeData);
                                }
                                break;

                            case ShaderUtil.ShaderPropertyType.TexEnv:
                                {
                                    if (!_ignoreTexEnv && _material.GetTexture(name) !=null)
                                    {
                                        _propertySet.texEnvs.Add(name);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
#endif
        }
    }
}
