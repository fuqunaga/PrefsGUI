using System.Collections.Generic;
using RosettaUI;

namespace PrefsGUI.RosettaUI.Editor
{
    public interface IPrefsGUIEditorRosettaUIObjCheckExtension
    {
        public Element Title();
        public Element PrefsLeft(PrefsParam prefs);
        Element PrefsSetLeft(IEnumerable<PrefsParam> prefsList);
    }
}