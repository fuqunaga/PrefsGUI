using System;
using System.Collections;
using System.Collections.Generic;
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

        public static bool update { get; set; }

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

        string keyPrefix
        {
            get
            {
                Assert.IsNotNull(_material);
                return $"{_material.name}-";
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
#endif
        }

        public void UpdateMaterial()
        {
            if (IsEnable)
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
                _ints.ForEach(i => _material.SetInt(KeyToPropertyName(i.key), i.Get()));
            }
        }

        public static Dictionary<string, Func<PrefsVector4, string, bool>> customVectorGUI = new();
    }
}