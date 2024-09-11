using System;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI.Editor;
using PrefsGUI.Kvs;
using RosettaUI;
using RosettaUI.UIToolkit.Editor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrefsGUI.RosettaUI.Editor
{
    public class PrefsGUIEditorWindowRosettaUI : RosettaUIEditorWindowUIToolkit
    {
        private enum Order
        {
            GameObject,
            Key,
        }
        
        [UnityEditor.MenuItem("Window/PrefsGUI")]
        public static void ShowWindow() => GetWindow<PrefsGUIEditorWindowRosettaUI>("PrefsGUI");

        public static Color nonDefaultPrefsBackgroundColor = new(0.5f, 0.2f, 0.2f, 0.8f);

        private static string searchWord = "";
        private static Order order;
        private static bool includeAssets = true;
        private static bool showComponent;
        private static IPrefsGUIEditorRosettaUIObjCheckExtension _objCheckExtension;
        
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
            return UI.Column(
                CreateTopBar(),
                UI.Page(
                    UI.Space().SetHeight(10f),
                    UI.Row(
                        UI.Field(UI.Label("Filter word", LabelType.Standard).SetWidth(70f), 
                            () => searchWord,
                            new FieldOption() { delayInput = true }
                            ).SetMinWidth(300f),
                        PrefsGUIEditorRosettaUIComponent.SpaceRowGap(),
                        UI.Field(UI.Label("Order").SetWidth(40f), () => order).SetWidth(154f),
                        // UI.Field("Include assets", () => includeAssets),
                        PrefsGUIEditorRosettaUIComponent.SpaceRowGap(),
                        UI.DynamicElementIf(
                            () => order == Order.GameObject,
                            () => UI.Toggle("Show component", () => showComponent)
                        ),
                        UI.Space()
                    )
                ).SetHeight(50f),
                _objCheckExtension?.Title(),
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
                ).SetFlexShrink(1f)
            ).SetFlexShrink(1f);

  
        }

        private static bool IsContainWord(string word, string searchWordLower) => word.ToLower().Contains(searchWordLower);
        

        private static Element CreateTopBar()
        {
            return UI.Row(
                UI.FieldReadOnly(null, () => PrefsKvsPathSelector.Path),
                UI.PopupMenuButton("...",
                    () => new[]
                    {
                        new MenuItem("Open Folder", () => Application.OpenURL(PrefsKvsPathSelector.Path)),
                        new MenuItem("Save Prefs", Prefs.Save),
                        new MenuItem("Load Prefs", Prefs.Load),
                        new MenuItem("Delete All Prefs", () =>
                        {
                            if (PrefsGUIEditorUtility.DisplayDialogDeleteAll())
                            {
                                Prefs.DeleteAll();
                            }
                        })
                    }
                ),
                UI.Space().SetWidth(20f),
                UI.FieldReadOnly("Total Prefs Key Count", () => PrefsParam.allDic.Count),
                UI.Space(),
                UI.Button("Open Set Current To Default Window",
                    () =>
                    {
                        var window = GetWindow<SetCurrentToDefaultWindowRosettaUI>(true);
                        window.includeAssets = includeAssets;
                    }).RegisterUpdateCallback(e =>
                    e.SetInteractable(
                        !Application.isPlaying
                        && PrefsAssetUtility.GetObjPrefsList(includeAssets)
                            .Any(op => op.PrefsAll.Any(p => !p.IsDefault))
                    )
                )
            ).SetFlexShrink(0f);
        }


        private static Element CreatePrefsUIAtoZ(string word)
        {
            var prefsObjAll = PrefsAssetUtility.GetPrefsObjEnumerable(includeAssets)
                .Where(po => IsContainWord(po.prefs.key, word))
                .OrderBy(po => po.prefs.key)
                .ToList();

            return UI.List(null,
                () => prefsObjAll,
                (binder, _) =>
                {
                    var (prefs, obj) = ((IBinder<(PrefsParam, Object)>)binder).Get();

                    var prefsElement = new[]
                    {
                        UI.Indent(
                            prefs.CreateElement().Close()
                        ),
                        PrefsGUIEditorRosettaUIComponent.CreateObjectField(obj)
                    };

                    return _objCheckExtension != null
                        ? UI.Row(
                            _objCheckExtension.PrefsLeft(prefs),
                            UI.Row(prefsElement)
                        )
                        : UI.Row(prefsElement);
                },
                new ListViewOption(false, true, false)
                {
                    reorderable = false,
                    fixedSize = true,
                    header = false,
                    suppressAutoIndent = true
                }
            );
        }
        
        
        private static Element CreatePrefsGameObject(string word)
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
                new ListViewOption()
                {
                    reorderable = false,
                    fixedSize = true,
                    header = false,
                    suppressAutoIndent = _objCheckExtension != null
                }
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

                
                // Show component: off
                if (!showComponent)
                {
                    return CreateObjFieldAndPrefsListElement(objPrefs.obj, prefsListForObj);
                }

                // Show component: on
                return UI.Column(
                    UI.Row(
                        _objCheckExtension?.PrefsSetLeft(prefsListForObj),
                        PrefsGUIEditorRosettaUIComponent.CreateObjectField(objPrefs.obj)
                    ),
                    UI.Indent(
                        objPrefs.holders.Select(holder =>
                        {
                            var enableFilter = !objNameHit &&
                                               !IsContainWord(holder.component.GetType().ToString(), word);

                            var prefsList = (enableFilter
                                    ? FilterPrefs(holder.prefsSet)
                                    : holder.prefsSet
                                ).ToList();

                            return CreateObjFieldAndPrefsListElement(holder.component, prefsList);
                        }),
                        2
                    )
                );
            }

            IEnumerable<PrefsParam> FilterPrefs(IEnumerable<PrefsParam> prefsSet) =>
                prefsSet.Where(prefs => IsContainWord(prefs.key, word));

            Element CreateObjFieldAndPrefsListElement(Object obj, IReadOnlyCollection<PrefsParam> prefsList, int indentLevel = 1)
            {
                if (_objCheckExtension == null)
                {
                    return UI.Fold(
                        CreateObjectFieldAndEditKeyPrefix(obj, prefsList),
                        prefsList.Select(CreatePrefsElement)
                    );
                }

                return UI.Row(
                    _objCheckExtension?.PrefsSetLeft(prefsList),
                    // UI.Fold()で自動的に付くIndentを避けるため new FoldElement() する
                    new FoldElement(
                        CreateObjectFieldAndEditKeyPrefix(obj, prefsList),
                        prefsList.Select(CreatePrefsElement)
                    )
                );

                Element CreatePrefsElement(PrefsParam prefs)
                {
                    var prefsElement = prefs.CreateElement().Close();

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

            Element CreateObjectFieldAndEditKeyPrefix(Object obj, IReadOnlyCollection<PrefsParam> prefsList)
            {
                return UI.Row(
                    PrefsGUIEditorRosettaUIComponent.CreateObjectField(obj),
                    PrefsGUIEditorRosettaUIComponent.SpaceRowGap(),
                    UI.Field(
                        UI.Label("KeyPrefix").SetWidth(60f),
                        () => prefsList
                            .Select(prefs => prefs.key)
                            .Select(PrefsKeyUtility.GetPrefix)
                            .FirstOrDefault(s => !string.IsNullOrEmpty(s)),
                        prefixNew => PrefsGUIEditorUtility.UpdateKeyPrefix(prefixNew, obj, prefsList)
                    ).SetWidth(250f)
                ).RegisterUpdateCallback(row =>
                {
                    var hasNonDefaultValue = prefsList.Any(p => !p.IsDefault);
                    row.SetBackgroundColor(hasNonDefaultValue ? nonDefaultPrefsBackgroundColor : null);
                });
            }
        }
        
        public static void RegisterObjCheckExtension(IPrefsGUIEditorRosettaUIObjCheckExtension objCheckExtension)
        {
            _objCheckExtension = objCheckExtension;
        }
    }
}