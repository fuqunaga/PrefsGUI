using System;
using System.Collections;
using RosettaUI;

namespace PrefsGUI.RosettaUI
{
    public static class PrefsGUIExtensionListBase
    {
        public static Element CreateElement<TList, TListForUI>(
            this PrefsListBase<TList, TListForUI> prefs,
            in ListViewOption? option = null
        )
            where TList : new()
            where TListForUI : IList
        {
            return prefs.CreateElement(null, option);
        }

        public static Element CreateElement<TList, TListForUI>(
            this PrefsListBase<TList, TListForUI> prefs,
            LabelElement label,
            in ListViewOption? option = null
        )
            where TList : new()
            where TListForUI : IList
        {
            var optionNotNull = option ?? ListViewOption.Default;
            var createItemElementFuncOriginal = optionNotNull.createItemElementFunc ?? UI.ListItemDefault;
            optionNotNull.createItemElementFunc = (binder, index) =>
            {
                var field = createItemElementFuncOriginal(binder, index);
                return (index < prefs.DefaultValueCount)
                    ? UI.Row(
                        field,
                        prefs.CreateDefaultButtonElementAt(index)
                    )
                    : field;
            };
            
            
            var listAccessor = prefs.GetListAccessor();
            var element = UI.Row(
                UI.List(
                    label ?? UI.Label(() => prefs.key),
                    () => listAccessor.InnerList,
                    option
                ),
                prefs.CreateDefaultButtonElement()
            );
            
            PrefsGUIExtension.SubscribeSyncedFlag(prefs, element);

            return element;
        }

        
        #region Obsolete signatures
        
        [Obsolete("Use ListViewOption to set createItemElementFunc")]
        public static Element CreateElement<TList, TListForUI>(
            this PrefsListBase<TList, TListForUI> prefs,
            Func<IBinder, int, Element> createItemElementFunc
        )
            where TList : new()
            where TListForUI : IList
        {
            return prefs.CreateElement(null, null, createItemElementFunc);
        }
        
        [Obsolete("Use ListViewOption to set createItemElementFunc")]
        public static Element CreateElement<TList, TListForUI>(
            this PrefsListBase<TList, TListForUI> prefs,
            LabelElement label,
            Func<IBinder, int, Element> createItemElementFunc
        )
            where TList : new()
            where TListForUI : IList
        {
            return prefs.CreateElement(label, null, createItemElementFunc);
        }
        
        [Obsolete("Use ListViewOption to set createItemElementFunc")]
        public static Element CreateElement<TList, TListForUI>(
            this PrefsListBase<TList, TListForUI> prefs,
            in ListViewOption? option,
            Func<IBinder, int, Element> createItemElementFunc
        )
            where TList : new()
            where TListForUI : IList
        {
            return prefs.CreateElement(null, option, createItemElementFunc);
        }

        [Obsolete("Use ListViewOption to set createItemElementFunc")]
        public static Element CreateElement<TList, TListForUI>(
            this PrefsListBase<TList, TListForUI> prefs,
            LabelElement label,
            in ListViewOption? option,
            Func<IBinder, int, Element> createItemElementFunc
        )
            where TList : new()
            where TListForUI : IList
        {
            var optionNotNull = option ?? ListViewOption.Default;
            optionNotNull.createItemElementFunc = createItemElementFunc;
            return prefs.CreateElement(label, optionNotNull);
        }
        
        #endregion


        public static Element CreateDefaultButtonElementAt<TList, TListForUI>(this PrefsListBase<TList, TListForUI> prefs, int index) 
            where TList : new()
            where TListForUI : IList
        {
            return PrefsGUIElement.CreateDefaultButtonElement(
                onClick: () => prefs.ResetToDefaultAt(index),
                isDefault: () => prefs.IsDefaultAt(index)
            );
        }
    }
}