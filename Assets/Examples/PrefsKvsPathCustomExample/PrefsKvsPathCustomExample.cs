using PrefsGUI.Kvs;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsKvsPathCustomExample : MonoBehaviour
    {
        public RosettaUIRoot root;

        private void Start()
        {
            root.Build(CreateElement());
        }

        private Element CreateElement()
        {
            return UI.Window(nameof(PrefsKvsPathCustomExample),
                UI.Page(
                    UI.Field(() => Application.platform),
                    UI.Field(() => PrefsKvsPathSelector.path)
                    )
                );
        }
    }
}