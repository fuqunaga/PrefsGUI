using RapidGUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public abstract class PrefsGUIExampleBase : MonoBehaviour
    {
        Rect windowRect = new Rect()
        {
            width = 500f
        };

        public void OnGUI()
        {
            windowRect = RGUI.ResizableWindow(GetHashCode(), windowRect, (id) =>
            {
                DoGUI();
                GUI.DragWindow();
            },
            "PrefsGUI");
        }

        protected abstract void DoGUI();
    }
}