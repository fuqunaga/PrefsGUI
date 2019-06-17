namespace PrefsGUI
{

    public class MaterialPropertyDebugMenuExample : PrefsGUIExampleBase
    {
        MaterialPropertyDebugMenu debugMenu;

        public void Start()
        {
            debugMenu = GetComponent<MaterialPropertyDebugMenu>();
        }

        protected override void DoGUI()
        {
            debugMenu.DoGUI();
        }
    }
}