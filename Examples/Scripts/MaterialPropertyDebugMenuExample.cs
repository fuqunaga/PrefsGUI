namespace PrefsGUI
{

    public class MaterialPropertyDebugMenuSample : PrefsGUIExampleBase
    {
        MaterialPropertyDebugMenu _debugMenu;

        public void Start()
        {
            _debugMenu = GetComponent<MaterialPropertyDebugMenu>();
        }

        protected override void OnGUIInternal()
        {
            _debugMenu.DebugMenu();
        }
    }
}