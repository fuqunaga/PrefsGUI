using System;

namespace PrefsGUI.Example
{
    public enum EnumSample
    {
        One,
        Two,
        Three
    }

    [Serializable]
    public class CustomClass
    {
        public string name;
        public int intValue;

        public CustomClass()
        {
        }

        public CustomClass(CustomClass other)
        {
            name = other.name;
            intValue = other.intValue;
        }
    }
}