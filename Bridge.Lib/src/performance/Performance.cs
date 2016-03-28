namespace Bridge.Lib
{
    public static class Performance
    {
        public static int Now()
        {
            int result = Script.Eval<int>("performance.now()");

            return result;
        }
    }
}