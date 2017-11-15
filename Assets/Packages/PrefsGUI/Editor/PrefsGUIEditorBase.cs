using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace PrefsGUI
{
    public abstract class PrefsGUIEditorBase : EditorWindow
    {
        protected IEnumerable<PrefsParam> PrefsList { get { return PrefsParam.all.Values.OrderBy(prefs => prefs.key); } }

        void OnGUI()
        {
            var buttunStyleOrig = GUI.skin.button;
            var buttonStyle = new GUIStyle(buttunStyleOrig);
            buttonStyle.richText = true;
            GUI.skin.button = buttonStyle;

            OnGUIInternal();

            GUI.skin.button = buttunStyleOrig;
        }

        protected abstract void OnGUIInternal();
    }
}