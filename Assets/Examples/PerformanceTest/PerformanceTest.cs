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
        public List<PrefsFloat> prefsFloats = new();

        private void Start()
        {
            uiRoot.Build(CreateElement());
        }

        private void Update()
        {
            if (prefsFloats.Count != count)
            {
                ResetPrefs();
            }

            if (updateValues)
            {
                foreach (var prefs in prefsFloats)
                {
                    prefs.Set(Random.value);
                }
            }
        }
        
        void ResetPrefs()
        {
            prefsFloats = Enumerable.Range(0, count)
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
                    idx => (0 <= idx && idx < prefsFloats.Count) ? prefsFloats[idx].CreateElement() : null
                )
            );
        }
    }
}