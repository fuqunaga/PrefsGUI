using System.Collections.Generic;
using UnityEngine;

namespace PrefsGUI.Example
{
    public class PrefsGUIExample_Dictionary : MonoBehaviour
    {
        public PrefsDictionary<string, int> prefsStringIntDictionary = new("StringIntDictionary");
        public PrefsDictionary<string, List<int>> prefsStringListDictionary = new("StringListDictionary");
        public PrefsDictionary<int, CustomClass> prefsIntClassDictionary = new("IntClassDictionary");
        public PrefsDictionary<EnumSample, CustomClass> prefsEnumClassDictionary = new("EnumClassDictionary");
        public PrefsDictionary<Vector2, CustomClass> prefsVector2ClassDictionary = new("Vector2ClassDictionary");
    }
}