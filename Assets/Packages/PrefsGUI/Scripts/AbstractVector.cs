using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

public static class AbstractVector {

    static readonly Dictionary<Type, int> _typeRankTable = new Dictionary<Type, int>()
    {
        {typeof(Vector2), 2 },
        {typeof(Vector3), 3 },
        {typeof(Vector4), 4 },
    };

    public static int GetElementNum<T>() { return _typeRankTable[typeof(T)]; }

    public static float GetAtIdx<T>(object o, int idx)
    {
        return (float)typeof(T).InvokeMember("Item", BindingFlags.GetProperty, null, o, new object[] { idx });
    }

    public static void SetAtIdx<T>(object o, int idx, float v)
    {
        typeof(T).InvokeMember("Item", BindingFlags.SetProperty, null, o, new object[] { idx, v });
    }
}
