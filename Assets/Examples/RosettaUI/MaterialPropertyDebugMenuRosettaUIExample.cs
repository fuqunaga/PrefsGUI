using PrefsGUI.RosettaUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    [RequireComponent(typeof(RosettaUIRoot))]
    public class MaterialPropertyDebugMenuRosettaUIExample : MaterialPropertyDebugMenuExample
    {
        private RosettaUIRoot _rosettaUIRoot;


        public override void Start()
        {
            base.Start();

            _rosettaUIRoot = GetComponent<RosettaUIRoot>();
            
            _rosettaUIRoot.Build(
                UI.Window(
                    debugMenu.CreateElement()
                )
            );
        }
    }
}