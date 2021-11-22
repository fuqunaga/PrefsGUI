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
                UI.WindowLauncher<PrefsGUIExample_Part3>("Part3")
            );

            Build(window);
        }

        /*
        windows = new WindowLaunchers();
        windows.isWindow = false;
        windows.Add("Part1", typeof(PrefsGUIExample_Part1));
        windows.Add("Part2", typeof(PrefsGUIExample_Part2));
        windows.Add("Part3", typeof(PrefsGUIExample_Part3));
        windows.Add("PrefsSearch", PrefsSearch.DoGUI).SetWidth(600f).SetHeight(800f);
        */
    }
}