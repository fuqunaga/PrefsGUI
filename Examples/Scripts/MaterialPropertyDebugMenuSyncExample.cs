using UnityEngine;


namespace PrefsGUI
{
    public class MaterialPropertyDebugMenuSyncSample : MaterialPropertyDebugMenuExample
    {
        bool menuEnable;

        protected override void OnGUIInternal()
        {
            if (menuEnable != GUILayout.Toggle(menuEnable, "MenuEnable"))
            {
                menuEnable = !menuEnable;
                MaterialPropertyDebugMenu.update = menuEnable;
            }

            if (menuEnable)
            {
                base.OnGUIInternal();
            }
        }
    }
}