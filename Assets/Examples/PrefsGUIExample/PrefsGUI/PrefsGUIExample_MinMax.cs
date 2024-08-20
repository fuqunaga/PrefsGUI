using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIExample_MinMax : MonoBehaviour
    {
        public PrefsMinMaxInt prefsMinMaxInt = new("PrefsMinMaxInt");
        public PrefsMinMaxFloat prefsMinMaxFloat = new("PrefsMinMaxFloat");
        public PrefsMinMaxVector2 prefsMinMaxVector2 = new("PrefsMinMaxVector2");
        public PrefsMinMaxVector3 prefsMinMaxVector3 = new("PrefsMinMaxVector3");
        public PrefsMinMaxVector4 prefsMinMaxVector4 = new("PrefsMinMaxVector4");
        public PrefsMinMaxVector2Int prefsMinMaxVector2Int = new("PrefsMinMaxVector2Int");
        public PrefsMinMaxVector3Int prefsMinMaxVector3Int = new("PrefsMinMaxVector3Int");
    }
}