﻿using System;
using UnityEngine;

namespace PrefsGUI
{
    [Serializable]
    public class PrefsMinMaxInt : PrefsMinMax<int, PrefsMinMaxInt.MinMax>
    {
        [Serializable]
        public class MinMax : MinMaxBase { }

        public PrefsMinMaxInt(string key, int defaultValueMax = 0) : base(key, defaultValueMax) { }
        public PrefsMinMaxInt(string key, int defaultValueMin, int defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override int defaultMin => 0;
        public override int defaultMax => 100;
    }

    [Serializable]
    public class PrefsMinMaxFloat : PrefsMinMax<float, PrefsMinMaxFloat.MinMax>
    {
        [Serializable]
        public class MinMax : MinMaxBase { }

        public PrefsMinMaxFloat(string key, float defaultValueMax = 0f) : base(key, defaultValueMax) { }
        public PrefsMinMaxFloat(string key, float defaultValueMin, float defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override float defaultMin => 0f;
        public override float defaultMax => 1f;
    }

    [Serializable]
    public class PrefsMinMaxVector2 : PrefsMinMaxVector<Vector2, PrefsMinMaxVector2.MinMax>
    {
        [Serializable]
        public class MinMax : MinMaxBase { }

        public PrefsMinMaxVector2(string key, Vector2 defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxVector2(string key, Vector2 defaultValueMin, Vector2 defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }
    }

    [Serializable]
    public class PrefsMinMaxVector3 : PrefsMinMaxVector<Vector3, PrefsMinMaxVector3.MinMax>
    {
        [Serializable]
        public class MinMax : MinMaxBase { }

        public PrefsMinMaxVector3(string key, Vector3 defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxVector3(string key, Vector3 defaultValueMin, Vector3 defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }
    }

    [Serializable]
    public class PrefsMinMaxVector4 : PrefsMinMaxVector<Vector4, PrefsMinMaxVector4.MinMax>
    {
        [Serializable]
        public class MinMax : MinMaxBase { }

        public PrefsMinMaxVector4(string key, Vector4 defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxVector4(string key, Vector4 defaultValueMin, Vector4 defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }
    }

    [Serializable]
    public class PrefsMinMaxVector2Int : PrefsMinMaxVector<Vector2Int, PrefsMinMaxVector2Int.MinMax>
    {
        [Serializable]
        public class MinMax : MinMaxBase { }

        public PrefsMinMaxVector2Int(string key, Vector2Int defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxVector2Int(string key, Vector2Int defaultValueMin, Vector2Int defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override Vector2Int defaultMax => base.defaultMax * 100;
    }

    [Serializable]
    public class PrefsMinMaxVector3Int : PrefsMinMaxVector<Vector3Int, PrefsMinMaxVector3Int.MinMax>
    {
        [Serializable]
        public class MinMax : MinMaxBase { }

        public PrefsMinMaxVector3Int(string key, Vector3Int defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxVector3Int(string key, Vector3Int defaultValueMin, Vector3Int defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override Vector3Int defaultMax => base.defaultMax * 100;
    }

    [Serializable]
    public class PrefsMinMaxRect : PrefsMinMax<Rect, PrefsMinMaxRect.MinMax>
    {
        [Serializable]
        public class MinMax : MinMaxBase { }

        public PrefsMinMaxRect(string key, Rect defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxRect(string key, Rect defaultValueMin, Rect defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override Rect defaultMin => default;
        public override Rect defaultMax => new Rect(1f, 1f, 1f, 1f);

        public bool DoGUISlider(float max, string label = null)
        {
            return this.DoGUISlider(new Rect(max, max, max, max), label);
        }
    }

    [Serializable]
    public class PrefsMinMaxBounds : PrefsMinMax<Bounds, PrefsMinMaxBounds.MinMax>
    {
        [Serializable]
        public class MinMax : MinMaxBase { }

        public PrefsMinMaxBounds(string key, Bounds defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxBounds(string key, Bounds defaultValueMin, Bounds defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override Bounds defaultMin => default;
        public override Bounds defaultMax => new Bounds(Vector3.one, Vector3.one);

        public bool DoGUISlider(float max, string label = null)
        {
            return this.DoGUISlider(new Bounds(Vector3.one * max, Vector3.one * max), label);
        }
    }

    [Serializable]
    public class PrefsMinMaxBoundsInt : PrefsMinMax<BoundsInt, PrefsMinMaxBoundsInt.MinMax>
    {
        [Serializable]
        public class MinMax : MinMaxBase { }

        public PrefsMinMaxBoundsInt(string key, BoundsInt defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxBoundsInt(string key, BoundsInt defaultValueMin, BoundsInt defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override BoundsInt defaultMin => default;
        public override BoundsInt defaultMax => new BoundsInt(Vector3Int.one * 100, Vector3Int.one * 100);

        public bool DoGUISlider(int max, string label = null)
        {
            return this.DoGUISlider(new BoundsInt(Vector3Int.one * max, Vector3Int.one * max), label);
        }
    }
}