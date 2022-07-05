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

            using var undo = new UndoAndPrefabSaveScope(obj, $"PrefsGUI {nameof(UpdateKeyPrefix)}");

            foreach (var prefs in prefsList)
            {
                prefs.key = prefixWithSeparator + prefs.key.Split(PrefsKeyUtility.separator).Last();
            }
        }

        public static void SetCurrentToDefault(Object component, IEnumerable<PrefsParam> prefsList)
        {
            using var undo = new UndoAndPrefabSaveScope(component, $"PrefsGUI {nameof(SetCurrentToDefault)}");

            foreach (var prefs in prefsList)
            {
                prefs.SetCurrentToDefault();
            }
        }

        public static void SavePrefabAssetIfNeed(Object obj)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(obj))
            {
                var root = obj switch
                {
                    GameObject go => go,
                    Component co => co.gameObject,
                    _ => throw new ArgumentOutOfRangeException(nameof(obj), obj, null)
                };

                var trans = root.transform;
                while (trans.parent != null) trans = trans.parent;


                PrefabUtility.SavePrefabAsset(trans.gameObject);
            }
        }

        /// <summary>
        /// Undoに記録し、PrefabAssetならセーブも行う
        /// objがPrefabAssetでPrefabモードのシーンを開いているときはうまく動作しない
        /// （SavePrefabAsset()の影響でシーンの操作が複数Undoに記録されて上書きされてしまう？）
        /// </summary>
        readonly struct UndoAndPrefabSaveScope : IDisposable
        {
            private readonly Object obj;

            public UndoAndPrefabSaveScope(Object obj, string undoName)
            {
                this.obj = obj;
                RecordUndo(undoName);
            }

            private void RecordUndo(string undoName)
            {
                if (PrefabUtility.IsPartOfPrefabAsset(obj))
                {
                    // PrefabAssetはコンポーネント単体で記録させずGameObjectならいけた
                    var root = PrefabUtility.GetNearestPrefabInstanceRoot(obj);
                    if (root != null)
                    {
                        Undo.RecordObject(root, undoName);
                    }
                }

                // Undo.RecordObject()に他のGameObjectの子供になっているGameObjectを渡しても記録されない
                // Componentを指定しないとダメみたい
                Undo.RecordObject(obj, undoName);
            }

            public void Dispose()
            {
                SavePrefabAssetIfNeed(obj);
            }
        }
    }
}