using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PrefsGUI.RapidGUI;
using PrefsGUI.RosettaUI;
using PrefsGUI.Utility;
using RapidGUI;
using RosettaUI;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIExample_Part1 : MonoBehaviour, IDoGUI, IElementCreator
    {
        #region Type Define

        public enum EnumSample
        {
            One,
            Two,
            Three
        }

        [Serializable]
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

        #endregion

        // define PrefsParams with key.
        public PrefsBool              prefsBool     = new("PrefsBool");
        public PrefsInt               prefsInt      = new("PrefsInt");
        public PrefsFloat             prefsFloat    = new("PrefsFloat");
        public PrefsString            prefsString   = new("PrefsString");
        public PrefsParam<EnumSample> prefsEnum     = new("PrefsEnum");
        public PrefsColor             prefsColor    = new("PrefsColor");
        public PrefsGradient          prefsGradient = new("PrefsGradient");
        public PrefsVector2           prefsVector2  = new("PrefsVector2");
        public PrefsVector3           prefsVector3  = new("PrefsVector3");
        public PrefsVector4           prefsVector4  = new("PrefsVector4");
        public PrefsAny<CustomClass>  prefsClass    = new("PrefsClass");
        public PrefsList<CustomClass> prefsList     = new("PrefsList");
        public PrefsDictionary<string, int> prefsDictionary = new("PrefsDictionary");
        
        // public SerializableDictionary<string, int> dictionary = new();
        public List<SerializableDictionary<string, Color>> list = new();
        public SerializableDictionary<string, Color>[] ary = new SerializableDictionary<string, Color>[3];
        
        public void DoGUI()
        {
            prefsBool.DoGUI();

            // Return true if value was changed
            var changed = prefsInt.DoGUI();
            if (changed)
            {
                // Implicitly convert
                int intValue = prefsInt;
                Debug.Log("DoGUI: Changed. " + intValue);
            }

            prefsFloat.DoGUI();
            prefsFloat.DoGUISlider();
            prefsString.DoGUI();
            prefsEnum.DoGUI();
            prefsColor.DoGUI();
            // prefsGradient.DoGUI(); // not supported
            prefsVector2.DoGUI();
            prefsVector2.DoGUISlider();
            prefsVector3.DoGUI();
            prefsVector3.DoGUISlider();
            prefsVector4.DoGUI();
            prefsVector4.DoGUISlider();
            prefsClass.DoGUI();
            prefsList.DoGUI();

            if (prefsList.Count > 0)
            {
                prefsList.DoGUIAt(0, "PrefsList element at 0");
            }
        }

        private void Update()
        {
            TestImplicitCast();
        }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        [SuppressMessage("ReSharper", "NotAccessedVariable")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        protected void TestImplicitCast()
        {
            bool b = prefsBool;
            int i = prefsInt;
            float f = prefsFloat;
            string s = prefsString;
            EnumSample e = prefsEnum;
            Color c = prefsColor;
            Gradient g = prefsGradient;
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

        public Element CreateElement(LabelElement label)
        {
            return UI.Column(
                prefsBool.CreateElement(),
                prefsInt.CreateElement(),
                prefsFloat.CreateElement(),
                prefsFloat.CreateSlider(),
                prefsString.CreateElement(),
                prefsEnum.CreateElement(),
                prefsColor.CreateElement(),
                prefsGradient.CreateElement(),
                prefsVector2.CreateElement(),
                prefsVector2.CreateSlider(),
                prefsVector3.CreateElement(),
                prefsVector3.CreateSlider(),
                prefsVector4.CreateElement(),
                prefsVector4.CreateSlider(),
                prefsList.CreateElement()
            );
        }

        [ContextMenu(nameof(ChangeDictionary))]
        public void ChangeDictionary()
        {
            // dictionary.Add("Hoge", 1);
        }
    }
}