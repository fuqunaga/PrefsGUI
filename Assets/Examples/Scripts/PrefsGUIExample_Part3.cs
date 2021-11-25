using PrefsGUI.RosettaUI;
using RapidGUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIExample_Part3 : MonoBehaviour, IDoGUI, IElementCreator
    {
        // define PrefsParams with key.
        public PrefsMinMaxInt prefsMinMaxInt = new PrefsMinMaxInt("PrefsMinMaxInt");
        public PrefsMinMaxFloat prefsMinMaxFloat = new PrefsMinMaxFloat("PrefsMinMaxFloat");
        public PrefsMinMaxVector2 prefsMinMaxVector2 = new PrefsMinMaxVector2("PrefsMinMaxVector2");
        public PrefsMinMaxVector3 prefsMinMaxVector3 = new PrefsMinMaxVector3("PrefsMinMaxVector3");
        public PrefsMinMaxVector4 prefsMinMaxVector4 = new PrefsMinMaxVector4("PrefsMinMaxVector4");
        public PrefsMinMaxVector2Int prefsMinMaxVector2Int = new PrefsMinMaxVector2Int("PrefsMinMaxVector2Int");
        public PrefsMinMaxVector3Int prefsMinMaxVector3Int = new PrefsMinMaxVector3Int("PrefsMinMaxVector3Int");

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

        void Update()
        {
            TestImplicitCast();
        }

        protected void TestImplicitCast()
        {
            MinMaxInt minMaxInt = prefsMinMaxInt;
            MinMaxFloat minMaxFloat = prefsMinMaxFloat;
            MinMaxVector2 minMaxVector2 = prefsMinMaxVector2;
            MinMaxVector3 minMaxVector3 = prefsMinMaxVector3;
            MinMaxVector4 minMaxVector4 = prefsMinMaxVector4;
            MinMaxVector2Int minMaxVector2Int = prefsMinMaxVector2Int;
            MinMaxVector3Int minMaxVector3Int = prefsMinMaxVector3Int;
        }

        public Element CreateElement()
        {
            return UI.Column(
                prefsMinMaxInt.CreateSlider(10),
                prefsMinMaxFloat.CreateSlider(),
                prefsMinMaxVector2.CreateSlider(),
                prefsMinMaxVector3.CreateSlider(),
                prefsMinMaxVector4.CreateSlider(),
                prefsMinMaxVector2Int.CreateSlider(),
                prefsMinMaxVector3Int.CreateSlider()
            );
        }
    }
}