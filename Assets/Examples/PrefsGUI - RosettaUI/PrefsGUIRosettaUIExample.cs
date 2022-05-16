using PrefsGUI.Example;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.RosettaUI.Example
{
    [RequireComponent(typeof(RosettaUIRoot))]
    public class PrefsGUIRosettaUIExample : MonoBehaviour
    {
        private void Start()
        {
            var root = GetComponent<RosettaUIRoot>();
            root.Build(CreateElement());
        }

        Element CreateElement()
        {
            return UI.Window(
                "RosettaUI",
                UI.WindowLauncher<PrefsGUIExample_Part1>("Part1"),
                UI.WindowLauncher<PrefsGUIExample_Part2>("Part2"),
                UI.WindowLauncher<PrefsGUIExample_Part3>("Part3"),
                UI.WindowLauncher(UI.Window(nameof(PrefsSearch), PrefsSearch.CreateElement()))
            );

        }
    }
}