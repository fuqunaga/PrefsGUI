using PrefsGUI.Example;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI.Example
{
    public class PrefsGUIRapidGUIExample : PrefsGUIRapidGUIExampleBase
    {
        public Vector2 position;
        private WindowLaunchers windowLaunchers;
        
        private void Start()
        {
            windowLaunchers = new WindowLaunchers
            {
                isWindow = false
            };
            windowLaunchers.Add("Primitive", typeof(PrefsGUIRapidGUIExample_Primitive));
            windowLaunchers.Add("Slider", typeof(PrefsGUIRapidGUIExample_Slider));
            windowLaunchers.Add("MinMaxSlider", typeof(PrefsGUIExample_MinMax));
            windowLaunchers.Add("PrefsSearch", PrefsSearch.DoGUI).SetWidth(600f).SetHeight(800f);

            windowRect.position = position;
        }

        protected override void DoGUI()
        {
            windowLaunchers.DoGUI();
            base.DoGUI();
        }
    }
}