using System;
using UnityEditor;
using UnityEngine;

namespace PrefsGUI
{
    public abstract class PrefsGUIEditorBase : EditorWindow
    {

        // GUI.skin is not same in Editor and runtime.
        // StyleScope set style like runtime in Editor;
        public class StyleScope : IDisposable
        {
            static GUIStyle label;
            static GUIStyle button;
            
            static StyleScope()
            {
                label = new GUIStyle(GUI.skin.label);
                label.wordWrap = true;

                button = new GUIStyle(GUI.skin.button);
                button.richText = true;
            }


            GUIStyle labelOrig;
            GUIStyle buttonOrig;

            public StyleScope()
            {
                labelOrig = GUI.skin.label;
                buttonOrig = GUI.skin.button;

                GUI.skin.label = label;
                GUI.skin.button = button;
            }

            public void Dispose()
            {
                GUI.skin.label = labelOrig;
                GUI.skin.button = buttonOrig;
            }
        }


        protected void Update()
        {
            GameObjectPrefsUtility.UpdateGoPrefs();
        }

        void OnGUI()
        {
            using (new StyleScope())
            {
                OnGUIInternal();
            }
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