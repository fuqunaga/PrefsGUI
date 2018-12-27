using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

public class AbstractVector
{
    public class Data
    {
        public int rank;
        public PropertyInfo pi;
    }

    static readonly Dictionary<Type, Data> _dataTable = new Dictionary<Type, Data>()
    {
        { typeof(Vector2), new Data() { rank = 2 } },
        { typeof(Vector3), new Data() { rank = 3 } },
        { typeof(Vector4), new Data() { rank = 4 } },
        { typeof(Vector2Int), new Data(){rank = 2} },
        { typeof(Vector3Int), new Data(){rank = 3} },
    };

    static AbstractVector()
    {
        _dataTable.Keys.ToList().ForEach(type =>
        {
            _dataTable[type].pi = type.GetProperty("Item");
        });
    }


    object obj;
    Data data;

    public AbstractVector(object o)
    {
        obj = o;
        data = _dataTable[o.GetType()];
    }

    public static int GetElementNum<T>() => _dataTable[typeof(T)].rank;

    public int GetElementNum() => data.rank;

    public object this[int idx]
    {
        get
        {
            return data.pi.GetValue(obj, new[] { (object)idx });
        }

        set
        {
            data.pi.SetValue(obj, value, new[] { (object)idx });
        }
    }
}
