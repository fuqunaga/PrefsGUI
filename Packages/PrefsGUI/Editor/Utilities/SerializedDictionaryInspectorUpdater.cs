using System.Linq;
using System.Text.RegularExpressions;
using PrefsGUI.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace PrefsGUI.Editor.Utility
{
    public class SerializedDictionaryInspectorUpdater
    {
        private const string ListPropertyName = "_list";
        private const string duplicatedKeyMarkName = "prefsgui-duplicated-key-mark";
        
        private static SerializedProperty GetListSerializedProperty(SerializedProperty property)
        {
            var listProp = property.FindPropertyRelative(ListPropertyName);
            Assert.IsNotNull(listProp);

            return listProp;
        }


        private readonly SerializedProperty _rootProperty;
        private IVisualElementScheduledItem _scheduleItem;
        
        public PropertyField Field { get; }

        
        public SerializedDictionaryInspectorUpdater(SerializedProperty property)
        {
            var name = property.displayName;
            _rootProperty = property;
            Field = new PropertyField(GetListSerializedProperty(property), name);

            Field.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                Field.RegisterCallback<SerializedPropertyChangeEvent>(OnSerializedPropertyChanged);
                Field.schedule.Execute(CheckAndUpdateUIForDuplicateKey);
            });

            Field.RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                Field.UnregisterCallback<SerializedPropertyChangeEvent>(OnSerializedPropertyChanged);
                _scheduleItem?.Pause();
                _scheduleItem = null;
            });
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
            
            _scheduleItem = Field.schedule.Execute(CheckAndUpdateUIForDuplicateKey);
        }

        private void CheckAndUpdateUIForDuplicateKey()
        {
            _scheduleItem = null;
            
            if (_rootProperty.GetActualObject() is not ISerializableDictionary serializableDictionary)
            {
                return;
            }

            var itemCount = serializableDictionary.SerializableItemCount;
            

            using var _ = HashSetPool<int>.Get(out var duplicatedKeyIndices);
            duplicatedKeyIndices.UnionWith(serializableDictionary.GetDuplicatedKeyIndices());
            
            var propertyFieldsAll = Field.Query<PropertyField>().Build();
            var propertyFields = Enumerable.Range(0, itemCount)
                .Select(i => $"{_rootProperty.propertyPath}.{ListPropertyName}.Array.data[{i}]")
                .Select(path => propertyFieldsAll.FirstOrDefault(pf => pf.bindingPath == path));

            
            foreach (var (propertyField, index) in propertyFields.Select((pf, index) => (pf, index)))
            {
                if (propertyField == null) continue;
                
                var duplicated = duplicatedKeyIndices.Contains(index);

                var mark = propertyField.Q<Label>(duplicatedKeyMarkName);
                if (duplicated)
                {
                    if (mark == null)
                    {
                        // FoldOutのLabelの横にマークを付ける
                        var firstLabel = propertyField.Q<Label>();
                        firstLabel.parent.Add(CreateDuplicatedKeyMark());
                    }
                }
                else
                {
                    mark?.RemoveFromHierarchy();
                }
            }
        }
        
        private VisualElement CreateDuplicatedKeyMark()
        {
            var color = new Color(0.9f, 0.4f, 0.1f);
            var backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            const float borderRadius = 10f;
            const float marginLR = 4f;
            const float paddingLR = 4f;

            var mark = new Label()
            {
                name = duplicatedKeyMarkName,
                text = "duplicated key",
                style =
                {
                    color = color,
                    backgroundColor = backgroundColor,
                    borderBottomLeftRadius = borderRadius,
                    borderBottomRightRadius = borderRadius,
                    borderTopLeftRadius = borderRadius,
                    borderTopRightRadius = borderRadius,
                    marginLeft = marginLR,
                    marginRight = marginLR,
                    paddingLeft = paddingLR,
                    paddingRight = paddingLR,
                }
            };

            return mark;
        }
    }
}