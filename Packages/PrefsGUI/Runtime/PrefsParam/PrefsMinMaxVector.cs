using RapidGUI;
using System;

namespace PrefsGUI
{
    public abstract class PrefsMinMaxVector<T, MinMaxT> : PrefsMinMax<T, MinMaxT>
        //where T : struct
        where MinMaxT : MinMax<T>, new()
    {
        public PrefsMinMaxVector(string key, T defaultValueMax = default) : base(key, defaultValueMax) { }

        public PrefsMinMaxVector(string key, T defaultValueMin, T defaultValueMax) : base(key, defaultValueMin, defaultValueMax) { }

        static Lazy<T> zero = new Lazy<T>(() => (T)typeof(T).GetProperty("zero").GetValue(null));
        static Lazy<T> one = new Lazy<T>(() => (T)typeof(T).GetProperty("one").GetValue(null));


        public override T defaultMin => zero.Value;
        public override T defaultMax => one.Value;
    }
}