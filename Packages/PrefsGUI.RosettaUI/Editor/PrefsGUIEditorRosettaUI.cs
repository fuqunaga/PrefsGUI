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

        public enum Order
        {
            GameObject,
            Key,
        }

        private string searchWord = "";
        private Order order;
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
                    UI.Field(() => searchWord),
                    UI.Field(() => order),
                    UI.DynamicElementIf(
                        () => order == Order.GameObject,
                        () => UI.Field(() => showComponent)
                    )
                ).SetHeight(150f),
                UI.Space().SetHeight(2f).SetBackgroundColor(Color.gray),
                UI.ScrollViewVertical(1000f,
                    UI.Indent(
                        UI.DynamicElementOnStatusChanged(
                            () => (order, searchWord.ToLower(), showComponent, objPrefsListChangeCount),
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
                    .Where(poc => IsContainWord(poc.Item1.key, word))
                    .OrderBy(poc => poc.Item1.key);
                
                return UI.Column(
                    pocAll.Select(poc =>
                    {
                        var (prefs, obj, component) = poc;
                        
                        return UI.Row(
                            prefs.CreateElement(),
                            UIEditor.ObjectFieldReadOnly(null, () => obj).SetWidth(objectFieldWidth)
                        );
                    })
                );
            }

            Element CreatePrefsGameObject(string word)
            {
                return UI.Column(
                    PrefsAssetUtility.ObjPrefsList.SelectMany(op =>
                    {
                        var objNameHit = IsContainWord(op.obj.name, word);
                        var componentNameHit = op.holders
                            .Select(holder => holder.parent)
                            .Any(parent => IsContainWord(parent.GetType().ToString(), word)
                            );
                        var prefsHit = op.PrefsAll.Any(p => IsContainWord(p.key, word));

                        if (!objNameHit && !prefsHit && !(showComponent && componentNameHit)) return null;

                        if (showComponent)
                        {
                            return op.holders.Select(holder =>
                                {
                                    var enableFilter = !objNameHit &&
                                                       !IsContainWord(holder.parent.GetType().ToString(), word);
                                    return UI.Indent( 
                                        CreateObjFieldAndPrefsListElement(holder.parent, holder.prefsSet.ToList(), true, enableFilter)
                                    );
                                })
                                .Prepend(
                                    CreateObjectFieldAndEditPrefs(op.obj, op.PrefsAll.ToList(), false)
                                );

                        }
                        else 
                        {
                            return CreateObjFieldAndPrefsListElement(op.obj, op.PrefsAll.ToList(), true, !objNameHit, 2);
                        }
                    })
                );

                IEnumerable<Element> CreateObjFieldAndPrefsListElement(Object obj, List<PrefsParam> prefsList, bool editPrefix, bool enableFilter, int indentLevel =
                    1)
                {
                    var prefsSet = enableFilter
                        ? prefsList.Where(prefs => IsContainWord(prefs.key, word))
                        : prefsList;

                    return new[]
                    {
                        CreateObjectFieldAndEditPrefs(obj, prefsList, editPrefix),
                        UI.Indent(
                            prefsSet.Select(prefs => prefs.CreateElement()),
                            indentLevel
                        )
                    };
                }
                
                Element CreateObjectFieldAndEditPrefs(Object obj, List<PrefsParam> prefsList, bool editPrefix)
                {
                    var objectField = UIEditor.ObjectFieldReadOnly(null, () => obj).SetWidth(objectFieldWidth);
                    if (!editPrefix)
                    {
                        return objectField;
                    }
                
                    return UI.Row(
                        objectField,
                        UI.Space().SetWidth(20f),
                        UI.Field(
                            UI.Label("KeyPrefix").SetWidth(100f),
                            () => prefsList
                                .Select(prefs => prefs.key)
                                .Select(PrefsKeyUtility.GetPrefix)
                                .FirstOrDefault(s => !string.IsNullOrEmpty(s)),
                            prefixNew => PrefsGUIEditorUtility.UpdateKeyPrefix(prefixNew, obj, prefsList)
                        ).SetWidth(300f)
                    );
                }
            }
            
            static bool IsContainWord(string word, string searchWordLower) => word.ToLower().Contains(searchWordLower);
        }
    }
}