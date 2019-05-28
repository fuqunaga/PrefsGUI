using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace PrefsGUI
{
    public abstract class PrefsGUIEditorBase : EditorWindow
    {
        public IEnumerable<PrefsParam> allPrefs => PrefsParam.all.Values.OrderBy(prefs => prefs.key);

        void OnGUI()
        {
            var button = GUI.skin.button;
            var tmp = button.richText;
            button.richText = true;

            OnGUIInternal();

            button.richText = tmp;
        }

        protected abstract void OnGUIInternal();
    }
}