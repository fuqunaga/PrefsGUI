using UnityEngine;

namespace PrefsGUI.Example
{
    [RequireComponent(typeof(MaterialPropertyDebugMenu))]
    public class MaterialPropertyDebugMenuExample : PrefsGUIExampleBase
    {
        protected MaterialPropertyDebugMenu debugMenu;

        public virtual void Start()
        {
            debugMenu = GetComponent<MaterialPropertyDebugMenu>();
        }

        protected override void DoGUI()
        {
            debugMenu.DoGUI();
            base.DoGUI();
        }
    }
}