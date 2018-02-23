using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class GUIUtil
{
	public static int IntButton(int v, string label = "")
	{
		using (var h = new GUILayout.HorizontalScope())
		{
			v = Field(v, label);
			const float width = 20f;
			if (GUILayout.Button("+", GUILayout.Width(width))) v++;
			if (GUILayout.Button("-", GUILayout.Width(width))) v--;
		}
		return v;
	}
}
