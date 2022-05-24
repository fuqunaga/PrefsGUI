using UnityEditor;
using UnityEngine;

namespace PrefsGUI.Kvs.Editor
{
    [CustomEditor(typeof(PrefsKvsPathCustom))]
    public class PrefsKvsPathCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            PrefsKvsPathCustom tg = (PrefsKvsPathCustom)target;

            base.OnInspectorGUI();
            var tmp = GUI.enabled;
            GUI.enabled = false;

            GUILayout.TextField(tg.path);

            GUI.enabled = tmp;
        }
    }
}