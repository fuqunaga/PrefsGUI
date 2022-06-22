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
        public static float objectFieldWidth = 300f;

        public enum Order
        {
            GameObject,
            Key,
        }

        private static string searchWord = "";
        private static Order order;
        private static bool includeAssets = true;
        private static bool showComponent;
        
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
            
            const float buttonWidth = 200f;
            
            return UI.Column(
                UI.Page(
                    UI.Row(
                        UI.Button("Save", Prefs.Save).SetWidth(buttonWidth),
                        UI.Button("Load", Prefs.Load).SetWidth(buttonWidth),
                        UI.Button("DeleteAll", () =>
                        {
                            if (PrefsGUIEditorUtility.DisplayDialogDeleteAll())
                            {
                                Prefs.DeleteAll();
                            }
                        }).SetWidth(buttonWidth),
                        UI.Space(),
                        UI.Button("Open Set Current To Default Window",
                            () => GetWindow<SetCurrentToDefaultWindowRosettaUI>(true)
                        ).RegisterUpdateCallback(e =>
                            e.SetInteractable(
                                !Application.isPlaying
                                && PrefsAssetUtility.ObjPrefsList.Any(op => op.PrefsAll.Any(p => !p.IsDefault))
                            )
                        )
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
                CreateLineElement(),
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
                var prefsObjAll = PrefsAssetUtility.PrefsObjEnumerable
                    .Where(po => includeAssets || !IsAsset(po.obj))
                    .Where(po => IsContainWord(po.prefs.key, word))
                    .OrderBy(po => po.prefs.key);
                
                return UI.Column(
                    prefsObjAll.Select(po =>
                    {
                        var (prefs, obj) = po;
                        
                        return UI.Row(
                            new[] {prefs.CreateElement()}
                                .Concat(
                                    CreateObjectFieldWithAssetMarkElement(obj, includeAssets)
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
                                .Select(holder => holder.component)
                                .Any(parent => IsContainWord(parent.GetType().ToString(), word)
                                );
                            var prefsHit = op.PrefsAll.Any(p => IsContainWord(p.key, word));

                            if (!objNameHit && !prefsHit && !(showComponent && componentNameHit))
                            {
                                return Array.Empty<Element>();
                            }

                            if (showComponent)
                            {
                                return new Element[]
                                {
                                    UI.Row(CreateObjectFieldWithAssetMarkElement(op.obj)),
                                    UI.Indent(
                                        op.holders.SelectMany(holder =>
                                        {
                                            var enableFilter = !objNameHit &&
                                                               !IsContainWord(holder.component.GetType().ToString(), word);
                                            
                                            return CreateObjFieldAndPrefsListElement(holder.component,
                                                holder.prefsSet.ToList(), enableFilter, false);
                                        })
                                    )
                                };
                            }
                            else
                            {
                                return CreateObjFieldAndPrefsListElement(op.obj, op.PrefsAll.ToList(), !objNameHit, true, 2);
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
                    var objectFieldEnumerable = enableAssetMark
                        ? CreateObjectFieldWithAssetMarkElement(obj, false)
                        : new[] {CreateObjectField(obj)};
                    
                    return UI.Row(
                        objectFieldEnumerable.Concat(
                            new[]
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
                            })
                    );
                }
            }
            
            
            static bool IsContainWord(string word, string searchWordLower) => word.ToLower().Contains(searchWordLower);
        }

        public static Element CreateLineElement() => UI.Space().SetHeight(2f).SetBackgroundColor(Color.gray);
        
        public static Element CreateObjectField(Object obj) => UIEditor.ObjectFieldReadOnly(null, () => obj).SetWidth(objectFieldWidth);
        
        public static IEnumerable<Element> CreateObjectFieldWithAssetMarkElement(Object obj, bool enableSpace = false)
        {
            yield return CreateObjectField(obj);

            if (IsAsset(obj))
            {
                yield return CreateAssetMarkElement();
            }
            else if (enableSpace)
            {
                yield return CreateAssetMarkElement();
            }
        }
      
        static Element CreateAssetMarkElement() => UI.Label("asset").SetColor(new Color(0.9f, 0.6f, 0.1f, 1f));
        static bool IsAsset(Object obj) => PrefabUtility.IsPartOfPrefabAsset(obj);
    }
}