using UnityEditor;
using UnityEngine;

namespace PrefsGUI
{
    public abstract class PrefsGUIEditorBase : EditorWindow
    {
        protected void Update()
        {
            GameObjectPrefsUtility.UpdateGoPrefs();
        }

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


        protected bool? ToggleMixed(int count, int maxCount)
        {
            var mixedFlag = (count == 0) ? false : ((count == maxCount) ? (bool?)true : null);
            return ToggleMixed(mixedFlag);
        }

        protected bool? ToggleMixed(bool? mixedFlag)
        {
            var value = (mixedFlag == null) || mixedFlag.Value;
            if (value != GUILayout.Toggle(value, "", (mixedFlag==null) ? "ToggleMixed" : GUI.skin.toggle, GUILayout.Width(16f)))
            {
                return !value;
            }

            return null;
        }
    }
}