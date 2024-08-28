using System;
using UnityEngine;

namespace PrefsGUI
{
    public partial class PrefsParam
    {
        public static Color syncedColor = new Color32(255, 143, 63, 255);

        public event Action<bool> onSyncedChanged;
        
        private bool _synced;
        public bool Synced {
            get => _synced;
            protected set
            {
                if (_synced != value)
                {
                    _synced = value;
                    onSyncedChanged?.Invoke(_synced);
                }
            }
        }
    }
}