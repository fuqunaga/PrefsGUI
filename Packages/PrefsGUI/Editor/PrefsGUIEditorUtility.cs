using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrefsGUI.Editor
{
    public static class PrefsGUIEditorUtility
    {
        public static bool DisplayDialogDeleteAll()
        {
            return EditorUtility.DisplayDialog("DeleteAll", "Are you sure to delete all current prefs parameters?",
                "DeleteAll", "Cancel");
        }

        public static void UpdateKeyPrefix(string prefixNew, Object obj, IEnumerable<PrefsParam> prefsList)
        {
            var prefixWithSeparator = string.IsNullOrEmpty(prefixNew)
                ? ""
                : prefixNew + PrefsKeyUtility.separator;

            Undo.RecordObject(obj, "Change PrefsGUI Prefix");

            foreach (var prefs in prefsList)
            {
                prefs.key = prefixWithSeparator + prefs.key.Split(PrefsKeyUtility.separator).Last();
            }
                            
            EditorUtility.SetDirty(obj);
            Debug.Log(obj.GetType());
            if (PrefabUtility.IsPartOfPrefabAsset(obj))
            {
                var root = obj switch
                {
                    GameObject go => go,
                    Component co => co.gameObject,
                    _ => throw new ArgumentOutOfRangeException(nameof(obj), obj, null)
                };
                
                while (root.transform.parent != null)
                {
                    root = root.transform.parent.gameObject;
                }
                                    
                PrefabUtility.SavePrefabAsset(root);
            }
        }
    }
}