using UnityEngine;


namespace PrefsGUI
{
    public class MaterialPropertyDebugMenuSyncSample : MaterialPropertyDebugMenuExample
    {
        bool menuEnable;

        protected override void DoGUI()
        {
            if (menuEnable != GUILayout.Toggle(menuEnable, "MenuEnable"))
            {
                menuEnable = !menuEnable;
                MaterialPropertyDebugMenu.update = menuEnable;
            }

            if (menuEnable)
            {
                base.DoGUI();
            }
        }
    }
}