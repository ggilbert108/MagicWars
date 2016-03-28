using System;

namespace Bridge.Lib
{
    public class Random
    {
        public int Next(int max)
        {
            return (int) Math.Floor(NextDouble()*max);
        }

        public double NextDouble()
        {
            return Math.Random();
        }

        public int Next(int min, int max)
        {
            return Next(max - min) + min;
        }
    }
}