using System.Net;
using PrefsGUI.RapidGUI;
using PrefsGUI.RosettaUI;
using RapidGUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIExample_Part2 : MonoBehaviour, IDoGUI, IElementCreator
    {
        // define PrefsParams with key.
        public PrefsVector2Int prefsVector2Int = new PrefsVector2Int("PrefsVector2Int");
        public PrefsVector3Int prefsVector3Int = new PrefsVector3Int("PrefsVector3Int");
        public PrefsRect prefsRect = new PrefsRect("PrefsRect");
        public PrefsBounds prefsBounds = new PrefsBounds("PrefsBounds");
        public PrefsBoundsInt prefsBoundsInt = new PrefsBoundsInt("PrefsBoundsInt");
        public PrefsIPEndPoint prefsIPEndPoint = new PrefsIPEndPoint("PrefsIPEndPoint");

        public void DoGUI()
        {
            prefsVector2Int.DoGUI();
            prefsVector2Int.DoGUISlider();
            prefsVector3Int.DoGUI();
            prefsVector3Int.DoGUISlider();
            prefsRect.DoGUI();
            prefsRect.DoGUISlider();
            prefsBounds.DoGUI();
            prefsBounds.DoGUISlider();
            prefsBoundsInt.DoGUI();
            prefsBoundsInt.DoGUISlider();
            prefsIPEndPoint.DoGUI();
        }

        public Element CreateElement(LabelElement _)
        {
            return UI.Column(
                prefsVector2Int.CreateElement(),
                prefsVector2Int.CreateSlider(),
                prefsVector3Int.CreateElement(),
                prefsVector3Int.CreateSlider(),
                prefsRect.CreateElement(),
                prefsRect.CreateSlider(),
                prefsBounds.CreateElement(),
                prefsBounds.CreateSlider(),
                prefsBoundsInt.CreateElement(),
                prefsIPEndPoint.CreateElement()
            );
        }

        void Update()
        {
            TestImplicitCast();
        }

        protected void TestImplicitCast()
        {
            Vector2Int v2Int = prefsVector2Int;
            Vector3Int v3Int = prefsVector3Int;
            Rect rect = prefsRect;
            Bounds bounds = prefsBounds;
            BoundsInt boundsInt = prefsBoundsInt;

            IPEndPoint ip = prefsIPEndPoint;
        }
    }
}