using System;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI.Editor;
using RosettaUI;
using RosettaUI.Editor;
using RosettaUI.UIToolkit.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrefsGUI.RosettaUI.Editor
{
    public class PrefsGUIEditorRosettaUI : RosettaUIEditorWindowUIToolkit
    {
        [UnityEditor.MenuItem("Window/PrefsGUI")]
        public static void ShowWindow() => GetWindow<PrefsGUIEditorRosettaUI>("PrefsGUI");

        public static float scrollViewHeight = 1000f;

        public enum Order
        {
            GameObject,
            Key,
        }

        private string searchWord = "";
        private Order order;
        private bool includeAssets = true;
        private bool showComponent;
        
        private int objPrefsListChangeCount;

 
        private void OnEnable()
        {
            PrefsAssetUtility.onObjPrefsListChanged += OnObjPrefsListChanged;
        }

        private void OnDisable()
        {
            PrefsAssetUtility.onObjPrefsListChanged -= OnObjPrefsListChanged;
        }

        protected override void Update()
        {
            base.Update();
            PrefsAssetUtility.UpdateObjPrefsIfNeed();
        }

        private void OnObjPrefsListChanged()
        {
            objPrefsListChangeCount++;
        }

        protected override Element CreateElement()
        {
            const float objectFieldWidth = 300f;
            
            return UI.Column(
                UI.Page(
                    UI.Row(
                        UI.Button("Save", Prefs.Save),
                        UI.Button("Load", Prefs.Load),
                        UI.Button("DeleteAll", () =>
                        {
                            if (PrefsGUIEditorUtility.DisplayDialogDeleteAll())
                            {
                                Prefs.DeleteAll();
                            }
                        })
                    ),
                    UI.Space().SetHeight(20f),
                    UI.Field(() => searchWord),
                    UI.Field(() => order),
                    UI.Field(() => includeAssets),
                    UI.DynamicElementIf(
                        () => order == Order.GameObject,
                        () => UI.Field(() => showComponent)
                    )
                ).SetHeight(200f),
                UI.Space().SetHeight(2f).SetBackgroundColor(Color.gray),
                UI.ScrollViewVertical(scrollViewHeight,
                    UI.Indent(
                        UI.DynamicElementOnStatusChanged(
                            () => (order, searchWord.ToLower(), includeAssets, showComponent, objPrefsListChangeCount),
                            _ =>
                            {
                                var word = searchWord.ToLower();

                                return order switch
                                {
                                    Order.Key => CreatePrefsUIAtoZ(word),
                                    Order.GameObject => CreatePrefsGameObject(word),
                                    _ => throw new ArgumentOutOfRangeException()
                                };
                            }
                        )
                    )
                )
            );

            Element CreatePrefsUIAtoZ(string word)
            {
                var pocAll = PrefsAssetUtility.PrefsObjComponentList
                    .Where(poc => includeAssets || !IsAsset(poc.Item2))
                    .Where(poc => IsContainWord(poc.Item1.key, word))
                    .OrderBy(poc => poc.Item1.key);
                
                return UI.Column(
                    pocAll.Select(poc =>
                    {
                        var (prefs, obj, component) = poc;

                        return UI.Row(
                            CreateObjectFieldWithAssetMarkElement(obj, true)
                                .Prepend(
                                    prefs.CreateElement()
                                )
                        );
                    })
                );
            }

            Element CreatePrefsGameObject(string word)
            {
                return UI.Column(
                    PrefsAssetUtility.ObjPrefsList
                        .Where(op => includeAssets || !IsAsset(op.obj))
                        .SelectMany(op =>
                        {
                            var objNameHit = IsContainWord(op.obj.name, word);
                            var componentNameHit = op.holders
                                .Select(holder => holder.parent)
                                .Any(parent => IsContainWord(parent.GetType().ToString(), word)
                                );
                            var prefsHit = op.PrefsAll.Any(p => IsContainWord(p.key, word));

                            if (!objNameHit && !prefsHit && !(showComponent && componentNameHit))
                            {
                                return null;
                            }

                            if (showComponent)
                            {
                                return new Element[]
                                {
                                    UI.Row(CreateObjectFieldWithAssetMarkElement(op.obj, false)),
                                    UI.Indent(
                                        op.holders.SelectMany(holder =>
                                        {
                                            var enableFilter = !objNameHit &&
                                                               !IsContainWord(holder.parent.GetType().ToString(), word);
                                            
                                            return CreateObjFieldAndPrefsListElement(holder.parent,
                                                holder.prefsSet.ToList(), enableFilter, false);
                                        })
                                    )
                                };
                            }
                            else
                            {
                                return CreateObjFieldAndPrefsListElement(op.obj, op.PrefsAll.ToList(), !objNameHit, false, 2);
                            }
                        })
                );

                IEnumerable<Element> CreateObjFieldAndPrefsListElement(Object obj, List<PrefsParam> prefsList, bool enableFilter, bool enableAssetMark, int indentLevel = 1)
                {
                    var prefsSet = enableFilter
                        ? prefsList.Where(prefs => IsContainWord(prefs.key, word))
                        : prefsList;

                    return new[]
                    {
                        CreateObjectFieldAndEditPrefs(obj, prefsList, enableAssetMark),
                        UI.Indent(
                            prefsSet.Select(prefs => prefs.CreateElement()),
                            indentLevel
                        )
                    };
                }
                
                Element CreateObjectFieldAndEditPrefs(Object obj, List<PrefsParam> prefsList, bool enableAssetMark)
                {
                    return UI.Row(
                        (
                            enableAssetMark
                                ? CreateObjectFieldWithAssetMarkElement(obj, false)
                                : new[] {CreateObjectField(obj)}
                        )
                        .Concat(new[]
                            {
                                UI.Space().SetWidth(20f),
                                UI.Field(
                                    UI.Label("KeyPrefix").SetWidth(100f),
                                    () => prefsList
                                        .Select(prefs => prefs.key)
                                        .Select(PrefsKeyUtility.GetPrefix)
                                        .FirstOrDefault(s => !string.IsNullOrEmpty(s)),
                                    prefixNew => PrefsGUIEditorUtility.UpdateKeyPrefix(prefixNew, obj, prefsList)
                                ).SetWidth(300f)
                            }
                        )
                    );
                }
            }

            static Element CreateObjectField(Object obj) => UIEditor.ObjectFieldReadOnly(null, () => obj).SetWidth(objectFieldWidth);
            static IEnumerable<Element> CreateObjectFieldWithAssetMarkElement(Object obj, bool enableSpace)
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
            
            static bool IsAsset(Object obj) => PrefabUtility.IsPartOfPrefabAsset(obj);
            static bool IsContainWord(string word, string searchWordLower) => word.ToLower().Contains(searchWordLower);
            static Element CreateAssetMarkElement() => UI.Label("asset").SetColor(new Color(0.9f, 0.6f, 0.1f, 1f));
        }
    }
}