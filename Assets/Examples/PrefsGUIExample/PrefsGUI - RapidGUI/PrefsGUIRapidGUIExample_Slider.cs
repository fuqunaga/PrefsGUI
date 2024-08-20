using PrefsGUI.Example;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI.Example
{
    public class PrefsGUIRapidGUIExample_Slider : MonoBehaviour, IDoGUI
    {
        public PrefsGUIExample_Primitive primitive;

        public void DoGUI()
        {
            // primitive.prefsBool.DoGUI();
            primitive.prefsInt.DoGUISlider();
            primitive.prefsFloat.DoGUISlider();
            // primitive.prefsString.DoGUISlider();
            // primitive.prefsEnum.DoGUISlider();
            // primitive.prefsColor.DoGUISlider();
            // prefsGradient.DoGUISlider(); // not supported
            primitive.prefsVector2.DoGUISlider();
            primitive.prefsVector3.DoGUISlider();
            primitive.prefsVector4.DoGUISlider();
            primitive.prefsVector2Int.DoGUISlider();
            primitive.prefsVector3Int.DoGUISlider();
            primitive.prefsRect.DoGUISlider();
            primitive.prefsBounds.DoGUISlider();
            primitive.prefsBoundsInt.DoGUISlider();
            // primitive.prefsClass.DoGUISlider();
            // primitive.prefsList.DoGUISlider();
        }
    }
}