using RapidGUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public abstract class PrefsGUIRapidGUIExampleBase : MonoBehaviour
    {
        protected Rect windowRect = new()
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
            "PrefsGUI - RapidGUI");
        }


        protected virtual void DoGUI()
        {
            GUILayout.Space(50f);
            GUILayout.Label($"file path: {KVS.PrefsKVSPathSelector.path}");

            if (GUILayout.Button("Save")) Prefs.Save();
            if (GUILayout.Button("DeleteAll")) Prefs.DeleteAll();
        }
    }
}