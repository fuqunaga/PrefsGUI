using UnityEngine;

namespace PrefsGUI
{
    /// <summary>
    /// Define Outer Interface
    /// </summary>
    public abstract class PrefsParamOuter<TOuter> : PrefsParam
    {
        [SerializeField]
        protected TOuter defaultValue;

        public TOuter DefaultValue => defaultValue;

        protected PrefsParamOuter(string key, TOuter defaultValue = default) : base(key)
        {
            this.defaultValue = defaultValue;
        }

        public static implicit operator TOuter(PrefsParamOuter<TOuter> me)
        {
            return me.Get();
        }


        #region abstract

        public abstract TOuter Get();

        /// <returns>true if the value is changed</returns>
        public abstract bool Set(TOuter v);

        #endregion


        #region override

        public override void ResetToDefault() => Set(DefaultValue);

        #endregion
    }
}