using RapidGUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public abstract class PrefsGUIRapidGUIExampleBase : MonoBehaviour
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


        protected virtual void DoGUI()
        {
            GUILayout.Space(50f);
            GUILayout.Label($"file path: {KVS.PrefsKVSPathSelector.path}");

            if (GUILayout.Button("Save")) Prefs.Save();
            if (GUILayout.Button("DeleteAll")) Prefs.DeleteAll();
        }
    }
}