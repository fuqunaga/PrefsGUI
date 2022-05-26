using PrefsGUI.RapidGUI;
using UnityEngine.Serialization;

namespace PrefsGUI.Example
{
    public class PrefsMaterialPropertyRapidGUIExample : PrefsGUIRapidGUIExampleBase
    {
        [FormerlySerializedAs("debugMenu")] public PrefsMaterialProperty prefsMaterialProperty;

        public void Start()
        {
            if (prefsMaterialProperty == null)
            {
                prefsMaterialProperty = GetComponent<PrefsMaterialProperty>();
            }
        }

        protected override void DoGUI()
        {
            if (prefsMaterialProperty != null)
            {
                prefsMaterialProperty.DoGUI();
            }

            base.DoGUI();
        }
    }
}