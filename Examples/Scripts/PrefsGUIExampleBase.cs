using RapidGUI;
using UnityEngine;

namespace PrefsGUI
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
                OnGUIInternal();
                GUI.DragWindow();
            },
            "");
        }

        protected abstract void OnGUIInternal();
    }
}