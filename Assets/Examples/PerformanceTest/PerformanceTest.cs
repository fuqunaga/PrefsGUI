using System.Collections.Generic;
using System.Linq;
using PrefsGUI.RosettaUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PerformanceTest : MonoBehaviour
    {
        public RosettaUIRoot uiRoot;
        
        public int count = 10000;
        public bool updateValues = true;
        public int showPrefsIndex;
        public List<PrefsFloat> prefsList = new();

        private void Start()
        {
            ResetPrefs();
            uiRoot.Build(CreateElement());
        }

        private void Update()
        {
            if (prefsList.Count != count)
            {
                ResetPrefs();
            }

            if (updateValues)
            {
                foreach (var prefs in prefsList)
                {
                    prefs.Set(Random.value);
                }
            }
        }
        
        void ResetPrefs()
        {
            prefsList = Enumerable.Range(0, count)
                .Select(i => new PrefsFloat(nameof(PrefsFloat) + i))
                .ToList();
        }
        
        private Element CreateElement()
        {
            return UI.Window(
                UI.Field(() => count),
                UI.Field(() => updateValues),
                UI.DynamicElementOnStatusChanged(
                    () => count,
                    max => UI.Slider(() => showPrefsIndex, max)
                ),
                UI.DynamicElementOnStatusChanged(
                    () => showPrefsIndex,
                    idx => (0 <= idx && idx < prefsList.Count) ? prefsList[idx].CreateElement() : null
                )
            );
        }
    }
}