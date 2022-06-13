using System.Linq;

namespace PrefsGUI
{
    public static class PrefsKeyUtility
    {
        public static char separator = '.';

        public static string GetPrefix(string key)
        {
            var units = key.Split(separator);
            return (units.Length <= 1)
                ? null
                : units.First();
        }
    }
}