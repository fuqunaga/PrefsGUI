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
            AtoZ,
            GameObject,
        }

        private Order order;
        private string searchWord = "";
        private int objPrefsListChangeCount;

        /*
        SetCurrentToDefaultWindow setCurrentToDefaultWindow;

        FastScrollView scrollViewAtoZ = new FastScrollView();
        FastScrollView scrollViewGameObject = new FastScrollView();

        private void Awake()
        {
            ObjectPrefsUtility.onGoPrefsListChanged += OnGpListChanged;
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (ObjectPrefsUtility.onGoPrefsListChanged != null)
                ObjectPrefsUtility.onGoPrefsListChanged -= OnGpListChanged;
        }

        
        void OnGpListChanged()
        {
            scrollViewAtoZ.SetNeedUpdateLayout();
            scrollViewGameObject.SetNeedUpdateLayout();
        }
        */

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
                            if (EditorUtility.DisplayDialog("DeleteAll",
                                    "Are you sure to delete all current prefs parameters?",
                                    "DeleteAll", "Don't Delete"))
                            {
                                Prefs.DeleteAll();
                            }
                        })
                    ),
                    UI.Field(() => searchWord),
                    UI.Field(() => order)
                ),
                UI.Space().SetHeight(30f),
                UI.Space().SetHeight(2f).SetBackgroundColor(Color.gray),
                UI.ScrollViewVertical(1000f,
                    UI.Indent(
                        UI.DynamicElementOnStatusChanged(
                            () => (order, searchWord.ToLower(), objPrefsListChangeCount),
                            pair =>
                            {
                                var (_, word, _) = pair;

                                return order switch
                                {
                                    Order.AtoZ => CreatePrefsUIAtoZ(word),
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
                    .Where(poc => poc.Item1.key.ToLower().Contains(word))
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
                    PrefsAssetUtility.ObjPrefsList.Select(op =>
                    {
                        var objNameHit = op.obj.name.ToLower().Contains(word);
                        var componentNameHit = op.holders.Select(holder => holder.parent)
                            .Any(parent => parent.name.ToLower().Contains(word)
                                           || parent.GetType().ToString().ToLower().Contains(word)
                            );
                        var prefsHit = op.PrefsAll.Any(p => p.key.ToLower().Contains(word));

                        if (!objNameHit && !prefsHit) return null;


                        return UI.Column(
                            CreateObjectFieldAndEditPrefs(op.obj, op.PrefsAll.ToList(), true),
                            UI.Indent(
                                op.holders.Select(holder =>
                                    UI.Indent(
                                        holder.prefsSet.Select(prefs => prefs.CreateElement())
                                    )
                                )
                            )
                        );
                    })
                );
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
                        prefixNew =>
                        {
                            var prefixWithSeparator = string.IsNullOrEmpty(prefixNew)
                                ? ""
                                : prefixNew + PrefsKeyUtility.separator;

                            Undo.RecordObject(obj, "Change PrefsGUI Prefix");

                            foreach (var prefs in prefsList)
                            {
                                prefs.key = prefixWithSeparator + prefs.key.Split(PrefsKeyUtility.separator).Last();
                            }
                            
                            EditorUtility.SetDirty(obj);
                        }).SetWidth(300f)
                );
            }
        }
    }
}