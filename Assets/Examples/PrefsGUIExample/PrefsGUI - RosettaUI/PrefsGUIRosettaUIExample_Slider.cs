using PrefsGUI.Example;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.RosettaUI.Example
{
    public class PrefsGUIRosettaUIExample_Slider : MonoBehaviour, IElementCreator
    {
        public PrefsGUIExample_Primitive primitive;
        
        public Element CreateElement(LabelElement label)
        {
            return UI.Column(
                // example.prefsBool.CreateSlider(),
                primitive.prefsInt.CreateSlider(),
                primitive.prefsFloat.CreateSlider(),
                // example.prefsString.CreateSlider(),
                // example.prefsEnum.CreateSlider(),
                // example.prefsColor.CreateSlider(),
                // example.prefsGradient.CreateSlider(),
                primitive.prefsVector2.CreateSlider(),
                primitive.prefsVector3.CreateSlider(),
                primitive.prefsVector4.CreateSlider(),
                primitive.prefsVector2Int.CreateSlider(),
                primitive.prefsVector3Int.CreateSlider(),
                primitive.prefsRect.CreateSlider(),
                primitive.prefsBounds.CreateSlider(),
                primitive.prefsBoundsInt.CreateSlider()//,
                // example.prefsClass.CreateSlider(),
                // example.prefsList.CreateSlider()
            );
        }
    }
}