using System;

namespace Ecs.EntitySystem
{
    public static class Util
    {
        public static Random Rng = new Random();

        public static bool ValueInRange<T>(T value, T min, T max) where T : IComparable
        {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }
    }
}