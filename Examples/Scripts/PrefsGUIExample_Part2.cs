using System.Collections.Generic;
using System.Net;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIExample_Part2 : MonoBehaviour, IDoGUI
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