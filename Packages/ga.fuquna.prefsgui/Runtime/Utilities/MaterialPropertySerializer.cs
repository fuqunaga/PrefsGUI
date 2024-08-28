using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PrefsGUI.Utility
{
    [ExecuteAlways]
    public class MaterialPropertySerializer : MonoBehaviour
    {
        #region TypeDefine

        [Serializable]
        public class PropertySet
        {
            [Serializable]
            public class NameAndDescription
            {
                public string name;
                public string description;
            }
            
            [Serializable]
            public class RangeData
            {
                public string name;
                public float min;
                public float max;
            }

            public List<string> colors = new();
            public List<string> vectors = new();
            public List<string> floats = new();
            public List<RangeData> ranges = new();
            public List<string> texEnvs = new();
            public List<string> ints = new();

            public List<NameAndDescription> propertyNameToDescription = new();

            private Dictionary<string, string> _propertyNameToDescription;

            public bool Any() => colors.Any() || vectors.Any() || floats.Any() || ranges.Any() || texEnvs.Any() ||
                                 ints.Any();

            public void Clear()
            {
                colors.Clear();
                vectors.Clear();
                floats.Clear();
                ranges.Clear();
                texEnvs.Clear();
                ints.Clear();
                
                propertyNameToDescription.Clear();
            }

            public string GetDescription(string name)
            {
                _propertyNameToDescription ??= propertyNameToDescription.ToDictionary(
                    nameAndDescription => nameAndDescription.name,
                    nameAndDescription => nameAndDescription.description
                );

                return _propertyNameToDescription.TryGetValue(name, out var description) ? description : null;
            }
        }

        #endregion

        public Material _material;
        public bool _ignoreTexEnv = true;
        public List<string> _ignoreProperties;
        public PropertySet _propertySet = new PropertySet();


        protected virtual void OnValidate()
        {
            UpdatePropertySet();
        }

        void UpdatePropertySet()
        {
#if UNITY_EDITOR
            _propertySet.Clear();
            if (_material == null) return;
            
            var shader = _material.shader;
            var count = ShaderUtil.GetPropertyCount(shader);
            for (var i = 0; i < count; ++i)
            {
                var propertyName = ShaderUtil.GetPropertyName(shader, i);
                
                
                if (!_ignoreProperties.Contains(propertyName))
                {
                    var propertyDescription = ShaderUtil.GetPropertyDescription(shader, i);
                    _propertySet.propertyNameToDescription.Add(new (){name = propertyName, description = propertyDescription});
                    
                    var type = ShaderUtil.GetPropertyType(shader, i);
                    switch (type)
                    {
                        case ShaderUtil.ShaderPropertyType.Color:  _propertySet.colors.Add(propertyName); break;
                        case ShaderUtil.ShaderPropertyType.Vector: _propertySet.vectors.Add(propertyName); break;
                        case ShaderUtil.ShaderPropertyType.Float:  _propertySet.floats.Add(propertyName); break;
                        case ShaderUtil.ShaderPropertyType.Range:
                        {
                            var rangeData = new PropertySet.RangeData()
                            {
                                name = propertyName,
                                min = ShaderUtil.GetRangeLimits(shader, i, 1),
                                max = ShaderUtil.GetRangeLimits(shader, i, 2),
                            };
                            _propertySet.ranges.Add(rangeData);
                        }
                            break;

                        case ShaderUtil.ShaderPropertyType.TexEnv:
                        {
                            if (!_ignoreTexEnv && _material.GetTexture(propertyName) != null)
                            {
                                _propertySet.texEnvs.Add(propertyName);
                            }
                        }
                            break;
                        case ShaderUtil.ShaderPropertyType.Int:
                            _propertySet.ints.Add(propertyName);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
#endif
        }
    }
}
