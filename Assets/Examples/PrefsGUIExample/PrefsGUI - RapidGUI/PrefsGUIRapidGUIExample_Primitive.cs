using PrefsGUI.Example;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI.Example
{
    public class PrefsGUIRapidGUIExample_Primitive : MonoBehaviour, IDoGUI
    {
        public PrefsGUIExample_Primitive primitive;

        public void DoGUI()
        {
            primitive.prefsBool.DoGUI();

            // Return true if value was changed
            var changed = primitive.prefsInt.DoGUI();
            if (changed)
            {
                // Implicitly convert
                int intValue = primitive.prefsInt;
                Debug.Log("DoGUI: Changed. " + intValue);
            }

            primitive.prefsFloat.DoGUI();
            primitive.prefsString.DoGUI();
            primitive.prefsEnum.DoGUI();
            primitive.prefsColor.DoGUI();
            // prefsGradient.DoGUI(); // not supported
            primitive.prefsVector2.DoGUI();
            primitive.prefsVector3.DoGUI();
            primitive.prefsVector4.DoGUI();
            primitive.prefsVector2Int.DoGUI();
            primitive.prefsVector3Int.DoGUI();
            primitive.prefsRect.DoGUI();
            primitive.prefsBounds.DoGUI();
            primitive.prefsBoundsInt.DoGUI();
            primitive.prefsClass.DoGUI();
            primitive.prefsList.DoGUI();

            if (primitive.prefsList.Count > 0)
            {
                primitive.prefsList.DoGUIAt(0, "PrefsList element at 0");
            }
            
            primitive.prefsIPEndPoint.DoGUI();
        }
    }
}