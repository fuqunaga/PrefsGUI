using System;
using UnityEngine;

public static partial class GUIUtil
{
	public static void Indent(Action action) { Indent(1, action); }
	public static void Indent(int level, Action action)
	{
		const int TAB = 20;
		using (var h = new GUILayout.HorizontalScope())
		{
			GUILayout.Space(TAB * level);
			using (var v = new GUILayout.VerticalScope())
			{
				action();
			}
		}
	}
}
