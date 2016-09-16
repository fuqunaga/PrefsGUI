using UnityEngine;
using System.Collections;
using PrefsGUI;
using System;

public class MaterialPropertyDebugMenuSample : PrefsGUISampleBase
{
    MaterialPropertyDebugMenu _debugMenu;

    public void Start()
    {
        _debugMenu = GetComponent<MaterialPropertyDebugMenu>();
    }

    protected override void OnGUIInternal()
    {
        _debugMenu.DebugMenu();
    }
}
