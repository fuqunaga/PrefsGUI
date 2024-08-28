using PrefsGUI.Kvs;

namespace PrefsGUI
{
    public static class Prefs
    {
        public static void Save() => PrefsKvs.Save();

        public static void Load()
        {
            ClearCache();
            PrefsKvs.Load();
        }

        public static void DeleteAll()
        {
            ClearCache();
            PrefsKvs.DeleteAll();
        }

        private static void ClearCache()
        {
            foreach (var prefs in PrefsParam.all)
            {
                prefs.ClearCache();
            }
        }
    }
}