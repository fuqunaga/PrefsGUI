using UnityEngine;
using System.Collections.Generic;
using RapidGUI;

namespace PrefsGUI
{
    public class PrefsGUISample : PrefsGUISampleBase
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
        public PrefsEnum prefsEnum = new PrefsEnum("PrefsEnum");
        public PrefsString prefsString = new PrefsString("PrefsString");
        public PrefsInt prefsInt = new PrefsInt("PrefsInt");
        public PrefsFloat prefsFloat = new PrefsFloat("PrefsFloat");
        public PrefsBool prefsBool = new PrefsBool("PrefsBool");
        public PrefsVector2 prefsVector2 = new PrefsVector2("PrefsVector2");
        public PrefsVector3 prefsVector3 = new PrefsVector3("PrefsVector3");
        public PrefsVector4 prefsVector4 = new PrefsVector4("PrefsVector4");
        public PrefsVector2Int prefsVector2Int = new PrefsVector2Int("PrefsVector2Int");
        public PrefsVector3Int prefsVector3Int = new PrefsVector3Int("PrefsVector3Int");

        public PrefsColor prefsColor = new PrefsColor("PrefsColor");
        public PrefsRect prefsRect = new PrefsRect("PrefsRect");
        public PrefsIPEndPoint prefsIPEndPoint = new PrefsIPEndPoint("PrefsIPEndPoint");

        public PrefsList prefsList = new PrefsList("PrefsList");

        bool isOpen;

        protected override void OnGUIInternal()
        {
            prefsEnum.DoGUI();
            prefsString.DoGUI();
            prefsInt.DoGUI();
            prefsFloat.DoGUI();
            prefsFloat.OnGUISlider();
            prefsBool.DoGUI();
            prefsVector2.DoGUI();
            prefsVector2.DoGUISlider();
            RGUI.Slider(prefsVector2.Get(), Vector2.zero, "sinple", ref isOpen);

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
            prefsColor.DoGUI();


            prefsIPEndPoint.OnGUI();

            // return true if value was changed
            if (prefsColor.DoGUISlider())
            {
                // use as native type
                Color color = prefsColor;
                Debug.Log("Changed. " + color);
            }

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
            EnumSample e = prefsEnum;
            string s = prefsString;
            int i = prefsInt;
            float f = prefsFloat;
            bool b = prefsBool;
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
            Color c = prefsVector4;
            Rect rect = prefsRect;
            c = prefsColor;
            v4 = prefsColor;
        }
    }
}
