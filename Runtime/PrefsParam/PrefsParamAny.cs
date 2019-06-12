using RapidGUI;
using System;
using System.IO;
using System.Xml.Serialization;

namespace PrefsGUI
{
    /// <summary>
    /// PrefsParam for any type
    /// </summary>
    public abstract class PrefsParamAny<OuterT> : PrefsParamOuterInner<OuterT, string>
    {
        public PrefsParamAny(string key, OuterT defaultValue = default) : base(key, defaultValue)
        {
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
            return DoGUIStrandard((v) => ToInner(RGUI.Field(Get(), label ?? key)));
        }
    }
}