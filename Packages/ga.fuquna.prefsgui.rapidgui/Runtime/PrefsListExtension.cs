using System.Collections.Generic;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsListExtension
    {
        public static bool DoGUI<T>(this PrefsList<T> prefs, string label = null)
        {
            return prefs.DoGUIStandard(
                (v) => RGUI.ListField(
                    v,
                    label ?? prefs.key,
                    customElementGUI: (list, idx, elemLabel) => prefs.DoGUIAt_(list, idx, elemLabel),
                    customLabelRightFunc: (list) =>
                    {
                        list = RGUI.ListLabelRightFunc(list);

                        var defaultValueCount = prefs.DefaultValueCount;
                        if (GUIComponent.DoGUIDefaultButton(prefs.IsDefaultCount))
                        {
                            list ??= new();
                            var listCount = list.Count;
                            if (defaultValueCount > listCount)
                            {
                                list.AddRange(prefs.DefaultValue.GetRange(listCount, defaultValueCount - listCount));
                            }
                            else if (defaultValueCount < listCount)
                            {
                                list.RemoveRange(defaultValueCount, listCount - defaultValueCount);
                            }
                        }

                        return list;
                    }
                ),
                false);
        }

        static T DoGUIAt_<T>(this PrefsList<T> prefs, List<T> list, int idx, string label)
        {
            using (new GUILayout.HorizontalScope())
            {
                var ret = list[idx];
                ret = RGUI.Field(ret, label);

                // defaultButton
                if (idx < prefs.DefaultValueCount)
                {
                    var dv = prefs.DefaultValue[idx];
                    if (GUIComponent.DoGUIDefaultButton(PrefsAnyUtility.IsEqual(ret, dv)))
                    {
                        ret = dv;
                    }
                }

                return ret;
            }
        }
        
                

        public static bool DoGUIAt<T>(this PrefsList<T> prefs, int idx, string label = null)
        {
            var list = prefs.Get();
            var prev = list[idx];

            var prevInner = PrefsAnyUtility.ToInner(prev);

            var next = prefs.DoGUIAt_(list, idx, label);
            var nextInner = PrefsAnyUtility.ToInner(next);

            var changed = (prevInner != nextInner);
            if (changed)
            {
                list[idx] = next;
                prefs.Set(list);
            }

            return changed;
        }

    }
}