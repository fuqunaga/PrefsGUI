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
        private static readonly Dictionary<PrefsParam, bool> PrefsCheckTable = new();
        public bool includeAssets;

        private static bool IsChecked(PrefsParam prefs)
        {
            if (!PrefsCheckTable.TryGetValue(prefs, out var ret))
            {
                PrefsCheckTable[prefs] = ret = false;
            }

            return ret;
        }

        protected override Element CreateElement()
        {
            var lastPrefsSetHash = 0;

            return UI.Column(
                UI.Column(
                    UI.HelpBox("\nSelect Prefs to change Default value.\n", HelpBoxType.Info),
                    UI.Space().SetHeight(5f),
                    UI.Row(
                        UI.Space(),
                        UI.Button("Set Current To Default", () =>
                        {
                            SetCurrentToDefault();
                            Close();
                        }).SetWidth(220f)
                    )
                ).SetFlexGrow(0),
                UI.DynamicElementOnTrigger(
                    _ => CheckPrefsNonDefaultChanged(),
                    () => UI.Column(
                        CreatePrefsListCheckElement(GetPrefsNonDefaults().ToList()),
                        PrefsGUIEditorRosettaUIComponent.CreateLineElement(),
                        UI.ScrollViewVertical(/*PrefsGUIEditorWindowRosettaUI.scrollViewHeight*/null,
                            CreateObjectPrefsElements()
                        )
                    ).SetFlexShrink(1)
                ).SetFlexShrink(1)
            ).SetFlexShrink(1);
            
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

            IEnumerable<PrefsParam> GetPrefsNonDefaults()
            {
                return PrefsAssetUtility.GetPrefsObjEnumerable(includeAssets)
                    .Select(po => po.prefs)
                    .Where(prefs => !prefs.IsDefault);
            }
        }

        private void SetCurrentToDefault()
        {
            var holders = PrefsAssetUtility.GetObjPrefsList(includeAssets).SelectMany(objPrefs => objPrefs.holders);

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
        
        private IEnumerable<Element> CreateObjectPrefsElements()
        {
            return  PrefsAssetUtility.GetObjPrefsList(includeAssets).SelectMany(objPrefs =>
            {
                var prefsList = objPrefs.PrefsAll.Where(prefs => !prefs.IsDefault).ToList();
                if (!prefsList.Any())
                {
                    return Array.Empty<Element>();
                }
                
                return new Element[]
                {
                    UI.Row(
                        CreatePrefsListCheckElement(prefsList),
                        PrefsGUIEditorRosettaUIComponent.CreateObjectField(objPrefs.obj)
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
                => UI.Toggle(null,
                    () => IsChecked(prefs),
                    flag => PrefsCheckTable[prefs] = flag
                ).SetHeight(28f);
        }

        private static Element CreatePrefsListCheckElement(List<PrefsParam> prefsSet)
        {
            return UI.Toggle(null,
                () => prefsSet.Any(IsChecked),
                flag =>
                {
                    foreach (var prefs in prefsSet)
                    {
                        PrefsCheckTable[prefs] = flag;
                    }
                }
            );
        }
    }
}