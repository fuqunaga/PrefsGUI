using System;

namespace PrefsGUI
{
    // Inner object accessor
    public interface IPrefsInnerAccessor<T>
    {
        PrefsParam Prefs { get; }
        T Get();
        void SetSyncedValue(T value, Action onIfAlreadyGet = null);
        bool Equals(T lhs, T rhs);

        string Key => Prefs.key;
    }
}