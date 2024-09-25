#if UNITY_EDITOR

using UnityEditor;

namespace PrefsGUI
{
    /// <summary>
    /// EditorのPlayModeが変化したときにstatic変数をクリアする
    /// </summary>
    [InitializeOnLoad]
    public partial class PrefsParam
    {
        static PrefsParam()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state is PlayModeStateChange.ExitingPlayMode or PlayModeStateChange.ExitingEditMode)
            {
                foreach (var prefs in All)
                {
                    prefs.Reset();
                }

                All.Clear();
                AllDic.Clear();
                KeyToOnValueChangedCallback.Clear();
            }
        }
    }
}

#endif