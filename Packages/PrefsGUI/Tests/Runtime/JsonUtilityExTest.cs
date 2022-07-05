using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PrefsGUI.Utility;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace PrefsGUI.Test
{
    public class JsonUtilityExTest
    {
        public enum Enum
        {
            One,
            Two,
            Three
        }

        [TestCaseSource(nameof(JsonUtilityUnsupportedTestSource))]
        public void JsonUtilityUnsupportedTest<T>(T v)
        {
            var json = JsonUtility.ToJson(v);
            var newV = JsonUtility.FromJson<T>(json);

            Assert.AreNotEqual(v, newV,
                $"{typeof(T).Name} supported. before[{v}] after[{newV}] json[{json}]",
                GetEqualityComparer(v)
            );
        }

        [TestCaseSource(nameof(ValueConsistencyTestSource))]
        public void ValueConsistencyTest<T>(T v)
        {
            var json = JsonUtilityEx.ToJson(v);
            var newV = JsonUtilityEx.FromJson<T>(json);

            Assert.AreEqual(v, newV,
                $"{typeof(T).Name} NOT Consistency before[{v}] after[{newV}] json[{json}]",
                GetEqualityComparer(v)
            );
        }
        

        static TestCaseData[] JsonUtilityUnsupportedTestSource =
        {
            // new(Enum.Two),
            new("string"),
            new(1),
            new(1.0),
            // new(Vector2.one),
            // new(Vector3.one),
            // new(Vector4.one),
            new(Vector2Int.one),
            new(Vector3Int.one),
            // new(Color.white),
            new(new Rect(1f, 1f, 1f, 1f)),
            new(new RectOffset(1, 1, 1, 1)),
            new(new Bounds(Vector3.one, Vector3.one)),
            new(new BoundsInt(Vector3Int.one, Vector3Int.one)),
            new(new[] {1, 2, 3}),
            new(new[] {1, 2, 3}.ToList())
        };
        
        static TestCaseData[] ValueConsistencyTestSource = {
            new(Enum.Two),
            new("string"),
            new(1),
            new(1.0),
            new(Vector2.one),
            new(Vector3.one),
            new(Vector4.one),
            new(Vector2Int.one),
            new(Vector3Int.one),
            new(Color.white),
            new(new Rect(1f, 1f, 1f, 1f)),
            new(new RectOffset(1, 1, 1, 1)),
            new(new Bounds(Vector3.one, Vector3.one)),
            new(new BoundsInt(Vector3Int.one, Vector3Int.one)),
            new(new[] {1, 2, 3}),
            new(new[] {1, 2, 3}.ToList())
        };

        IEqualityComparer<T> GetEqualityComparer<T>(T v)
        {
            var type = typeof(T);
            
            IEqualityComparer<T> ret = EqualityComparer<T>.Default;
            if (!type.IsValueType && type != typeof(string))
            {
                ret = v switch
                {
                    IFormattable => new ToStringComparer<T>(),
                    IEnumerable => CreateEnumerableComparer(),
                    _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
                };
            }

            return ret;
            
            
            static IEqualityComparer<T> CreateEnumerableComparer()
            {
                var t = typeof(T);
                var itemType = t.IsArray
                    ? t.GetElementType()
                    : t.GetGenericArguments()[0];
                
                var comparerType = typeof(EnumerableComparer<>).MakeGenericType(itemType);
                return (IEqualityComparer<T>) Activator.CreateInstance(comparerType);
            }
        }
        
        public class ToStringComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return x.ToString() == y.ToString();
            }

            public int GetHashCode(T obj)
            {
                return obj.ToString().GetHashCode();
            }
        }

        public class EnumerableComparer<T> : IEqualityComparer<IEnumerable<T>>
        {
            public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
            {
                if (x == null && y == null) return true;
                
                return (x != null && y != null) && x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<T> obj)
            {
                return obj.Aggregate(0, (hash, element) => hash |= element.GetHashCode());
            }
        }
    }
}