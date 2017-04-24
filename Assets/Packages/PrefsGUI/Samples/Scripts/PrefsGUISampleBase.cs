using UnityEngine;
using System.Collections;

namespace PrefsGUI
{
    public abstract class PrefsGUISampleBase : MonoBehaviour
    {
        public enum EnumSample
        {
            One,
            Two,
            Three
        }


        Rect _windowRect = new Rect();

        public void OnGUI()
        {
            _windowRect = GUILayout.Window(GetHashCode(), _windowRect, (id) =>
            {
                OnGUIInternal();
                GUI.DragWindow();
            },
            "",
            GUILayout.MinWidth(MinWidth));
        }

        protected abstract void OnGUIInternal();
        protected virtual float MinWidth { get { return 500f; } }
    }
}