using UnityEngine;

public static partial class GUIUtil
{
    public static float prefixWidth = 128;

    public static void PrefixLabel(string text)
    {
        PrefixLabel(text, GUI.skin.label);
    }

    public static void PrefixLabel(string text, GUIStyle style)
    {
        if (string.IsNullOrEmpty(text))
        {
            GUILayout.Label(text, style);
        }
        else
        {
            Prefix((width) => GUILayout.Label(text, style, GUILayout.Width(width)));
        }
    }

    public static void Prefix(System.Action<float> action)
    {
        action(prefixWidth);
    }

    public static T Prefix<T>(System.Func<float, T> func)
    {
        return func(prefixWidth);
    }
}