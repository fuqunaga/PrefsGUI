using PrefsGUI.RosettaUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    [RequireComponent(typeof(RosettaUIRoot))]
    public class MaterialPropertyDebugMenuRosettaUIExample : MonoBehaviour
    {
        public MaterialPropertyDebugMenu debugMenu;
        
        public void Start()
        {
            var rosettaUIRoot = GetComponent<RosettaUIRoot>();
            
            rosettaUIRoot.Build(
                UI.Window(
                    debugMenu.CreateElement()
                )
            );
        }
    }
}