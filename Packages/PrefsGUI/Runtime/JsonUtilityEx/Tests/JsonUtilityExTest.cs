using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace PrefsGUI.Test
{

    public class JsonUtilityExTest : MonoBehaviour
    {
        public enum Enum
        {
            One,
            Two,
            Three
        }


        [Test]
        public void TestConsistency()
        {
            TestConsistency(Enum.One);
            TestConsistency("string");
            TestConsistency(1);
            TestConsistency(1.0);
            TestConsistency(Vector2.one);
            TestConsistency(Vector3.one);
            TestConsistency(Vector4.one);
            TestConsistency(Vector2Int.one);
            TestConsistency(Vector3Int.one);
            TestConsistency(Color.white);
            TestConsistency(new Rect(1f, 1f, 1f, 1f));
            TestConsistency(new RectOffset(1, 1, 1, 1), new SimpleComparer<RectOffset>());
            TestConsistency(new Bounds(Vector3.one, Vector3.one));
            TestConsistency(new BoundsInt(Vector3Int.one, Vector3Int.one));
            TestConsistency<int[]>(new[] { 1, 2, 3 }, new EnumerableComparer<int>()); // Type inference that T is IEnumerable<int>
            TestConsistency<List<int>>(new[] { 1, 2, 3 }.ToList(), new EnumerableComparer<int>());// Type inference that T is IEnumerable<int>
        }


        void TestConsistency<T>(T v, IEqualityComparer<T> comparer = null)
        {
            var json = JsonUtilityEx.ToJson(v);
            var newV = JsonUtilityEx.FromJson<T>(json);

            if (comparer != null)
            {
                Assert.AreEqual(v, newV, $"{typeof(T).Name} NOT Consistency", comparer);
            }
            else
            {
                Assert.AreEqual(v, newV, $"{typeof(T).Name} NOT Consistency");
            }
        }

        public class SimpleComparer<T> : IEqualityComparer<T>
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
                return x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<T> obj)
            {
                return obj.Aggregate(0, (hash, element) => hash |= element.GetHashCode());
            }
        }
    }
}