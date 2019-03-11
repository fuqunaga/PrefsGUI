#pragma warning disable 0219

using UnityEngine;
using System.Collections.Generic;

namespace PrefsGUI
{

    public class PrefsGUISample : PrefsGUISampleBase
    {
        [System.Serializable]
        public class PrefsEnum : PrefsParam<EnumSample>
        {
            public PrefsEnum(string key, EnumSample defaultValue = default(EnumSample)) : base(key, defaultValue) { }
        }

        [System.Serializable]
        public class CustomClass : GUIUtil.IDebugMenu
        {
            public string name;
            public int intValue;

            public void DebugMenu()
            {
                name = GUIUtil.Field(name ?? "", nameof(name));
                intValue = GUIUtil.Field(intValue, nameof(intValue));
            }
        }

        [System.Serializable]
        public class PrefsList : PrefsList<CustomClass> { public PrefsList(string key, System.Func<List<CustomClass>, List<CustomClass>> customOnGUI = null) : base(key, customOnGUI) { } }

        // define PrefsParams with key.
        public PrefsEnum _prefsEnum = new PrefsEnum("PrefsEnum");
        public PrefsString _prefsString = new PrefsString("PrefsString");
        public PrefsInt _prefsInt = new PrefsInt("PrefsInt");
        public PrefsFloat _prefsFloat = new PrefsFloat("PrefsFloat");
        public PrefsBool _prefsBool = new PrefsBool("PrefsBool");
        public PrefsVector2 _prefsVector2 = new PrefsVector2("PrefsVector2");
        public PrefsVector3 _prefsVector3 = new PrefsVector3("PrefsVector3");
        public PrefsVector4 _prefsVector4 = new PrefsVector4("PrefsVector4");
        public PrefsVector2Int _prefsVector2Int = new PrefsVector2Int("PrefsVector2Int");
        public PrefsVector3Int _prefsVector3Int = new PrefsVector3Int("PrefsVector3Int");

        public PrefsColor _prefsColor = new PrefsColor("PrefsColor");
        public PrefsRect _prefsRect = new PrefsRect("PrefsRect");
        public PrefsIPEndPoint _prefsIPEndPoint = new PrefsIPEndPoint("PrefsIPEndPoint");

        public PrefsList _prefsList = new PrefsList("PrefsList");


        protected override void OnGUIInternal()
        {
            _prefsEnum.OnGUI();
            _prefsString.OnGUI();
            _prefsInt.OnGUI();
            _prefsFloat.OnGUI();
            _prefsFloat.OnGUISlider();
            _prefsBool.OnGUI();
            _prefsVector2.OnGUI();
            _prefsVector2.OnGUISlider();
            _prefsVector3.OnGUI();
            _prefsVector3.OnGUISlider();
            _prefsVector4.OnGUI();
            _prefsVector4.OnGUISlider();
            _prefsVector2Int.OnGUI();
            _prefsVector2Int.OnGUISlider();
            _prefsVector3Int.OnGUI();
            _prefsVector3Int.OnGUISlider();
            _prefsRect.OnGUI();
            _prefsRect.OnGUISlider();
            _prefsColor.OnGUI();


            _prefsIPEndPoint.OnGUI();

            // return true if value was changed
            if (_prefsColor.OnGUISlider())
            {
                // use as native type
                Color color = _prefsColor;
                Debug.Log("Changed. " + color);
            }


            // PrefsList can call runtime element GUI
            _prefsList.OnGUI((element) =>
            {
                element.name = GUIUtil.Field(element.name ?? "", "name");
                element.intValue = GUIUtil.IntButton(element.intValue, "intValue");
            },
            "PrefsList runtime element GUI");


            _prefsList.OnGUI("PrefsList automaticaly call element.DebugMenu() if it is IDebugMenu");



            GUILayout.Label($"file path: {PrefsGUI.Wrapper.PrefsWrapperPathSelector.path}");

            if (GUILayout.Button("Save")) Prefs.Save();
            if (GUILayout.Button("DeleteAll")) Prefs.DeleteAll();
        }


        void Update()
        {
            TestImplicit();
        }

        protected void TestImplicit()
        {
            EnumSample e = _prefsEnum;
            string s = _prefsString;
            int i = _prefsInt;
            float f = _prefsFloat;
            bool b = _prefsBool;
            Vector2 v2 = _prefsVector2;
            Vector3 v3 = _prefsVector2;
            Vector4 v4 = _prefsVector2;
            v2 = _prefsVector3;
            v3 = _prefsVector3;
            v4 = _prefsVector3;
            v2 = _prefsVector4;
            v3 = _prefsVector4;
            v4 = _prefsVector4;
            Vector2Int v2Int = _prefsVector2Int;
            Vector3Int v3Int = _prefsVector3Int;
            Color c = _prefsVector4;
            Rect rect = _prefsRect;
            c = _prefsColor;
            v4 = _prefsColor;
        }
    }
}
