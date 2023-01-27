using System;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI.Editor;
using RosettaUI;
using RosettaUI.UIToolkit.Editor;
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

        private static string searchWord = "";
        private static Order order;
        private static bool includeAssets = true;
        private static bool showComponent;
        
        private int objPrefsListChangeCount;

        private static IPrefsGUIEditorRosettaUIObjCheckExtension _objCheckExtension;
        
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
                            () =>
                            {
                                var window = GetWindow<SetCurrentToDefaultWindowRosettaUI>(true);
                                window.includeAssets = includeAssets;
                            }).RegisterUpdateCallback(e =>
                            e.SetInteractable(
                                !Application.isPlaying
                                && PrefsAssetUtility.GetObjPrefsList(includeAssets).Any(op => op.PrefsAll.Any(p => !p.IsDefault))
                            )
                        )
                    ),
                    UI.Space().SetHeight(20f),
                    UI.Field("Search word", () => searchWord, new FieldOption() { delayInput = true }),
                    UI.Field("Order", () => order),
                    UI.Field("Include assets", () => includeAssets),
                    UI.DynamicElementIf(
                        () => order == Order.GameObject,
                        () => UI.Field("Show component", () => showComponent)
                    ),
                    UI.Space(),
                    _objCheckExtension?.Title()
                ).SetHeight(230f),
                PrefsGUIEditorRosettaUIComponent.CreateLineElement(),
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
                ).SetHeight(scrollViewHeight)
            );

            Element CreatePrefsUIAtoZ(string word)
            {
                var prefsObjAll = PrefsAssetUtility.GetPrefsObjEnumerable(includeAssets)
                    .Where(po => IsContainWord(po.prefs.key, word))
                    .OrderBy(po => po.prefs.key)
                    .ToList();

                return UI.List(null,
                    () => prefsObjAll,
                    (binder, index) =>
                    {
                        var (prefs, obj) = ((IBinder<(PrefsParam, Object)>)binder).Get();

                        var prefsElement = PrefsGUIEditorRosettaUIComponent
                            .CreateObjectFieldWithAssetMarkParts(obj, includeAssets)
                            .Prepend(prefs.CreateElement());

                        return _objCheckExtension != null
                            ? UI.Row(
                                _objCheckExtension.PrefsLeft(prefs),
                                UI.Indent(UI.Row(prefsElement))
                            )
                            : UI.Indent(UI.Row(prefsElement));
                    },
                    new ListViewOption(false, true, false)
                );
            }

            Element CreatePrefsGameObject(string word)
            {
                var objPrefsList = PrefsAssetUtility.GetObjPrefsList(includeAssets)
                    .Where(objPrefs =>
                    {
                        var objNameHit = IsContainWord(objPrefs.obj.name, word);
                        var componentNameHit = objPrefs.holders
                            .Select(holder => holder.component)
                            .Any(parent => IsContainWord(parent.GetType().ToString(), word)
                            );
                        var prefsHit = objPrefs.PrefsAll.Any(p => IsContainWord(p.key, word));

                        return objNameHit || prefsHit || showComponent && componentNameHit;
                    })
                    .ToList();
                
                return UI.List(null,
                    () => objPrefsList, (binder, idx) =>
                    {
                        var typedBinder = (IBinder<PrefsAssetUtility.ObjPrefs>)binder;
                        return CreateObjPrefsElement(typedBinder.Get());
                    },
                    new ListViewOption(false, true, false)
                    );

                Element CreateObjPrefsElement(PrefsAssetUtility.ObjPrefs objPrefs)
                {
                    var objNameHit = IsContainWord(objPrefs.obj.name, word);
                    var componentNameHit = objPrefs.holders
                        .Select(holder => holder.component)
                        .Any(parent => IsContainWord(parent.GetType().ToString(), word)
                        );
                    var prefsHit = objPrefs.PrefsAll.Any(p => IsContainWord(p.key, word));

                    if (!objNameHit && !prefsHit && !(showComponent && componentNameHit))
                    {
                        return null;
                    }

                    var prefsListForObj = (objNameHit
                            ? objPrefs.PrefsAll
                            : FilterPrefs(objPrefs.PrefsAll)
                        ).ToList();

                    if (showComponent)
                    {
                        var objFieldParts =
                            PrefsGUIEditorRosettaUIComponent.CreateObjectFieldWithAssetMarkParts(objPrefs.obj);

                        return UI.Column(
                            UI.Row(
                                _objCheckExtension == null
                                    ? objFieldParts
                                    : objFieldParts.Prepend(_objCheckExtension.PrefsSetLeft(prefsListForObj))
                            ),
                            UI.Indent(
                                objPrefs.holders.SelectMany(holder =>
                                {
                                    var enableFilter = !objNameHit &&
                                                       !IsContainWord(holder.component.GetType().ToString(), word);

                                    var prefsList = (enableFilter
                                            ? FilterPrefs(holder.prefsSet)
                                            : holder.prefsSet
                                        ).ToList();

                                    return CreateObjFieldAndPrefsListElement(holder.component, prefsList, false);
                                })
                            )
                        );
                    }
                    else
                    {
                        return UI.Column(
                            CreateObjFieldAndPrefsListElement(objPrefs.obj, prefsListForObj, true, 2)
                        );
                    }
                }
         
                IEnumerable<PrefsParam> FilterPrefs(IEnumerable<PrefsParam> prefsSet) =>
                    prefsSet.Where(prefs => IsContainWord(prefs.key, word));

                IEnumerable<Element> CreateObjFieldAndPrefsListElement(Object obj, List<PrefsParam> prefsList, bool enableAssetMark, int indentLevel = 1)
                {
                    return new[]
                    {
                        CreateObjectFieldAndEditPrefs(obj, prefsList, enableAssetMark),
                        UI.Indent(
                            prefsList.Select(CreatePrefsElement),
                            indentLevel
                        )
                    };

                    Element CreatePrefsElement(PrefsParam prefs)
                    {
                        var prefsElement = prefs.CreateElement(); 
                        
                        return _objCheckExtension == null
                            ? prefsElement
                            : UI.Row(
                                _objCheckExtension.PrefsLeft(prefs),
                                UI.Indent(
                                    prefsElement
                                )
                            );
                    }
                }
                
                Element CreateObjectFieldAndEditPrefs(Object obj, IReadOnlyCollection<PrefsParam> prefsList, bool enableAssetMark)
                {
                    var objectFieldParts = enableAssetMark
                        ? PrefsGUIEditorRosettaUIComponent.CreateObjectFieldWithAssetMarkParts(obj, false)
                        : new[] {PrefsGUIEditorRosettaUIComponent.CreateObjectField(obj)};

                    var objectFieldAndEditPrefsParts = objectFieldParts.Concat(
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
                        });



                    return UI.Row(
                        _objCheckExtension != null
                            ? objectFieldAndEditPrefsParts.Prepend(_objCheckExtension.PrefsSetLeft(prefsList))
                            : objectFieldAndEditPrefsParts
                    );
                }
            }
            
            
            static bool IsContainWord(string word, string searchWordLower) => word.ToLower().Contains(searchWordLower);
        }

        public static void RegisterObjCheckExtension(IPrefsGUIEditorRosettaUIObjCheckExtension objCheckExtension)
        {
            _objCheckExtension = objCheckExtension;
        }
    }
}