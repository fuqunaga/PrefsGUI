using UnityEngine;

namespace PrefsGUI.Example
{
    [CreateAssetMenu(fileName = nameof(PrefsGUIOnScriptableObject), menuName = "PrefsGUI.Example/PrefsGUIOnScriptableObject")]
    public class PrefsGUIOnScriptableObject : ScriptableObject
    {
        public PrefsInt prefsInt = new("PrefsIntInScriptableObject");
    }
}