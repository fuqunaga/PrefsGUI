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

        public static readonly PrefsSearch Instance = new();

        PrefsSearch() { }


        public static void DoGUI()
        {
            Instance.DoGUI_();
        }

        #endregion

        
        private string _lastSearchWord;

        public string SearchWord
        {
            get => _lastSearchWord;
            set
            {
                if (_lastSearchWord != value)
                {
                    _lastSearchWord = value;
                    UpdateList();
                }
            }
        }

        public List<PrefsParam> PrefsList { get; protected set; } = new();

        readonly FastScrollView _scrollView = new();
        

        void DoGUI_()
        {
            SearchWord = GUILayout.TextField(SearchWord);

            _scrollView.DoGUI(PrefsList, (prefs) => prefs.DoGUI());
        }

        void UpdateList()
        {
            if (string.IsNullOrEmpty(SearchWord))
            {
                PrefsList.Clear();
            }
            else
            {
                var lowerWord = SearchWord.ToLower();
                PrefsList = PrefsParam.all.Where(prefs => prefs.key.ToLower().Contains(lowerWord)).OrderBy(prefs => prefs.key).ToList();

                //Debug.Log("InvalidKey:" + string.Join("\n", PrefsParam.allDic.Where(pair => pair.Key != pair.Value.key).Select(pair => pair.Key + ":" + pair.Value.key).ToArray()));
            }

            _scrollView.SetNeedUpdateLayout();
        }
    }
}