using RapidGUI;
using System;

namespace PrefsGUI
{
    /// <summary>
    /// PrefsParam for user type that has new(), serializable by Unity(JsonUtility)
    /// </summary>
    [Serializable]
    public class PrefsAny<OuterT> : PrefsParamOuterInner<OuterT, string> 
        where OuterT : new()
    {
        public PrefsAny(string key, OuterT defaultValue = default) : base(key, defaultValue)
        {
            if (this.defaultValue == null) this.defaultValue = new OuterT();
        }

        protected override string ToInner(OuterT outerV) => PrefsAnyUtility.ToInner(outerV);

        protected override OuterT ToOuter(string innerV) => PrefsAnyUtility.ToOuter<OuterT>(innerV);

        public override bool DoGUI(string label = null)
        {
            return DoGUIStrandard((v) => RGUI.Field(v, label ?? key));
        }
    }

    public static class PrefsAnyUtility
    {
        public static string ToInner<OuterT>(OuterT outerV) => (outerV == null) ? "" : JsonUtilityEx.ToJson(outerV);
        public static OuterT ToOuter<OuterT>(string innerV) => string.IsNullOrEmpty(innerV) ? default : JsonUtilityEx.FromJson<OuterT>(innerV);
    }
}