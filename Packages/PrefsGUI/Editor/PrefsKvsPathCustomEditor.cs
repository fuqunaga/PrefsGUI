using System;
using UnityEditor;
using UnityEngine;

namespace PrefsGUI.Kvs.Editor
{
    [CustomEditor(typeof(PrefsKvsPathCustom))]
    public class PrefsKvsPathCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var pathCustom = (PrefsKvsPathCustom)target;

            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            
            
            // Script field
            prop.NextVisible(true);
            GUI.enabled = false;
            EditorGUILayout.PropertyField(prop);
            GUI.enabled = true;
            
            // Other fields
            while(prop.NextVisible(false))
            {
                if (prop.name == nameof(PrefsKvsPathCustom.Platform))
                {
                    PlatformMaskGUI(prop);
                }
                else
                {
                    EditorGUILayout.PropertyField(prop);
                }
            }
            
            GUI.enabled = false;
            GUILayout.TextField(pathCustom.pathWithoutPlatformCheck);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        static void PlatformMaskGUI(SerializedProperty prop)
        {
            prop.enumValueFlag = EditorGUILayout.MaskField(
                ObjectNames.NicifyVariableName(prop.name),
                prop.enumValueFlag,
                Enum.GetNames(typeof(PrefsKvsPathCustom.Platform))
            );
        }
    }
}