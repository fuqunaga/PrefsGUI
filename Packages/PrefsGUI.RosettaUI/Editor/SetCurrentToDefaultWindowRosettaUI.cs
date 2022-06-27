using System;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI.Editor;
using RosettaUI;
using RosettaUI.UIToolkit.Editor;
using UnityEngine.Pool;

namespace PrefsGUI.RosettaUI.Editor
{
    public class SetCurrentToDefaultWindowRosettaUI : RosettaUIEditorWindowUIToolkit
    {
        private static readonly Dictionary<PrefsParam, bool> prefsCheckTable = new();
        
        static bool IsChecked(PrefsParam prefs)
        {
            if (!prefsCheckTable.TryGetValue(prefs, out var ret))
            {
                prefsCheckTable[prefs] = ret = true;
            }

            return ret;
        }

        protected override Element CreateElement()
        {
            var lastPrefsSetHash = 0;
            
            return UI.Column(
                UI.HelpBox("\nSelect Prefs to change Default.\n", HelpBoxType.Info),
                UI.Space().SetHeight(5f),
                UI.Row(
                    UI.Space(),
                    UI.Button("Set Current To Default", () =>
                    {
                        SetCurrentToDefault();
                        Close();
                    }).SetWidth(220f)
                    ),
                UI.DynamicElementOnTrigger(
                    _ => CheckPrefsNonDefaultChanged(),
                    () => UI.Column(
                        CreatePrefsListCheckElement(GetPrefsNonDefaults().ToList()),
                        PrefsGUIEditorRosettaUIComponent.CreateLineElement(),
                        UI.ScrollViewVertical(PrefsGUIEditorRosettaUI.scrollViewHeight,
                            CreateObjectPrefsElements()
                        )
                    )
                )
            );
            
            bool CheckPrefsNonDefaultChanged()
            {
                var hash = GetPrefsNonDefaults().Aggregate(0, (total, prefs) => total ^ prefs.GetHashCode());
            
                var updated = (lastPrefsSetHash != hash);
                if ( updated)
                {
                    lastPrefsSetHash = hash;
                }

                return updated;
            }

            static IEnumerable<PrefsParam> GetPrefsNonDefaults()
            {
                return PrefsAssetUtility.PrefsObjEnumerable
                    .Select(po => po.prefs)
                    .Where(prefs => !prefs.IsDefault);
            }
        }

        static void SetCurrentToDefault()
        {
            var holders = PrefsAssetUtility.ObjPrefsList.SelectMany(objPrefs => objPrefs.holders);

            using var listPool = ListPool<PrefsParam>.Get(out var prefsNonDefaults);

            foreach (var holder in holders)
            {
                prefsNonDefaults.Clear();
                prefsNonDefaults.AddRange(
                    holder.prefsSet.Where(prefs => !prefs.IsDefault && IsChecked(prefs))
                );

                if (prefsNonDefaults.Any())
                {
                    PrefsGUIEditorUtility.SetCurrentToDefault(holder.component, prefsNonDefaults);
                }
            }
        }
        
        private static IEnumerable<Element> CreateObjectPrefsElements()
        {
            return  PrefsAssetUtility.ObjPrefsList.SelectMany(objPrefs =>
            {
                var prefsList = objPrefs.PrefsAll.Where(prefs => !prefs.IsDefault).ToList();
                if (!prefsList.Any())
                {
                    return Array.Empty<Element>();
                }
                
                return new Element[]
                {
                    UI.Row(
                        PrefsGUIEditorRosettaUIComponent.CreateObjectFieldWithAssetMarkParts(objPrefs.obj)
                            .Prepend(
                                CreatePrefsListCheckElement(prefsList)
                            )
                    ),
                    UI.Indent(
                        prefsList.Select(prefs => UI.Row(
                                CreatePrefsCheckElement(prefs),
                                UI.Indent(
                                    prefs.CreateElement()
                                )
                            )
                        )
                    )
                };
            });
            

            Element CreatePrefsCheckElement(PrefsParam prefs)
                => UI.Field(null,
                    () => IsChecked(prefs),
                    flag => prefsCheckTable[prefs] = flag
                );
        }

        private static Element CreatePrefsListCheckElement(List<PrefsParam> prefsSet)
        {
            return UI.Field(null,
                () => prefsSet.Any(IsChecked),
                flag =>
                {
                    foreach (var prefs in prefsSet)
                    {
                        prefsCheckTable[prefs] = flag;
                    }
                }
            );
        }
    }
}