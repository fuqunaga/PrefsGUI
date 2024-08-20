using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PrefsGUI.Editor.Utility
{
    public static class SerializedPropertyExtensions
    {
        private static readonly Dictionary<(Type, string fieldName), FieldInfo> _fieldInfoDictionary = new();
        private static readonly Regex ArrayDataRegex = new(@"data\[(\d+)\]");
        
        public static object GetActualObject(this SerializedProperty property)
        {
            var serializedObject = property.serializedObject;
            if (serializedObject == null)
            {
                return null;
            }
            
            object targetObject = serializedObject.targetObject;
            foreach (var fieldName in property.propertyPath.Split('.'))
            {
                if (targetObject == null)
                {
                    break;
                }
                
                var type = targetObject.GetType();
                
                // Array/ListのpropertyPathは
                // "Array.data[n]" となっている
                if (targetObject is IList list)
                {
                    if (fieldName == "Array")
                    {
                        continue;
                    }
                    
                    var match = ArrayDataRegex.Match(fieldName);
                    if (!match.Success)
                    {
                        Debug.LogWarning($"Invalid propertyPath: {property.propertyPath}");
                        return null;
                    }
                    
                    var index = Convert.ToInt32(match.Groups[1].Value);
                    targetObject = list[index];
                    continue;
                }
                
                
                var key = (type, fieldName);
                if (!_fieldInfoDictionary.TryGetValue(key, out var fi))
                {
                    fi = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    _fieldInfoDictionary[key] = fi;
                    Assert.IsNotNull(fi, $"FieldInfo is null. Type: {type}, FieldName: {fieldName}");
                }
                
                targetObject = fi.GetValue(targetObject);
            }
 

            return targetObject;
        }
    }
}