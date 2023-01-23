using PrefsGUI.Utility;
using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsSearch
    {
        public static Element CreateElement()
        {
            var ps = PrefsSearchCore.Instance;

            return UI.Column(
                UI.Field(null, () => ps.SearchWord, new FieldOption() { delayInput = true }),
                UI.Box(
#if false
                    UI.ScrollViewVertical(800f,
                        UI.DynamicElementOnStatusChanged(
                            () => ps.SearchWord,
                            _ => UI.Page(
                                ps.PrefsList.Select(prefs => prefs.CreateElement()))
                        )
                    )
#else
                    UI.Page(
                        UI.DynamicElementOnStatusChanged(
                            () => ps.SearchWord,
                            _ => UI.List(
                                () => ps.PrefsList,
                                (binder, idx) => UI.Field(null, binder),
                                new ListOption(false, true, false)
                                ).SetMaxHeight(500f)
                        )
                    )
#endif
                ).SetMinWidth(500f)
            );
        }
    }
}