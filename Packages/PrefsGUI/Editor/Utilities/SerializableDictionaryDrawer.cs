using PrefsGUI.Utility;
using UnityEditor;
using UnityEngine.UIElements;

namespace PrefsGUI.Editor.Utility
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        // 配列内の要素だと、同一インスタンスで複数のSerializedPropertyに大してコールされるっぽいので
        // SerializableDictionaryDrawer自体のメンバー変数は使わない
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var inspectorUpdater = new SerializedDictionaryInspectorUpdater(property);
            return inspectorUpdater.Field;
        }
    }
}