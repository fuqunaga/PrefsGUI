using PrefsGUI.KVS;
using RapidGUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrefsGUI
{
    /// <summary>
    /// Basic implementation of OuterT and InnnerT
    /// </summary>
    public abstract class PrefsParamOuterInner<OuterT, InnerT> : PrefsParamOuter<OuterT>
    {
        protected bool isCachedOuter;
        protected OuterT cachedOuter;

        protected bool isCachedObj;
        protected object cachedObj;

        protected bool synced;

        protected bool hasDefaultInner;
        protected InnerT defaultInner;


        public PrefsParamOuterInner(string key, OuterT defaultValue = default) : base(key, defaultValue)
        {
        }

        protected InnerT GetDefaultInner()
        {
            if (!hasDefaultInner)
            {
                defaultInner = ToInner(defaultValue);
                hasDefaultInner = true;
            }

            return defaultInner;
        }

        protected InnerT _Get()
        {
            return PrefsKVS.Get(key, GetDefaultInner());
        }

        protected void _Set(InnerT v, bool synced = false, Action onIfAlreadyGet = null)
        {
            if (false == Compare(v, _Get()))
            {
                if (onIfAlreadyGet != null && !this.synced && synced)
                {
                    if (isCachedOuter || isCachedObj)
                    {
                        onIfAlreadyGet();
                    }
                }

                PrefsKVS.Set(key, v);
                isCachedOuter = false;
                isCachedObj = false;
            }

            this.synced = synced;
        }

        protected virtual bool Compare(InnerT lhs, InnerT rhs) => EqualityComparer<InnerT>.Default.Equals(lhs, rhs);

        protected virtual Dictionary<string, string> GetCustomLabel() => null;



        #region abstract

        protected abstract OuterT ToOuter(InnerT innerV);
        protected abstract InnerT ToInner(OuterT outerV);

        #endregion



        #region override

        public override OuterT Get()
        {
            if (!isCachedOuter)
            {
                cachedOuter = ToOuter(_Get());
                isCachedOuter = true;
            }
            return cachedOuter;
        }

        public override void Set(OuterT v) { _Set(ToInner(v)); }

        public override Type GetInnerType()
        {
            return typeof(InnerT);
        }
        public override object GetObject()
        {
            if (!isCachedObj)
            {
                cachedObj = _Get();
                isCachedObj = true;
            }

            return cachedObj;
        }
        public override void SetSyncedObject(object obj, Action onIfAlreadyGet = null)
        {
            _Set((InnerT)obj, true, onIfAlreadyGet);
        }

        public override bool IsDefault => Compare(GetDefaultInner(), _Get());

        public override void SetCurrentToDefault()
        {
            defaultValue = Get();
            hasDefaultInner = false;
        }


        public override bool DoGUI(string label = null)
        {
            return DoGUIStrandard((v) => RGUI.Field(v, label ?? key));
        }

        #endregion



        #region GUI Implement

        protected delegate InnerT GUIFunc(InnerT v);

        protected bool DoGUIStrandard(GUIFunc func)
        {
            var customLabel = GetCustomLabel();
            if (customLabel != null) RGUI.BeginCustomLabel(customLabel);
            if (synced) RGUI.BeginColor(syncedColor);

            var changed = false;
            using (new GUILayout.HorizontalScope())
            {
                changed = DoGUICheckChanged(key, func);
                changed |= DoGUIDefaultButton();
            }

            if (synced) RGUI.EndColor();
            if (customLabel != null) RGUI.EndCustomLabel();

            return changed;
        }

        // public for Custom GUI
        public bool DoGUIDefaultButton()
        {
            var label = Compare(_Get(), ToInner(defaultValue)) ? "default" : "<color=red>default</color>";

            var ret = GUILayout.Button(label, GUILayout.ExpandWidth(false));
            if (ret)
            {
                Set(defaultValue);
            }

            return ret;
        }

        protected bool DoGUICheckChanged(string key, GUIFunc func)
        {
            var changed = false;
            if (!PrefsKVS.HasKey(key))
            {
                Set(defaultValue);
                changed = true;
            }

            var prev = _Get();
            var next = func(prev);
            if (!Compare(prev, next))
            {
                _Set(next);
                changed = true;
            }

            return changed;
        }

        #endregion
    }
}