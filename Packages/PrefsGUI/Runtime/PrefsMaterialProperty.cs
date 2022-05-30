using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PrefsGUI.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace PrefsGUI
{
    [ExecuteAlways]
    public class PrefsMaterialProperty : MaterialPropertySerializer
    {
        #region Type Define

        [Serializable]
        public class PrefsTexEnv : PrefsVector4
        {
            public PrefsTexEnv(string key, Vector2 tiling, Vector2 offset = default) : base(key,
                new Vector4(tiling.x, tiling.y, offset.x, offset.y))
            {
            }

            public Vector2 GetScale()
            {
                var v = Get();
                return new Vector2(v.x, v.y);
            }

            public Vector2 GetOffset()
            {
                var v = Get();
                return new Vector2(v.z, v.w);
            }
        }

        #endregion

        [SerializeField] List<PrefsColor> _colors = new();
        [SerializeField] List<PrefsVector4> _vectors = new();
        [SerializeField] List<PrefsFloat> _floats = new();
        [SerializeField] List<PrefsFloat> _ranges = new();
        [SerializeField] List<PrefsTexEnv> _texEnvs = new();
        [SerializeField] List<PrefsInt> _ints = new();
        

        public List<PrefsColor> Colors => _colors;
        public List<PrefsVector4> Vectors => _vectors;
        public List<PrefsFloat> Floats => _floats;
        public List<PrefsFloat> Ranges => _ranges;
        public List<PrefsTexEnv> TexEnvs => _texEnvs;
        public List<PrefsInt> Ints => _ints;

        public bool IsEnable => _material != null && _propertySet.Any();

        string KeyPrefix
        {
            get
            {
                Assert.IsNotNull(_material);
                return $"{_material.name}-";
            }
        }

        string PropertyNameToKey(string n) => KeyPrefix + n;
        public string KeyToPropertyName(string key) => key.Replace(KeyPrefix, "");

        
        #region Unity
        
        public void Start()
        {
            InitMaterialAndSetPrefsCallback();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            UpdatePrefs();
            InitMaterialAndSetPrefsCallback();
        }
        
        #endregion
        

        [Conditional("UNITY_EDITOR")]
        void UpdatePrefs()
        {
            UpdatePrefsList(_colors, _propertySet.colors,
                (key) => new PrefsColor(key, _material.GetColor(KeyToPropertyName(key)))
            );

            UpdatePrefsList(_vectors, _propertySet.vectors,
                (key) => new PrefsVector4(key, _material.GetVector(KeyToPropertyName(key)))
            );
            
            UpdatePrefsList(_floats, _propertySet.floats,
                (key) => new PrefsFloat(key, _material.GetFloat(KeyToPropertyName(key)))
            );

            UpdatePrefsList(_ranges, _propertySet.ranges.Select(r => r.name),
                (key) => new PrefsFloat(key, _material.GetFloat(KeyToPropertyName(key)))
            );

            UpdatePrefsList(_texEnvs, _propertySet.texEnvs,
                (key) => new PrefsTexEnv(key, _material.GetTextureScale(KeyToPropertyName(key)), _material.GetTextureOffset(KeyToPropertyName(key)))
            );
            
            UpdatePrefsList(_ints, _propertySet.ints,
                (key) => new PrefsInt(key, _material.GetInt(KeyToPropertyName(key)))
            );

            void UpdatePrefsList<T>(List<T> prefsList, IEnumerable<string> propertyNames, Func<string, T> createPrefsFromPropertyName)
                where T : PrefsParam
            {
                using (ListPool<string>.Get(out var currentKeyList))
                using (ListPool<string>.Get(out var newKeyList))
                {
                    currentKeyList.AddRange(prefsList.Select(prefs => prefs.key));
                    newKeyList.AddRange(propertyNames.Select(PropertyNameToKey));

                    if (currentKeyList.SequenceEqual(newKeyList))
                    {
                        return;
                    }

                    var removeKeys = currentKeyList.Except(newKeyList);
                    foreach (var removeKey in removeKeys)
                    {
                        prefsList.RemoveAll(prefs => prefs.key == removeKey);
                    }

                    var newPrefs = newKeyList.Except(currentKeyList).Select(createPrefsFromPropertyName);
                    prefsList.AddRange(newPrefs);
                }
            }
        }

        private void InitMaterialAndSetPrefsCallback()
        {
            if (!IsEnable) return;

            _colors.ForEach(prefs => BindPrefsToMaterialProperty(prefs, _material.SetColor));
            _vectors.ForEach(prefs => BindPrefsToMaterialProperty(prefs, _material.SetVector));
            _floats.ForEach(prefs => BindPrefsToMaterialProperty(prefs, _material.SetFloat));
            _ranges.ForEach(prefs => BindPrefsToMaterialProperty(prefs, _material.SetFloat));
            _texEnvs.ForEach(prefs => BindPrefsToMaterialProperty(prefs, (propertyName, _) =>
            {
                _material.SetTextureScale(propertyName, prefs.GetScale());
                _material.SetTextureOffset(propertyName, prefs.GetOffset());
            }));
            _ints.ForEach(prefs => BindPrefsToMaterialProperty(prefs, _material.SetInt));
            

            void BindPrefsToMaterialProperty<T>(PrefsParamOuter<T> prefs, Action<string, T> setPrefsToMaterial)
            {
                SetPrefsToMaterial();
                prefs.RegisterValueChangedCallback(SetPrefsToMaterial);

                void SetPrefsToMaterial()
                {
                    setPrefsToMaterial.Invoke(KeyToPropertyName(prefs.key), prefs.Get());
                }
            }
        }
    }
}