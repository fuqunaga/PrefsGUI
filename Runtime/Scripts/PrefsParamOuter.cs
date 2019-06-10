using UnityEngine;

namespace PrefsGUI
{
    /// <summary>
    /// Define Outer Interface
    /// </summary>
    public abstract class PrefsParamOuter<OuterT> : PrefsParam
    {
        [SerializeField]
        protected OuterT defaultValue;

        public PrefsParamOuter(string key, OuterT defaultValue = default(OuterT)) : base(key)
        {
            this.defaultValue = defaultValue;
        }

        public static implicit operator OuterT(PrefsParamOuter<OuterT> me)
        {
            return me.Get();
        }


        #region abstract

        public abstract OuterT Get();

        public abstract void Set(OuterT v);

        #endregion


        #region override

        public override void SetCurrentToDefault() { defaultValue = Get(); }

        #endregion
    }
}