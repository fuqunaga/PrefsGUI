using RapidGUI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static PrefsGUI.Editor.GameObjectPrefsUtility;

namespace PrefsGUI.Editor
{
    public class SetCurrentToDefaultWindow : PrefsGUIEditorBase
    {
        public PrefsGUIEditor parentWindow;

        protected override void OnGUIInternal()
        {
            var gpListNonDeffault = goPrefsList.Select(gp => new GoPrefs()
            {
                go = gp.go,
                monoPrefsList = gp.monoPrefsList
                    .Select(mp => new MonoPrefs() { mono = mp.mono, prefsList = mp.prefsList.Where(p => !p.IsDefault).ToList() })
                    .Where(mp => mp.prefsList.Any())
                   .ToList()
            })
            .Where(gp => gp.monoPrefsList.Any())
            .ToList();


            EditorGUILayout.HelpBox("\nSelect Prefs to change Default.\n", MessageType.None);


            CheckGoPrefsListGUI(gpListNonDeffault);


            var checkedMonoPrefs = gpListNonDeffault
                .SelectMany(gp => gp.monoPrefsList.SelectMany(mpList => mpList.prefsList.Where(p => checkedList[(gp.go, p)]).Select(p => (mpList.mono, p))));


            GUILayout.Space(8f);

            using (new RGUI.EnabledScope(checkedMonoPrefs.Any()))
            {
                if (GUILayout.Button("SetCurrentToDefault"))
                {
                    var monos = checkedMonoPrefs.Select(mp => mp.mono).ToArray();
                    var checkedPrefs = checkedMonoPrefs.Select(mp => mp.p).ToList();


                    Undo.RecordObjects(monos, "Set PrefsGUI default value");


                    // SetCurrent To Default
                    checkedPrefs.ForEach(prefs => prefs.SetCurrentToDefault());

                    monos.ToList().ForEach(mono => EditorUtility.SetDirty(mono));


                    Close();
                    parentWindow.Repaint();
                }
            }
        }


        Vector2 scrollPosition;

        protected Dictionary<(GameObject, PrefsParam), bool> checkedList = new Dictionary<(GameObject, PrefsParam), bool>();


        public void CheckGoPrefsListGUI(List<GoPrefs> gpList)
        {
            // set new keys true
            gpList
                .SelectMany(gp => gp.keys)
                .Except(checkedList.Keys)
                .ToList()
                .ForEach(key => checkedList[key] = true);



            // check all
            var trueCount = checkedList.Values.Where(c => c).Count();

            var checkAll = ToggleMixed(trueCount, checkedList.Count);
            if (checkAll.HasValue)
            {
                checkedList.Keys.ToList().ForEach(key => checkedList[key] = checkAll.Value);
            }

            // per gameobject
            using (var sc = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = sc.scrollPosition;

                gpList.ForEach(gp =>
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        var keys = gp.keys.ToList();

                        var check = ToggleMixed(keys.Where(key => checkedList[key]).Count(), keys.Count);
                        if (check.HasValue)
                        {
                            keys.ForEach(key => checkedList[key] = check.Value);
                        }

                        using (new RGUI.EnabledScope(false))
                        {
                            EditorGUILayout.ObjectField(gp.go, typeof(GameObject), true);
                        }
                    }


                    using (new RGUI.IndentScope())
                    {
                        gp.prefsList.ToList().ForEach(prefs =>
                        {
                            var key = (gp.go, prefs);
                            bool check = checkedList[key];

                            using (new GUILayout.HorizontalScope())
                            {
                                if (check != GUILayout.Toggle(check, GUIContent.none, ToggleWidth)) checkedList[key] = !check;
                                prefs.DoGUI();
                            }
                        });
                    }
                });
            }
        }
    }
}