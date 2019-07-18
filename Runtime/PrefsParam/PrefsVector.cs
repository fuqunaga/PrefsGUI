using System;

namespace PrefsGUI
{
    public abstract class PrefsVector<T> : PrefsSlider<T>
    {
        public PrefsVector(string key, T defaultValue = default) : base(key, defaultValue) { }

        static Lazy<T> zero = new Lazy<T>(() => (T)typeof(T).GetProperty("zero").GetValue(null));
        static Lazy<T> one = new Lazy<T>(() => (T)typeof(T).GetProperty("one").GetValue(null));

        public override T defaultMin => zero.Value;
        public override T defaultMax => one.Value;
    }
}