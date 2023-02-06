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
                    UI.Page(
                        UI.DynamicElementOnStatusChanged(
                            () => ps.SearchWord,
                            _ => UI.List(
                                () => ps.PrefsList,
                                (binder, idx) => UI.Field(null, binder),
                                new ListViewOption(false, true, false)
                            )
                        )
                        // ここで高さ制限しないとUIToolkitのListView初期化がうまくいかない@Unity2021.3
                        // 本来はUIToolkit側修正すべきだが大変そうなので暫定対処
                    ).SetMinWidth(850f).SetMaxHeight(1000f) 
                )
            );
        }
    }
}