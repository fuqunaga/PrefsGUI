namespace PrefsGUI
{
    public abstract class PrefsSlider<T> : PrefsParam<T>, IPrefsSlider<T>
        where T : struct
    {
        protected bool isOpen;
        
        protected PrefsSlider(string key, T defaultValue = default) : base(key, defaultValue) { }


        #region IPrefsSlider

        public abstract T defaultMin { get; }
        public abstract T defaultMax { get; }

        #endregion
    }


    public interface IPrefsSlider<TOuter>
    {
        TOuter defaultMin { get; }
        TOuter defaultMax { get; }
    }
}