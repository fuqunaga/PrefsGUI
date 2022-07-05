using System.Collections.Generic;
using System.Linq;
using PrefsGUI.Editor;
using RapidGUI;
using UnityEditor;
using UnityEngine;

namespace PrefsGUI.RapidGUI.Editor
{
    public class SetCurrentToDefaultWindowRapidGUI : PrefsGUIEditorRapidGUIBase
    {
        public PrefsGUIEditorRapidGUI parentWindow;

        protected override void OnGUIInternal()
        {
            var objPrefsKeysList = PrefsAssetUtility.ObjPrefsList
                .Select(objPrefs => ( objPrefs, prefsList: objPrefs.PrefsAll.Where(prefs => !prefs.IsDefault)))
                .Where(pair => pair.prefsList.Any())
                .Select(pair =>
                {
                    var objPrefs = pair.objPrefs;
                    return (objPrefs, keys : pair.prefsList.Select(prefs => (objPrefs.obj, prefs)).ToList());
                })
                .ToList();


            EditorGUILayout.HelpBox("\nSelect Prefs to change Default.\n", MessageType.None);


            CheckGoPrefsListGUI(objPrefsKeysList);


            var checkedPrefsExists = objPrefsKeysList.Any(objPrefsKeys => objPrefsKeys.keys.Any(key => checkedList[key]));

            GUILayout.Space(8f);

            using (new RGUI.EnabledScope(checkedPrefsExists))
            {
                if (GUILayout.Button("SetCurrentToDefault"))
                {
                    foreach (var objPrefsKeys in objPrefsKeysList)
                    {
                        var checkedPrefsList = objPrefsKeys.keys.Where(key => checkedList[key]).Select(key => key.prefs).ToList();
                        var objs = checkedPrefsList.Select(prefs => objPrefsKeys.objPrefs.GetPrefsParent(prefs)).Distinct().ToArray();

                        Undo.RecordObjects(objs, "Set PrefsGUI default value");
                        
                        // SetCurrent To Default
                        foreach (var prefs in checkedPrefsList)
                        {
                            prefs.SetCurrentToDefault();
                        }


                        foreach (var obj in objs)
                        {
                            PrefsGUIEditorUtility.SavePrefabAssetIfNeed(obj);
                        }
                    }


                    Close();
                    parentWindow.Repaint();
                }
            }
        }


        Vector2 scrollPosition;

        protected Dictionary<(Object, PrefsParam), bool> checkedList = new Dictionary<(Object, PrefsParam), bool>();


        public void CheckGoPrefsListGUI(List<(PrefsAssetUtility.ObjPrefs objPrefs, List<(Object, PrefsParam prefs)> keys)>  objPrefsKeysList)
        {
            // set new keys true
            var newKeys = objPrefsKeysList
                .SelectMany(ok => ok.keys)
                .Except(checkedList.Keys);

            foreach (var key in newKeys)
            {
                checkedList[key] = true;
            }


            // check all
            var trueCount = checkedList.Values.Count(c => c);

            var checkAll = ToggleMixed(trueCount, checkedList.Count);
            if (checkAll.HasValue)
            {
                foreach (var key in checkedList.Keys.ToList())
                {
                    checkedList[key] = checkAll.Value;
                }
            }

            // per object
            using var sc = new GUILayout.ScrollViewScope(scrollPosition);
            scrollPosition = sc.scrollPosition;

            foreach (var (objPrefs, keys) in objPrefsKeysList)
            {
                using (new GUILayout.HorizontalScope())
                {
                    var check = ToggleMixed(keys.Count(key => checkedList[key]), keys.Count);
                    if (check.HasValue)
                    {
                        foreach (var key in keys)
                        {
                            checkedList[key] = check.Value;
                        }
                    }

                    using (new RGUI.EnabledScope(false))
                    {
                        EditorGUILayout.ObjectField(objPrefs.obj, typeof(Object), true);
                    }
                }


                using (new RGUI.IndentScope())
                {
                    foreach(var key in keys)
                    {
                        var check = checkedList[key];

                        using (new GUILayout.HorizontalScope())
                        {
                            if (check != GUILayout.Toggle(check, GUIContent.none, ToggleWidth)) checkedList[key] = !check;
                            key.prefs.DoGUI();
                        }
                    }
                }
            }
        }
    }
}