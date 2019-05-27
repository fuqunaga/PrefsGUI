using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace PrefsGUI
{
    public class PrefsGUIEditor : PrefsGUIEditorBase
    {
        [MenuItem("Window/PrefsGUI")]
        public static void ShowWindow()
        {
            GetWindow<PrefsGUIEditor>("PrefsGUI");
        }


        public enum Order
        {
            AtoZ,
            GameObject,

        }

        public Order order { get; protected set; }


        PrefsGUIEditorImpl impl;


        Vector2 scrollPosition;
        SetCurrentToDefaultWindow setCurrentToDefaultWindow;

        
        public PrefsGUIEditor()
        {
            CreateImpl();
        }

        void CreateImpl()
        {
            var baseType = typeof(PrefsGUIEditorImpl);
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            var type = asms.SelectMany(asm => asm.GetTypes()).Where(t => t.IsSubclassOf(baseType)).FirstOrDefault();
            
            impl = (PrefsGUIEditorImpl)Activator.CreateInstance(type ?? baseType);
        }


        protected override void OnGUIInternal()
        {
            using (var h0 = new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save")) Prefs.Save();
                if (GUILayout.Button("Load")) Prefs.Load();
                if (GUILayout.Button("DeleteAll"))
                {
                    if (EditorUtility.DisplayDialog("DeleteAll", "Are you sure to delete all current prefs parameters?", "DeleteAll", "Don't Delete"))
                    {
                        Prefs.DeleteAll();
                    }
                }
            }

            var currentToDefaultEnable = !Application.isPlaying && allPrefs.Any(prefs => !prefs.IsDefault);
            GUI.enabled = currentToDefaultEnable;
            if (GUILayout.Button("Open Current To Default Window"))
            {
                if (setCurrentToDefaultWindow == null) setCurrentToDefaultWindow = CreateInstance<SetCurrentToDefaultWindow>();
                setCurrentToDefaultWindow.parentWindow = this;
                setCurrentToDefaultWindow.ShowUtility();
            }
            GUI.enabled = true;

            GUILayout.Space(8f);

            using (var h = new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Order");
                order = (Order)GUILayout.SelectionGrid((int)order, System.Enum.GetNames(typeof(Order)), 5);
            }

            GUILayout.Space(8f);

            using (var sc = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = sc.scrollPosition;

#if true
                impl.OnGUI(this);
#else
                AdditionalHeader();

                if (Order.GameObject == _order)
                {
                    goParams.Where(dic => dic.Key != null).OrderBy(dic => dic.Key.name).ToList().ForEach(pair =>
                    {
                        var go = pair.Key;
                        var prefsParams = pair.Value;

                        using (new GUILayout.HorizontalScope())
                        {
                            LabelWithEditPrefix(go.name, go, prefsParams);
                            GUILayout.FlexibleSpace();
                        }

                        GUIUtil.Indent(() =>
                        {
                            PrefsParamsGUI(prefsParams);
                        });
                    });
                }
                else
                {
                    PrefsParamsGUI(allPrefs.ToList());
                }
#endif
            }

            if ((setCurrentToDefaultWindow != null) && Event.current.type == EventType.Repaint) setCurrentToDefaultWindow.Repaint();
        }




        private void Update()
        {
#if true
            impl.Update();
#else
            var time = (float)EditorApplication.timeSinceStartup;
            if (time - lastTime > interaval)
            {
                UpdateCompParam();
                lastTime = time;
            }
#endif
        }
    }
}