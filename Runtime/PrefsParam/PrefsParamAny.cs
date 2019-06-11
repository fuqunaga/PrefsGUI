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
        static Lazy<XmlSerializer> serializer = new Lazy<XmlSerializer>(() => new XmlSerializer(typeof(OuterT)));

        public PrefsParamAny(string key, OuterT defaultValue = default) : base(key, defaultValue)
        {
        }

        protected override string ToInner(OuterT outerV)
        {
            if (outerV == null) return "";

            using (var writer = new StringWriter())
            {
                serializer.Value.Serialize(writer, outerV);
                return writer.ToString();
            }
        }

        protected override OuterT ToOuter(string innerV)
        {
            if (!string.IsNullOrEmpty(innerV))
            {
                using (var reader = new StringReader(innerV))
                {
                    try
                    {
                        return (OuterT)serializer.Value.Deserialize(reader);
                    }
                    catch { }
                }
            }
            return default;
        }

        public override bool DoGUI(string label = null)
        {
            return DoGUIStrandard((v) => ToInner(RGUI.Field(Get(), label ?? key)));
        }
    }
}