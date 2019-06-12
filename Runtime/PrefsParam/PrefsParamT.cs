namespace PrefsGUI
{
    /// <summary>
    /// Basic PrefsParam same as InnerType and OuterType
    /// </summary>
    public abstract class PrefsParam<T> : PrefsParamOuterInner<T, T>
    {
        public PrefsParam(string key, T defaultValue = default) : base(key, defaultValue) { }

        protected override T ToOuter(T innerV) => innerV;
        protected override T ToInner(T TouterV) => TouterV;
    }
}