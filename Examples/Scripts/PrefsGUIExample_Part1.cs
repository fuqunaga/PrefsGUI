using System.Collections.Generic;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIExample_Part1 : MonoBehaviour, IDoGUI
    {
        #region Type Define

        public enum EnumSample
        {
            One,
            Two,
            Three
        }

        [System.Serializable]
        public class CustomClass
        {
            public string name;
            public int intValue;

            public CustomClass()
            {
            }

            public CustomClass(CustomClass other)
            {
                name = other.name;
                intValue = other.intValue;
            }
        }

        [System.Serializable]
        public class PrefsEnum : PrefsParam<EnumSample>
        {
            public PrefsEnum(string key, EnumSample defaultValue = default) : base(key, defaultValue)
            {
            }
        }

        [System.Serializable]
        public class PrefsClass : PrefsAny<CustomClass>
        {
            public PrefsClass(string key, CustomClass defaultValue = default) : base(key, defaultValue)
            {
            }
        }

        [System.Serializable]
        public class PrefsList : PrefsList<CustomClass>
        {
            public PrefsList(string key, List<CustomClass> defaultValue = null) : base(key, defaultValue)
            {
            }
        }

        #endregion

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
        public PrefsClass prefsClass = new PrefsClass("PrefsClass");
        public PrefsList prefsList = new PrefsList("PrefsList");

        public void DoGUI()
        {
            prefsBool.DoGUI();
 
            // Return true if value was changed
            var changed = prefsInt.DoGUI();
            if (changed)
            {
                // Implicitly convert
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
            prefsClass.DoGUI();
            prefsList.DoGUI();
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

            CustomClass customClass = prefsClass;
            List<CustomClass> list = prefsList;
        }
    }
}