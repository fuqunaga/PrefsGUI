using System;
using PrefsGUI.Editor;
using UnityEditor;
using UnityEngine;
using RapidGUI;

namespace PrefsGUI.RapidGUI.Editor
{
    public abstract class PrefsGUIEditorRapidGUIBase : EditorWindow
    {
        #region Type Define 

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
            float prefixLabelWidth;

            public StyleScope()
            {
                labelOrig = GUI.skin.label;
                buttonOrig = GUI.skin.button;
                prefixLabelWidth = RGUI.PrefixLabelSetting.width;

                GUI.skin.label = label;
                GUI.skin.button = button;

                RGUI.PrefixLabelSetting.width = 200f;
            }

            public void Dispose()
            {
                GUI.skin.label = labelOrig;
                GUI.skin.button = buttonOrig;

                RGUI.PrefixLabelSetting.width = prefixLabelWidth;
            }
        }

        #endregion


        public static readonly GUILayoutOption ToggleWidth = GUILayout.Width(8f);

        void OnGUI()
        {
            using (new StyleScope())
            {
                OnGUIInternal();
            }
        }

        protected abstract void OnGUIInternal();


        public static  bool? ToggleMixed(int count, int maxCount)
        {
            var mixedFlag = (count == 0) ? false : ((count == maxCount) ? (bool?)true : null);
            return ToggleMixed(mixedFlag);
        }

        public static bool? ToggleMixed(bool? mixedFlag)
        {
            var value = (mixedFlag == null) || mixedFlag.Value;
            if (value != GUILayout.Toggle(value, "", (mixedFlag==null) ? "ToggleMixed" : GUI.skin.toggle, ToggleWidth))
            {
                return !value;
            }

            return null;
        }
    }
}