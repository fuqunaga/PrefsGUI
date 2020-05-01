using RapidGUI;

namespace PrefsGUI
{
    /// <summary>
    /// PrefsParam for user type that has new(), serializable by Unity(JsonUtility)
    /// </summary>
    public abstract class PrefsAny<OuterT> : PrefsParamOuterInner<OuterT, string> 
        where OuterT : new()
    {
        public PrefsAny(string key, OuterT defaultValue = default) : base(key, defaultValue)
        {
            if (this.defaultValue == null) this.defaultValue = new OuterT();
        }

        protected override string ToInner(OuterT outerV)
        {
            if (outerV == null) return "";

            return JsonUtilityEx.ToJson(outerV);
        }

        protected override OuterT ToOuter(string innerV)
        {
            if (!string.IsNullOrEmpty(innerV))
            {
                return JsonUtilityEx.FromJson<OuterT>(innerV);
            }
            return default;
        }

        public override bool DoGUI(string label = null)
        {
            return DoGUIStrandard((v) => RGUI.Field(v, label ?? key));
        }
    }
}