using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace PrefsGUI.Test.Editor
{
    public class TestMultiInstance
    {
        [SetUp]
        public void SetUp() => Prefs.DeleteAll();

        [TearDown]
        public void TearDown() => Prefs.DeleteAll();

        [TestCaseSource(nameof(PrefsSource))]
        public void GetAndSet<TPrefs, TValue>(TPrefs prefs0, TPrefs prefs1, TValue value) 
            where TPrefs : PrefsParamOuter<TValue>
        {
            Prefs.DeleteAll();
            
            prefs0.Get(); // get for making cache
            prefs1.Set(value);
            
            Assert.AreEqual(prefs0.Get(), prefs1.Get(), $"{prefs0.Get()} {prefs1.Get()}");
        }
        
        [TestCaseSource(nameof(PrefsSource))]
        public void ValueChangedCallback<TPrefs, TValue>(TPrefs prefs0, TPrefs prefs1, TValue value) 
            where TPrefs : PrefsParamOuter<TValue>
        {
            var prefs0EventCalled = false;
            var prefs1EventCalled = false;
            prefs0.RegisterValueChangedCallback(() => prefs0EventCalled = true);
            prefs1.RegisterValueChangedCallback(() => prefs1EventCalled = true);

            prefs0.Set(value);
            
            Assert.IsTrue(prefs0EventCalled, "prefs0 event is not called");
            Assert.IsTrue(prefs1EventCalled, "prefs1 event is not called");
        }
        

        private static TestCaseData[] PrefsSource()
        {
            return new[]
            {
                CreateData(true),
                CreateData(Color.yellow),
                CreateData(-1),
                CreateData<uint>(1),
                CreateData(1f),
                CreateData(Vector2.one),
                CreateData(Vector3.one),
                CreateData(Vector4.one),
                CreateData(Vector2Int.one),
                CreateData(Vector3Int.one),
                CreateData(new Rect(1f,1f,1f,1f)),
                CreateData(new Bounds(Vector3.one, Vector3.one)),
                CreateData(new BoundsInt(Vector3Int.one, Vector3Int.one)),
                new(new PrefsString("string"), new PrefsString("string"), "hoge"),
                new(new PrefsList<int>("list"), new PrefsList<int>("list"), new List<int> { 1 }),
            };


            TestCaseData CreateData<T>(T value) where T : struct
            {
                var key = typeof(T).ToString();
                return new TestCaseData(
                    new PrefsParam<T>(key),
                    new PrefsParam<T>(key),
                    value
                );
            }
        }
    }
}