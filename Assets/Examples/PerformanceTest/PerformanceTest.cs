using System;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI.RosettaUI;
using RosettaUI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace PrefsGUI.Sync.Example
{
    public class PerformanceTest : MonoBehaviour
    {
        public class MyClass
        {
            public int intValue;
            public string stringValue;
        }
        
        public RosettaUIRoot uiRoot;
        public Vector2 windowPosition;
        
        public int count = 10000;
        public bool updatePrefsValues = true;
        public bool enablePrefsFloat = true;
        public bool enablePrefsString = true;
        public bool enablePrefsAny = true;
        public int showPrefsIndex;
        public List<PrefsFloat> prefsFloats;
        public List<PrefsString> prefsStrings;
        [FormerlySerializedAs("prefsClasses")] public List<PrefsAny<MyClass>> prefsAnys;

        private static List<string> _stringValues = new();
        
        public void Start()
        {
            ResetPrefs();
            uiRoot.Build(CreateElement());
        }

        private void Update()
        {
            if (updatePrefsValues)
            {
                UpdatePrefs(enablePrefsFloat, prefsFloats, p => p.Set(Random.value));
                UpdatePrefs(enablePrefsString, prefsStrings, p => p.Set(_stringValues[Random.Range(0,count)]));
                UpdatePrefs(enablePrefsAny, prefsAnys, p => p.Set(new MyClass()
                {
                    intValue = Random.Range(0, int.MaxValue),
                    stringValue = _stringValues[Random.Range(0, count)]
                }));
            }

            void UpdatePrefs<T>(bool enable, List<T> prefsList, Action<T> updateAction)
            {
                if (!enable) return;
                foreach (var prefs in prefsList)
                {
                    updateAction(prefs);
                }
            }
        }

        void ResetPrefs()
        {
            prefsFloats = Enumerable.Range(0, count)
                .Select(i => new PrefsFloat(nameof(PrefsFloat) + i))
                .ToList();
            
            prefsStrings = Enumerable.Range(0, count)
                .Select(i => new PrefsString(nameof(PrefsString) + i))
                .ToList();

            prefsAnys = Enumerable.Range(0, count)
                .Select(i => new PrefsAny<MyClass>(nameof(PrefsAny<MyClass>) + i))
                .ToList();

            if (_stringValues.Count <= count)
            {
                _stringValues = Enumerable.Range(0, count).Select(i => i.ToString()).ToList();
            }
        }

        public Element CreateElement()
        {
            return UI.Window(nameof(PerformanceTest),
                UI.Page(
                    UI.Field(() => count).RegisterValueChangeCallback(ResetPrefs),
                    UI.Field(() => updatePrefsValues),
                    UI.Field(() => enablePrefsFloat),
                    UI.Field(() => enablePrefsString),
                    UI.Field(() => enablePrefsAny),
                    UI.Box(
                        UI.DynamicElementOnStatusChanged(
                            () => count,
                            max => UI.Slider(() => showPrefsIndex, max - 1)
                        ),
                        UI.DynamicElementOnStatusChanged(
                            () => showPrefsIndex,
                            (idx) =>
                            {
                                if (0 <= idx && idx < prefsFloats.Count)
                                {
                                    return UI.Column(
                                        UI.DynamicElementIf(() => enablePrefsFloat,
                                            () => prefsFloats[idx].CreateElement()),
                                        UI.DynamicElementIf(() => enablePrefsString,
                                            () => prefsStrings[idx].CreateElement()),
                                        UI.DynamicElementIf(() => enablePrefsAny, () => prefsAnys[idx].CreateElement())
                                    );
                                }

                                return null;
                            })
                    )
                )
            ).SetPosition(windowPosition);
        }
    }
}