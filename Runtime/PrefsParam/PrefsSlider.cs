using RapidGUI;

namespace PrefsGUI
{
    public abstract class PrefsSlider<T> : PrefsSlider<T,T>
    {
        public PrefsSlider(string key, T defaultValue = default) : base(key, defaultValue)
        {
        }

        protected override T ToInner(T outerV) => outerV;
        protected override T ToOuter(T innerV) => innerV;
    }


    public abstract class PrefsSlider<OuterT, InnerT> : PrefsParamOuterInner<OuterT, InnerT>
    {
        protected bool isOpen;


        #region abstract

        protected abstract InnerT defaultMin { get; }
        protected abstract InnerT defaultMax { get; }

        #endregion abstract



        public PrefsSlider(string key, OuterT defaultValue = default) : base(key, defaultValue) { }

        public bool DoGUISlider(string label = null)
        {
            return DoGUISlider(defaultMin, defaultMax, label);
        }

        public bool DoGUISlider(OuterT min, OuterT max, string label = null)
        {
            return DoGUISlider(ToInner(min), ToInner(max), label);
        }

        protected bool DoGUISlider(InnerT min, InnerT max, string label = null)
        {
            return DoGUIStrandard((InnerT v) => RGUI.Slider(v, min, max, label ?? key, ref isOpen));
        }
    }
}