using PrefsGUI.Utility;
using RapidGUI;
using UnityEngine;

namespace PrefsGUI.RapidGUI
{
    public static class PrefsSearch
    {
        static readonly FastScrollView _scrollView = new();
        private static PrefsSearchCore Instance => PrefsSearchCore.Instance;

        static PrefsSearch()
        {
            Instance.onUpdateList += () => _scrollView.SetNeedUpdateLayout();
        }
        
        public static void DoGUI()
        {
            var instance = Instance;

            instance.SearchWord = GUILayout.TextField(instance.SearchWord);

            _scrollView.DoGUI(instance.PrefsList, (prefs) => prefs.DoGUI());
        }
    }
}