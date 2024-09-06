using System.Collections.Generic;
using RosettaUI;
using RosettaUI.Editor;
using UnityEditor;
using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global

namespace PrefsGUI.RosettaUI.Editor
{
    public static class PrefsGUIEditorRosettaUIComponent
    {
        public static float objectFieldWidth = 300f;
        public static float rowGapWidth = 20f;
        
        public static Element CreateLineElement() => UI.Space().SetHeight(2f).SetBackgroundColor(Color.gray);
        
        public static Element CreateObjectField(Object obj) => UIEditor.ObjectFieldReadOnly(null, () => obj).SetWidth(objectFieldWidth);
        
        public static Element SpaceRowGap() => UI.Space().SetWidth(rowGapWidth);
    }
}