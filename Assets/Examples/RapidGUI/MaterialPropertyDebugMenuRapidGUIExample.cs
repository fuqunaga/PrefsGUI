using PrefsGUI.RapidGUI;

namespace PrefsGUI.Example
{
    public class MaterialPropertyDebugMenuRapidGUIExample : PrefsGUIRapidGUIExampleBase
    {
        public MaterialPropertyDebugMenu debugMenu;

        public void Start()
        {
            if (debugMenu == null)
            {
                debugMenu = GetComponent<MaterialPropertyDebugMenu>();
            }
        }

        protected override void DoGUI()
        {
            if (debugMenu != null)
            {
                debugMenu.DoGUI();
            }

            base.DoGUI();
        }
    }
}