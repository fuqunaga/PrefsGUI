using RapidGUI;

namespace PrefsGUI
{
    public abstract class PrefsMinMax<T, MinMaxT> : PrefsAny<MinMaxT>, IPrefsSlider<T>   
        where MinMaxT : PrefsMinMax<T, MinMaxT>.MinMaxBase, new()
    {
        // can't use directly because unity don't serialize generic type
        public abstract class MinMaxBase
        {
            public T min;
            public T max;
        }

        public T min => Get().min;
        public T max => Get().max;

        public PrefsMinMax(string key, T defaultValueMax = default) : this(key, default, defaultValueMax) { }
        public PrefsMinMax(string key, T defaultValueMin,  T defaultValueMax) : base(key, new MinMaxT() { min = defaultValueMin, max = defaultValueMax }) { }


        #region IPrefSlider

        public abstract T defaultMin { get; }

        public abstract T defaultMax { get; }

        public bool DoGUISlider(T rangeMin, T rangeMax, string label = null)
        {
            return DoGUIStrandard((v) =>
            {
                var minMax = ToOuter(v);
                RGUI.MinMaxSlider(ref minMax.min, ref minMax.max, rangeMin, rangeMax, label ?? key);
                return ToInner(minMax);
            });
        }

        #endregion
    }
}