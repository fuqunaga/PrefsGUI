using UnityEngine;
using System.Collections;

namespace MaterialPropertyAccessor
{

    public class MaterialPropertyGUISample : MaterialPropertyBehaviour
    {
        void OnGUI()
        {
            if (_material != null && _propertySet != null)
            {
                _propertySet.colors.ForEach(name =>
                {
                    var c = _material.GetColor(name);

                    using (var h = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(name);
                        for (var i = 0; i < 4; ++i)
                        {
                            c[i] = float.Parse(GUILayout.TextField(c[i].ToString()));
                        }
                    }

                    _material.SetColor(name, c);
                });

                _propertySet.vectors.ForEach(name =>
                {
                    var v = _material.GetVector(name);

                    using (var h = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(name);
                        for (var i = 0; i < 4; ++i)
                        {
                            v[i] = float.Parse(GUILayout.TextField(v[i].ToString()));
                        }
                    }

                    _material.SetVector(name, v);
                });

                _propertySet.floats.ForEach(name =>
                {
                    var v = _material.GetFloat(name);

                    using (var h = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(name);
                        v = float.Parse(GUILayout.TextField(v.ToString()));
                    }

                    _material.SetFloat(name, v);
                });

                _propertySet.ranges.ForEach(range =>
                {
                    var name = range.name;
                    var v = _material.GetFloat(name);

                    using (var h = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(name);
                        v = float.Parse(GUILayout.TextField(v.ToString()));
                    }

                    _material.SetFloat(name, v);
                });

                _propertySet.texEnvs.ForEach(name =>
                {
                    var tiling = _material.GetTextureScale(name);
                    var offset = _material.GetTextureOffset(name);

                    GUILayout.Label(name);
                    using (var h = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Tiling");
                        tiling.x = float.Parse(GUILayout.TextField(tiling.x.ToString()));
                        tiling.y = float.Parse(GUILayout.TextField(tiling.y.ToString()));
                    }

                    using (var h = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Offset");
                        offset.x = float.Parse(GUILayout.TextField(offset.x.ToString()));
                        offset.y = float.Parse(GUILayout.TextField(offset.y.ToString()));
                    }

                    _material.SetTextureScale(name, tiling);
                    _material.SetTextureOffset(name, offset);
                });
            }
        }
    }

}