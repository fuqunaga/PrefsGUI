using UnityEngine;
using System.Collections.Generic;
using RapidGUI;
using System.Net;

namespace PrefsGUI
{
    public class PrefsGUIExample : PrefsGUIExampleBase
    {
        [System.Serializable]
        public class PrefsEnum : PrefsParam<EnumSample>
        {
            public PrefsEnum(string key, EnumSample defaultValue = default) : base(key, defaultValue)
            { }
        }

        [System.Serializable]
        public class CustomClass
        {
            public string name;
            public int intValue;
        }

        [System.Serializable]
        public class PrefsList : PrefsList<CustomClass>
        {
            public PrefsList(string key, List<CustomClass> defaultValue = null) : base(key, defaultValue)
            {
            }
        }

        // define PrefsParams with key.
        public PrefsBool prefsBool = new PrefsBool("PrefsBool");
        public PrefsInt prefsInt = new PrefsInt("PrefsInt");
        public PrefsFloat prefsFloat = new PrefsFloat("PrefsFloat");
        public PrefsString prefsString = new PrefsString("PrefsString");
        public PrefsEnum prefsEnum = new PrefsEnum("PrefsEnum");
        public PrefsColor prefsColor = new PrefsColor("PrefsColor");
        public PrefsVector2 prefsVector2 = new PrefsVector2("PrefsVector2");
        public PrefsVector3 prefsVector3 = new PrefsVector3("PrefsVector3");
        public PrefsVector4 prefsVector4 = new PrefsVector4("PrefsVector4");
        public PrefsVector2Int prefsVector2Int = new PrefsVector2Int("PrefsVector2Int");
        public PrefsVector3Int prefsVector3Int = new PrefsVector3Int("PrefsVector3Int");
        public PrefsRect prefsRect = new PrefsRect("PrefsRect");
        public PrefsBounds prefsBounds = new PrefsBounds("PrefsBounds");
        public PrefsBoundsInt prefsBoundsInt = new PrefsBoundsInt("PrefsBoundsInt");
        
        public PrefsIPEndPoint prefsIPEndPoint = new PrefsIPEndPoint("PrefsIPEndPoint");
        public PrefsList prefsList = new PrefsList("PrefsList");


        protected override void DoGUI()
        {
            prefsBool.DoGUI();
            var changed = prefsInt.DoGUI();
            // return true if value was changed
            if (changed)
            {
                // use as native type
                int intValue = prefsInt;
                Debug.Log("Changed. " + intValue);
            }

            prefsFloat.DoGUI();
            prefsFloat.DoGUISlider();
            prefsString.DoGUI();
            prefsEnum.DoGUI();
            prefsColor.DoGUI();
            prefsVector2.DoGUI();
            prefsVector2.DoGUISlider();
            prefsVector3.DoGUI();
            prefsVector3.DoGUISlider();
            prefsVector4.DoGUI();
            prefsVector4.DoGUISlider();

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
            prefsList.DoGUI();


            GUILayout.Label($"file path: {PrefsGUI.KVS.PrefsKVSPathSelector.path}");

            if (GUILayout.Button("Save")) Prefs.Save();
            if (GUILayout.Button("DeleteAll")) Prefs.DeleteAll();
        }


        void Update()
        {
            TestImplicitCast();
        }

        protected void TestImplicitCast()
        {
            bool b = prefsBool;
            int i = prefsInt;
            float f = prefsFloat;
            string s = prefsString;
            EnumSample e = prefsEnum;
            Color c = prefsColor;
            Vector2 v2 = prefsVector2;
            Vector3 v3 = prefsVector2;
            Vector4 v4 = prefsVector2;
            v2 = prefsVector3;
            v3 = prefsVector3;
            v4 = prefsVector3;
            v2 = prefsVector4;
            v3 = prefsVector4;
            v4 = prefsVector4;
            Vector2Int v2Int = prefsVector2Int;
            Vector3Int v3Int = prefsVector3Int;
            Rect rect = prefsRect;
            Bounds bounds = prefsBounds;
            BoundsInt boundsInt = prefsBoundsInt;

            IPEndPoint ip = prefsIPEndPoint;
            List<CustomClass> list = prefsList;
        }
    }
}
