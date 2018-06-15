using UnityEditor;
using UnityEngine;

namespace PrefsGUI.Wrapper
{
    [CustomEditor(typeof(PrefsWrapperPathCustom))]
    public class PrefsWrapperPathReativeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PrefsWrapperPathCustom tg = (PrefsWrapperPathCustom)target;

            base.OnInspectorGUI();
            var tmp = GUI.enabled;
            GUI.enabled = false;

            GUILayout.TextField(tg.path);

            GUI.enabled = tmp;
        }
    }
}