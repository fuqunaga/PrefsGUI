using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PrefsGUI.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace PrefsGUI.Editor.Utility
{
    public class SerializableDictionaryInspectorUpdater
    {
        private const string ListPropertyName = "_list";
        
        
        private static SerializedProperty GetListSerializedProperty(SerializedProperty property)
        {
            var listProp = property.FindPropertyRelative(ListPropertyName);
            Assert.IsNotNull(listProp);

            return listProp;
        }


        private readonly SerializedProperty _rootProperty;
        private IVisualElementScheduledItem _scheduleItem;
        
        public PropertyField Field { get; }

        
        public SerializableDictionaryInspectorUpdater(SerializedProperty property)
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
            
            if (_rootProperty.GetActualObject() is not ISerializableDictionaryForUI serializableDictionary)
            {
                return;
            }

            var itemCount = serializableDictionary.SerializableItemCount;


            // インデックスから、所属する重複キーのインデックスリストを求めるテーブル
            using var _ = ListPool<List<int>>.Get(out var indexToSameKeyIndexGroup);
            indexToSameKeyIndexGroup.AddRange(Enumerable.Repeat<List<int>>(null, itemCount));
            foreach (var sameKeyIndexGroup in serializableDictionary.GetSameKeyIndexGroups())
            {
                var list = ListPool<int>.Get();
                list.AddRange(sameKeyIndexGroup);
                
                foreach (var index in list)
                {
                    indexToSameKeyIndexGroup[index] = list;
                }
            }
            
            // PropertyFieldごとにマークを脱着
            var propertyFieldsAll = Field.Query<PropertyField>().Build();
            for(var i=0; i<itemCount; ++i)
            {
                var path = $"{_rootProperty.propertyPath}.{ListPropertyName}.Array.data[{i}]";
                var propertyField = propertyFieldsAll.FirstOrDefault(pf => pf.bindingPath == path);

            
                if (propertyField == null) continue;

                VisualElement mark = propertyField.Q<Label>(className: SerializableDictionaryUIUtility.UssClassNameMark);
                var sameKeyIndices = indexToSameKeyIndexGroup[i];
                
                // 重複キーなし
                if (sameKeyIndices == null || sameKeyIndices.Count == 1)
                {
                    mark?.RemoveFromHierarchy();
                    continue;
                }

                // previous key
                if (i == sameKeyIndices[0])
                {
                    if (mark == null || !mark.ClassListContains(SerializableDictionaryUIUtility.UssClassNameMarkPrevious))
                    {
                        mark?.RemoveFromHierarchy();
                        mark = SerializableDictionaryUIUtility.CreatePreviousKeyMark();
                        AddMark(propertyField, mark);
                    }
                    
                    mark.tooltip = $"duplicate key index: \n{string.Join(", ", sameKeyIndices.Skip(1))}";
                }
                // duplicate key
                else
                {
                    if (mark == null || !mark.ClassListContains(SerializableDictionaryUIUtility.UssClassNameMarkDuplicate))
                    {
                        mark?.RemoveFromHierarchy();
                        mark = SerializableDictionaryUIUtility.CreateDuplicatedKeyMark();
                        AddMark(propertyField, mark);
                    }
                    
                    mark.tooltip = $"previous key index: {sameKeyIndices[0]}.\nIgnored in the dictionary.";
                }
            }

            // sameKeyIndexGroupのリストを返却
            foreach (var list in indexToSameKeyIndexGroup.Distinct().Where(l => l != null))
            {
                ListPool<int>.Release(list);
            }

            return;

            void AddMark(PropertyField field, VisualElement mark)
            {
                // FoldOutのLabelの横にマークを付ける
                var firstLabel = field.Q<Label>();
                firstLabel.parent.Add(mark);

            }
        }
    }
}