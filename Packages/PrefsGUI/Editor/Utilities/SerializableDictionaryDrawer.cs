using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using PrefsGUI.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace PrefsGUI.Editor.Utility
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const string ListPropertyName = "_list";
        private static readonly Dictionary<Type, FieldInfo> _objectToSerializableDictionaryFieldInfoTable = new();
        
        private static SerializedProperty GetListSerializedProperty(SerializedProperty property)
        {
            var listProp = property.FindPropertyRelative(ListPropertyName);
            Assert.IsNotNull(listProp);

            return listProp;
        }
        

        private SerializedProperty _rootProperty;
        private PropertyField _propertyField;
        private IVisualElementScheduledItem _scheduleItem;
        
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var name = property.displayName;
            _rootProperty = property;
            _propertyField = new PropertyField(GetListSerializedProperty(property), name);
            _propertyField.RegisterCallback<SerializedPropertyChangeEvent>(OnSerializedPropertyChanged);

            CheckAndUpdateUIForDuplicateKey();
            
            return _propertyField;
        }

        private void OnSerializedPropertyChanged(SerializedPropertyChangeEvent evt)
        {
            if (_scheduleItem != null) return;
            
            // プロパティのパスからDictionaryのKeyの変更を検知する
            // _rootProperty.propertyPathはアプリケーション側で自由につけれるので
            // 正規表現のメタ文字が入ってくる可能性があるためあらかじめ省く
            var path = evt.changedProperty.propertyPath;
            var rootPath = _rootProperty.propertyPath;
            if (!path.StartsWith($"{rootPath}.")) return;

            path = path[(rootPath.Length + 1)..];
            var keyPath = $@"{ListPropertyName}\.Array\.data\[\d+\]\.key$";
            if (!Regex.IsMatch(path, keyPath)) return;
            
            _scheduleItem = _propertyField.schedule.Execute(CheckAndUpdateUIForDuplicateKey);
            
            Debug.Log(evt.changedProperty.propertyPath);
        }

        private void CheckAndUpdateUIForDuplicateKey()
        {
            _scheduleItem = null;

            var obj = _rootProperty.GetActualObject();
            if (obj is IEnumerable collection)
            {
                Debug.Log(collection);
            }

            // var targetObject = _rootProperty.serializedObject.targetObject;
            //
            // var targetObjectType = targetObject.GetType();
            // if ( !_objectToSerializableDictionaryFieldInfoTable.TryGetValue(targetObjectType, out var fi) )
            // {
            //     fi = targetObjectType.GetField(_rootProperty.propertyPath);
            //     _objectToSerializableDictionaryFieldInfoTable.Add(targetObjectType, fi);
            // }
            //
            // var dictionary = fi.GetValue(targetObject);
            Debug.Log($"{nameof(CheckAndUpdateUIForDuplicateKey)}");
            
        }
    }
}