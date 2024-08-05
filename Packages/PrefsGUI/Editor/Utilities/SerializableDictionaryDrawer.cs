using PrefsGUI.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace PrefsGUI.Editor.Utility
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var name = property.displayName;
            return new PropertyField(GetListSerializedProperty(property), name);
        }
        
        private static SerializedProperty GetListSerializedProperty(SerializedProperty property)
        {
            SerializedProperty listProp = null;
            while(property.NextVisible(true))
            {
                if (property.name == "_list")
                {
                    listProp = property;
                    break;
                }
            }
            Assert.IsNotNull(listProp);

            return listProp;
        }
    }
}