using System.Linq;
using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsSearch
    {
        public static Element CreateElement()
        {
            var ps = PrefsSearchCore.Instance;

            return UI.Column(
                UI.Field(null, () => ps.SearchWord),
                UI.Box(
                    UI.ScrollViewVertical(800f,
                        UI.DynamicElementOnStatusChanged(
                            () => ps.SearchWord,
                            _ => UI.Page(
                                ps.PrefsList.Select(prefs => prefs.CreateElement()))
                        )
                    )
                ).SetMinWidth(500f)
            );
        }
    }
}