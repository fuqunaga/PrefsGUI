using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class GUIUtil
{
    public class ColorScope : IDisposable
    {
        Color _color;
        public ColorScope(Color color)
        {
            _color = GUI.color;
            GUI.color = color;
        }

        public void Dispose()
        {
            GUI.color = _color;
        }
    }
}
