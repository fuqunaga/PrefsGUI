using PrefsGUI.Example;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.RosettaUI.Example
{
    [RequireComponent(typeof(RosettaUIRoot))]
    public class PrefsGUIRosettaUIExample : MonoBehaviour
    {
        public Vector2 position;
        
        private void Start()
        {
            var root = GetComponent<RosettaUIRoot>();
            root.Build(CreateElement());
        }

        private Element CreateElement()
        {
            return UI.Window(
                "PrefsGUI - RosettaUI",
                UI.WindowLauncher<PrefsGUIExample_Primitive>("Primitive"),
                UI.WindowLauncher<PrefsGUIRosettaUIExample_Slider>("Slider"),
                UI.WindowLauncher<PrefsGUIRosettaUIExample_MinMaxSlider>("MinMaxSlider"),
                UI.WindowLauncher<PrefsGUIExample_Dictionary>("Dictionary"),
                UI.WindowLauncher(UI.Window(nameof(PrefsSearch), PrefsSearch.CreateElement())),
                UI.Space().SetHeight(15f),
                UI.Label(() => $"file path: {Kvs.PrefsKvsPathSelector.Path}"),
                UI.Button(nameof(Prefs.Save), Prefs.Save),
                UI.Button(nameof(Prefs.DeleteAll), Prefs.DeleteAll)
            ).SetPosition(position);
        }
    }
}