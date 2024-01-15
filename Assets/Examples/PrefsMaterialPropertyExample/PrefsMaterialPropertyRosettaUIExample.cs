using System.Collections.Generic;
using System.Linq;
using PrefsGUI.RosettaUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    [RequireComponent(typeof(RosettaUIRoot))]
    public class PrefsMaterialPropertyRosettaUIExample : MonoBehaviour
    {
        public List<PrefsMaterialProperty> prefsMaterialProperties;
        public Vector2 position;
        
        public void Start()
        {
            var rosettaUIRoot = GetComponent<RosettaUIRoot>();
            
            rosettaUIRoot.Build(
                UI.Window(
                    prefsMaterialProperties.Select(pm => pm.CreateElement())
                ).SetPosition(position)
            );
        }
    }
}