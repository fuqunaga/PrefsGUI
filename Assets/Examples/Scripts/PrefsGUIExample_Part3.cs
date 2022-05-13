using PrefsGUI.RapidGUI;
using PrefsGUI.RosettaUI;
using RapidGUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIExample_Part3 : MonoBehaviour, IDoGUI, IElementCreator
    {
        // define PrefsParams with key.
        public PrefsMinMaxInt prefsMinMaxInt = new("PrefsMinMaxInt");
        public PrefsMinMaxFloat prefsMinMaxFloat = new("PrefsMinMaxFloat");
        public PrefsMinMaxVector2 prefsMinMaxVector2 = new("PrefsMinMaxVector2");
        public PrefsMinMaxVector3 prefsMinMaxVector3 = new("PrefsMinMaxVector3");
        public PrefsMinMaxVector4 prefsMinMaxVector4 = new("PrefsMinMaxVector4");
        public PrefsMinMaxVector2Int prefsMinMaxVector2Int = new("PrefsMinMaxVector2Int");
        public PrefsMinMaxVector3Int prefsMinMaxVector3Int = new("PrefsMinMaxVector3Int");

        public void DoGUI()
        {
            prefsMinMaxInt.DoGUISlider();
            prefsMinMaxFloat.DoGUISlider();
            prefsMinMaxVector2.DoGUISlider();
            prefsMinMaxVector3.DoGUISlider();
            prefsMinMaxVector4.DoGUISlider();
            prefsMinMaxVector2Int.DoGUISlider();
            prefsMinMaxVector3Int.DoGUISlider();
        }

        public Element CreateElement(LabelElement _)
        {
            return UI.Column(
                prefsMinMaxInt.CreateMinMaxSlider(),
                prefsMinMaxFloat.CreateMinMaxSlider(),
                prefsMinMaxVector2.CreateMinMaxSlider(),
                prefsMinMaxVector3.CreateMinMaxSlider(),
                prefsMinMaxVector4.CreateMinMaxSlider(),
                prefsMinMaxVector2Int.CreateMinMaxSlider(),
                prefsMinMaxVector3Int.CreateMinMaxSlider()
            );
        }
    }
}