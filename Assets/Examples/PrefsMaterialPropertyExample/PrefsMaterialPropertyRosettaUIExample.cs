using PrefsGUI.RosettaUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    [RequireComponent(typeof(RosettaUIRoot))]
    public class PrefsMaterialPropertyRosettaUIExample : MonoBehaviour
    {
        public PrefsMaterialProperty prefsMaterialProperty;
        public Vector2 position;
        
        public void Start()
        {
            var rosettaUIRoot = GetComponent<RosettaUIRoot>();
            
            rosettaUIRoot.Build(
                UI.Window(
                    prefsMaterialProperty.CreateElement()
                ).SetPosition(position)
            );
        }
    }
}