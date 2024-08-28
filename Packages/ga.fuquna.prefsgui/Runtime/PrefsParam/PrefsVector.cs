using System;

namespace PrefsGUI
{
    public abstract class PrefsVector<T> : PrefsSlider<T>
        where T : struct
    {
        public PrefsVector(string key, T defaultValue = default) : base(key, defaultValue) { }

        static readonly Lazy<T> zero = new(() => (T)typeof(T).GetProperty("zero").GetValue(null));
        static readonly Lazy<T> one = new(() => (T)typeof(T).GetProperty("one").GetValue(null));

        public override T defaultMin => zero.Value;
        public override T defaultMax => one.Value;
    }
}