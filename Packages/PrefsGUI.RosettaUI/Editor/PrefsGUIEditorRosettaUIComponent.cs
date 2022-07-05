using System.Collections.Generic;
using RosettaUI;
using RosettaUI.Editor;
using UnityEditor;
using UnityEngine;

namespace PrefsGUI.RosettaUI.Editor
{
    public static class PrefsGUIEditorRosettaUIComponent
    {
        public static float objectFieldWidth = 300f;
        
        public static Element CreateLineElement() => UI.Space().SetHeight(2f).SetBackgroundColor(Color.gray);
        
        public static Element CreateObjectField(Object obj) => UIEditor.ObjectFieldReadOnly(null, () => obj).SetWidth(objectFieldWidth);
        
        public static IEnumerable<Element> CreateObjectFieldWithAssetMarkParts(Object obj, bool enableSpace = false)
        {
            yield return CreateObjectField(obj);

            if (IsAsset(obj))
            {
                yield return CreateAssetMarkElement();
            }
            else if (enableSpace)
            {
                yield return UI.Space().SetWidth(52f);
            }
        }
      
        static Element CreateAssetMarkElement() => UI.Label("asset").SetColor(new Color(0.9f, 0.6f, 0.1f, 1f));
     
        public static bool IsAsset(Object obj) => PrefabUtility.IsPartOfPrefabAsset(obj);
    }
}