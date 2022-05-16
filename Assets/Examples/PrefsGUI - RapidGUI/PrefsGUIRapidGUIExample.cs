using PrefsGUI.RapidGUI;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIRapidGUIExample : PrefsGUIRapidGUIExampleBase
    {
        private WindowLaunchers windows;
        
        private void Start()
        {
            windows = new WindowLaunchers
            {
                isWindow = false
            };
            windows.Add("Part1", typeof(PrefsGUIExample_Part1));
            windows.Add("Part2", typeof(PrefsGUIExample_Part2));
            windows.Add("Part3", typeof(PrefsGUIExample_Part3));
            windows.Add("PrefsSearch", PrefsSearch.DoGUI).SetWidth(600f).SetHeight(800f);

            windowRect.y = Screen.height * 0.5f;
        }

        protected override void DoGUI()
        {
            windows.DoGUI();
            base.DoGUI();
        }
    }
}