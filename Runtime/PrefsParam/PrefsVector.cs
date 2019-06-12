using System;

namespace PrefsGUI
{
    public abstract class PrefsVector<T> : PrefsSlider<T, T>
    {
        public PrefsVector(string key, T defaultValue = default) : base(key, defaultValue) { }

        static Lazy<T> zero = new Lazy<T>(() => (T)typeof(T).GetProperty("zero").GetValue(null));
        static Lazy<T> one = new Lazy<T>(() => (T)typeof(T).GetProperty("one").GetValue(null));

        protected override T defaultMin => zero.Value;
        protected override T defaultMax => one.Value;


        protected override T ToOuter(T innerV) => innerV;
        protected override T ToInner(T TouterV) => TouterV;
    }

}