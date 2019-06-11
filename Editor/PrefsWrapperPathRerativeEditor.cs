using UnityEditor;
using UnityEngine;

namespace PrefsGUI.KVS
{
    [CustomEditor(typeof(PrefsKVSPathCustom))]
    public class PrefsWrapperPathReativeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PrefsKVSPathCustom tg = (PrefsKVSPathCustom)target;

            base.OnInspectorGUI();
            var tmp = GUI.enabled;
            GUI.enabled = false;

            GUILayout.TextField(tg.path);

            GUI.enabled = tmp;
        }
    }
}