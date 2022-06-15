using System;
using NUnit.Framework;

namespace PrefsGUI.Test
{
    public class SameKeyTest
    {
        [Test]
        public void SameKey()
        {
            void Test()
            {
                var prefs0 = new PrefsBool("key");
                var prefs1 = new PrefsFloat("key");

                prefs0.Get();
                prefs1.Get();
            }

            Assert.Catch<InvalidCastException>(Test);
        }
    }
}
