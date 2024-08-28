namespace PrefsGUI
{
    // Inner object accessor
    public interface IPrefsInnerAccessor<T>
    {
        PrefsParam Prefs { get; }
        bool IsAlreadyGet { get; }
        T Get();
        bool SetSyncedValue(T value);
        bool Equals(T lhs, T rhs);
        
        string Key => Prefs.key;
    }
}