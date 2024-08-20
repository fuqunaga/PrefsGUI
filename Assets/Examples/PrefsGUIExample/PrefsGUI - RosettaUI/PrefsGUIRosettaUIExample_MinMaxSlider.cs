using PrefsGUI.Example;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.RosettaUI.Example
{
    public class PrefsGUIRosettaUIExample_MinMaxSlider : MonoBehaviour, IElementCreator
    {
        public PrefsGUIExample_MinMax minMax;
        
        public Element CreateElement(LabelElement label)
        {
            return UI.Column(
                minMax.prefsMinMaxInt.CreateMinMaxSlider(),
                minMax.prefsMinMaxFloat.CreateMinMaxSlider(),
                minMax.prefsMinMaxVector2.CreateMinMaxSlider(),
                minMax.prefsMinMaxVector3.CreateMinMaxSlider(),
                minMax.prefsMinMaxVector4.CreateMinMaxSlider(),
                minMax.prefsMinMaxVector2Int.CreateMinMaxSlider(),
                minMax.prefsMinMaxVector3Int.CreateMinMaxSlider()
            );
        }
    }
}