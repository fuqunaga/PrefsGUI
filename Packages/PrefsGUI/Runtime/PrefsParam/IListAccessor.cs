using System.Collections;

namespace PrefsGUI
{
    /// <summary>
    /// PrefsDictionary, PrefsListなどでUI用に内部のListにアクセスするためのクラス
    /// </summary>
    public interface IListAccessor<TList>
        where TList: IList
    {
        TList InnerList { get; set; }
    }
}