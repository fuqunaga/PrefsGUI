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
        public class CustomClass
        {
            public string name;
            public int intValue;
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
        public PrefsColor _prefsColor = new PrefsColor("PrefsColor");
        public PrefsRect _prefsRect = new PrefsRect("PrefsRect");
        public PrefsList _prefsList = new PrefsList("PrefsList");
        public PrefsList _prefsListRuntimeGUI = new PrefsList("PrefsListRuntimeGUI");
        public PrefsList _prefsListCustomGUI = new PrefsList("PrefsListCustomGUI", (list) =>
        {
            list.ForEach(customClass =>
            {
                customClass.name = GUILayout.TextField(customClass.name ?? "");
                customClass.intValue = GUIUtil.IntButton(customClass.intValue);
            });
            using (var h = new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add")) list.Add(new CustomClass());
                if (GUILayout.Button("Remove")) list.RemoveAt(list.Count - 1);
            }
            return list;
        });


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
            _prefsRect.OnGUI();
            _prefsRect.OnGUISlider();

            _prefsColor.OnGUI();
            // return true if value was changed
            if (_prefsColor.OnGUISlider())
            {
                // use as native type
                Color color = _prefsColor;
                Debug.Log("Changed. " + color);
            }


            // default OnGUI() is NOT user friendly. but PrefsList can save/load parametors.
            _prefsList.OnGUI();


            GUILayout.Label("PrefsListRuntimeGUI");
            GUIUtil.Indent(() =>
            {
                var list = _prefsListRuntimeGUI.Get();
                list.ForEach(customClass =>
                {
                    customClass.name = GUILayout.TextField(customClass.name ?? "");
                    customClass.intValue = GUIUtil.IntButton(customClass.intValue);
                });
                using (var h = new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Add")) list.Add(new CustomClass() { name = "Elem" + list.Count });
                    if (GUILayout.Button("Remove")) list.RemoveAt(list.Count - 1);

                    _prefsListRuntimeGUI.Set(list);
                    _prefsList.OnGUIDefaultButton();
                }
            });

            // if you use OnGUI. to set cumstomOnGUI is bettor.
            _prefsListCustomGUI.OnGUI();

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
            Color c = _prefsVector4;
            Rect rect = _prefsRect;
            c = _prefsColor;
            v4 = _prefsColor;
        }
    }
}
