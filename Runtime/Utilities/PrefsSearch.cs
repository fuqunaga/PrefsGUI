using RapidGUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PrefsGUI
{
    /// <summary>
    /// Show and search all Prefs
    /// </summary>
    public class PrefsSearch
    {
        #region static

        static PrefsSearch Instance = new PrefsSearch();

        PrefsSearch() { }


        public static void DoGUI()
        {
            Instance.DoGUI_();
        }

        #endregion


        string word;
        FastScrollView scrollView = new FastScrollView();
        List<PrefsParam> prefsList = new List<PrefsParam>();

        void DoGUI_()
        {
            var newWord = GUILayout.TextField(word);
            if (word != newWord)
            {
                word = newWord;
                UpdateList();
            }

            scrollView.DoGUI(prefsList, (prefs) => prefs.DoGUI());
        }

        void UpdateList()
        {
            if (string.IsNullOrEmpty(word))
            {
                prefsList.Clear();
            }
            else
            {
                var lword = word.ToLower();
                prefsList = PrefsParam.all.Where(prefs => prefs.key.ToLower().Contains(lword)).OrderBy(prefs => prefs.key).ToList();
            }

            scrollView.SetNeedUpdateLayout();
        }
    }
}