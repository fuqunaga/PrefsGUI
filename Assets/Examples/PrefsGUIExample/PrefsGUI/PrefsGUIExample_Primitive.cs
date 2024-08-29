using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIExample_Primitive : MonoBehaviour
    {
        public PrefsBool              prefsBool       = new("PrefsBool");
        public PrefsInt               prefsInt        = new("PrefsInt");
        public PrefsFloat             prefsFloat      = new("PrefsFloat");
        public PrefsString            prefsString     = new("PrefsString");
        public PrefsParam<EnumSample> prefsEnum       = new("PrefsEnum");
        public PrefsColor             prefsColor      = new("PrefsColor");
        public PrefsGradient          prefsGradient   = new("PrefsGradient");
        public PrefsVector2           prefsVector2    = new("PrefsVector2");
        public PrefsVector3           prefsVector3    = new("PrefsVector3");
        public PrefsVector4           prefsVector4    = new("PrefsVector4");
        public PrefsVector2Int        prefsVector2Int = new("PrefsVector2Int");
        public PrefsVector3Int        prefsVector3Int = new("PrefsVector3Int");
        public PrefsRect              prefsRect       = new("PrefsRect");
        public PrefsBounds            prefsBounds     = new("PrefsBounds");
        public PrefsBoundsInt         prefsBoundsInt  = new("PrefsBoundsInt");
        public PrefsAny<CustomClass>  prefsClass      = new("PrefsClass");
        public PrefsList<CustomClass> prefsList       = new("PrefsList");
        public PrefsIPAddressAndPort  prefsIPEndPoint = new("PrefsIPAddressAndPort");
    }
}