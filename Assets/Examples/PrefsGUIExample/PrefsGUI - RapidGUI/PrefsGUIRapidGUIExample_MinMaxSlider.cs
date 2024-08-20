using PrefsGUI.Example;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI.Example
{
    public class PrefsGUIRapidGUIExample_MinMaxSlider : MonoBehaviour, IDoGUI
    {
        public PrefsGUIExample_MinMax minMax;

        public void DoGUI()
        {
            minMax.prefsMinMaxInt.DoGUISlider();
            minMax.prefsMinMaxFloat.DoGUISlider();
            minMax.prefsMinMaxVector2.DoGUISlider();
            minMax.prefsMinMaxVector3.DoGUISlider();
            minMax.prefsMinMaxVector4.DoGUISlider();
            minMax.prefsMinMaxVector2Int.DoGUISlider();
            minMax.prefsMinMaxVector3Int.DoGUISlider();
        }
    }
}