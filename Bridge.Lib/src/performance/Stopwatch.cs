using System;

namespace Bridge.Lib
{
    public class Stopwatch
    {
        private int startTime = -1;

        public Stopwatch()
        {
            
        }

        public void Restart()
        {
            startTime = Performance.Now();
        }

        public int GetElapsedMilliseconds()
        {
            if (startTime == -1)
            {
                throw new Exception("The stopwatch has not been started");
            }

            return Performance.Now() - startTime;
        }
    }
}