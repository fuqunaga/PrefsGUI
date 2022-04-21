using System;
using UnityEngine;

namespace PrefsGUI
{
    [Serializable]
    public class PrefsMinMaxInt : PrefsMinMax<int>
    {
        public PrefsMinMaxInt(string key, int defaultValueMax = 0) : base(key, defaultValueMax) { }
        public PrefsMinMaxInt(string key, int defaultValueMin, int defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override int defaultMin => 0;
        public override int defaultMax => 100;
    }

    [Serializable]
    public class PrefsMinMaxFloat : PrefsMinMax<float>
    {
        public PrefsMinMaxFloat(string key, float defaultValueMax = 0f) : base(key, defaultValueMax) { }
        public PrefsMinMaxFloat(string key, float defaultValueMin, float defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override float defaultMin => 0f;
        public override float defaultMax => 1f;
    }

    [Serializable]
    public class PrefsMinMaxVector2 : PrefsMinMaxVector<Vector2>
    {
        public PrefsMinMaxVector2(string key, Vector2 defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxVector2(string key, Vector2 defaultValueMin, Vector2 defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }
    }

    [Serializable]
    public class PrefsMinMaxVector3 : PrefsMinMaxVector<Vector3>
    {
        public PrefsMinMaxVector3(string key, Vector3 defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxVector3(string key, Vector3 defaultValueMin, Vector3 defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }
    }

    [Serializable]
    public class PrefsMinMaxVector4 : PrefsMinMaxVector<Vector4>
    {
        public PrefsMinMaxVector4(string key, Vector4 defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxVector4(string key, Vector4 defaultValueMin, Vector4 defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }
    }

    [Serializable]
    public class PrefsMinMaxVector2Int : PrefsMinMaxVector<Vector2Int>
    {
        public PrefsMinMaxVector2Int(string key, Vector2Int defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxVector2Int(string key, Vector2Int defaultValueMin, Vector2Int defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override Vector2Int defaultMax => base.defaultMax * 100;
    }

    [Serializable]
    public class PrefsMinMaxVector3Int : PrefsMinMaxVector<Vector3Int>
    {
        public PrefsMinMaxVector3Int(string key, Vector3Int defaultValueMax = default) : base(key, defaultValueMax) { }
        public PrefsMinMaxVector3Int(string key, Vector3Int defaultValueMin, Vector3Int defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        public override Vector3Int defaultMax => base.defaultMax * 100;
    }
}