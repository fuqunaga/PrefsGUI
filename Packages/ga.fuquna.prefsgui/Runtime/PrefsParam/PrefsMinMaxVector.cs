using System;

namespace PrefsGUI
{
    public abstract class PrefsMinMaxVector<T> : PrefsMinMax<T>
    {
        protected PrefsMinMaxVector(string key, T defaultValueMax = default) : base(key, defaultValueMax) { }

        protected PrefsMinMaxVector(string key, T defaultValueMin, T defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        static readonly Lazy<T> zero = new(() => (T)typeof(T).GetProperty("zero").GetValue(null));
        static readonly Lazy<T> one = new(() => (T)typeof(T).GetProperty("one").GetValue(null));


        public override T defaultMin => zero.Value;
        public override T defaultMax => one.Value;
    }
}