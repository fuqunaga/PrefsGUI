using PrefsGUI.Example;
using RosettaUI;
using RosettaUI.UIToolkit;

namespace PrefsGUI.RosettaUI.Example
{
    public class TestRosettaUI : RosettaUIRootUIToolkit
    {
        private void Start()
        {
            var window = UI.Window(
                "RosettaUI",
                UI.WindowLauncher<PrefsGUIExample_Part1>("Part1"),
                UI.WindowLauncher<PrefsGUIExample_Part2>("Part2"),
                UI.WindowLauncher<PrefsGUIExample_Part3>("Part3"),
                UI.WindowLauncher(UI.Window(nameof(PrefsSearch), PrefsSearch.CreateElement()))
            );

            Build(window);
        }
    }
}