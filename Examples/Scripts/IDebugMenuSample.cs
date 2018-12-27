using UnityEngine;

namespace PrefsGUI
{
    public class IDebugMenuSample : MonoBehaviour, GUIUtil.IDebugMenu
    {
        bool _countup = true;
        int _count;
        public void Update()
        {
            if (_countup) _count++;
        }


        public void DebugMenu()
        {
            GUILayout.Label("IDebugMenuSample");
            GUIUtil.Indent(() =>
            {
                _countup = GUILayout.Toggle(_countup, "CountUp");
                GUILayout.Label("Count: " + _count);
            });
        }
    }
}