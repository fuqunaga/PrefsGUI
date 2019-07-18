namespace PrefsGUI
{
    /// <summary>
    /// Basic PrefsParam that has same InnerType, OuterType
    /// </summary>
    public abstract class PrefsParam<T> : PrefsParamOuterInner<T, T>
        where T:struct // if class, that can change devaultValue in DoGUI()
    {
        public PrefsParam(string key, T defaultValue = default) : base(key, defaultValue) { }

        protected override T ToOuter(T innerV) => innerV;
        protected override T ToInner(T TouterV) => TouterV;
    }
}