using System;
using PrefsGUI.Utility;

namespace PrefsGUI
{
    /// <summary>
    /// PrefsParam for user type that has new(), serializable by Unity(JsonUtility)
    /// </summary>
    [Serializable]
    public class PrefsAny<TOuter> : PrefsParamOuterInner<TOuter, string> 
        where TOuter : new()
    {
        public PrefsAny(string key, TOuter defaultValue = default) : base(key, defaultValue)
        {
            this.defaultValue ??= new TOuter();
        }

        protected override string ToInner(TOuter outerV) => PrefsAnyUtility.ToInner(outerV);

        protected override TOuter ToOuter(string innerV) => PrefsAnyUtility.ToOuter<TOuter>(innerV);
    }

    public static class PrefsAnyUtility
    {
        public static string ToInner<TOuter>(TOuter outerV) => (outerV == null) ? "" : JsonUtilityEx.ToJson(outerV);
        public static TOuter ToOuter<TOuter>(string innerV) => string.IsNullOrEmpty(innerV) ? default : JsonUtilityEx.FromJson<TOuter>(innerV);
        public static bool IsEqual<TOuter>(TOuter lhs, TOuter rhs)
            =>ToInner(lhs) == PrefsAnyUtility.ToInner(rhs);
    }
}