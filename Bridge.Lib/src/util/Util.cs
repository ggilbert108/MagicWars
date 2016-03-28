using System;

namespace Bridge.Lib
{
    public class Util
    {
        public static bool ValueInRange(int value, int min, int max)
        {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }
    }
}