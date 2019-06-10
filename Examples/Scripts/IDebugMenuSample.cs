using UnityEngine;
using RapidGUI;

namespace PrefsGUI
{
    public class IDebugMenuSample : MonoBehaviour, IDoGUI
    {
        bool countup = true;
        int count;
        public void Update()
        {
            if (countup) count++;
        }


        public void DoGUI()
        {
            GUILayout.Label("IDebugMenuSample");
            using (new RGUI.IndentScope())
            {
                countup = GUILayout.Toggle(countup, "CountUp");
                GUILayout.Label("Count: " + count);
            }
        }
    }
}