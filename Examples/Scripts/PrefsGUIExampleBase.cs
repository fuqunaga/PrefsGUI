using RapidGUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public abstract class PrefsGUIExampleBase : MonoBehaviour
    {
        public enum EnumSample
        {
            One,
            Two,
            Three
        }


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
            "");
        }

        protected abstract void DoGUI();
    }
}