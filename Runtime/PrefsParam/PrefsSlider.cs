using RapidGUI;

namespace PrefsGUI
{
    public abstract class PrefsSlider<T> : PrefsParam<T>, IPrefsSlider<T>
    {
        protected bool isOpen;


        public PrefsSlider(string key, T defaultValue = default) : base(key, defaultValue) { }

        protected override T ToInner(T outerV) => outerV;
        protected override T ToOuter(T innerV) => innerV;


        #region IPrefsSlider

        public abstract T defaultMin { get; }
        public abstract T defaultMax { get; }

        public virtual bool DoGUISlider(T min, T max, string label = null)
        {
            return DoGUIStrandard((T v) => RGUI.Slider(v, min, max, label ?? key, ref isOpen));
        }

        #endregion
    }


    public interface IPrefsSlider<OuterT>
    {
        OuterT defaultMin { get; }
        OuterT defaultMax { get; }

        bool DoGUISlider(OuterT min, OuterT max, string label = null);
    }

    public static class IPrefsSliderExtension
    {
        public static bool DoGUISlider<OuterT>(this IPrefsSlider<OuterT> me, string label = null)
        {
            return me.DoGUISlider(me.defaultMin, me.defaultMax, label);
        }

        public static bool DoGUISlider<OuterT>(this IPrefsSlider<OuterT> me, OuterT max, string label = null)
        {
            return me.DoGUISlider(me.defaultMin, max, label);
        }
    }
}