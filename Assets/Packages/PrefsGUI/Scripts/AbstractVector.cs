using UnityEngine;
using System.Collections.Generic;
using System;

public static class AbstractVector
{

    public class Data
    {
        public int rank;
        public Func<object, int, float> get;
        public Func<object, int, float, object> set;
    };

    static readonly Dictionary<Type, Data> _dataTable = new Dictionary<Type, Data>()
    {
        {typeof(Vector2), new Data() { rank = 2, get=(o,idx) => ((Vector2)o)[idx], set = (o, idx, v) => { var vec =((Vector2)o); vec[idx] = v; return vec; } } },
        {typeof(Vector3), new Data() { rank = 3, get=(o,idx) => ((Vector3)o)[idx], set = (o, idx, v) => { var vec =((Vector3)o); vec[idx] = v; return vec; } } },
        {typeof(Vector4), new Data() { rank = 4, get=(o,idx) => ((Vector4)o)[idx], set = (o, idx, v) => { var vec =((Vector4)o); vec[idx] = v; return vec; } } },
    };

    public static int GetElementNum<T>() { return _dataTable[typeof(T)].rank; }

    public static float GetAtIdx<T>(object o, int idx)
    {
        return _dataTable[typeof(T)].get(o, idx);
    }

    public static object SetAtIdx<T>(object o, int idx, float v)
    {
        return _dataTable[typeof(T)].set(o, idx, v);
    }
}
