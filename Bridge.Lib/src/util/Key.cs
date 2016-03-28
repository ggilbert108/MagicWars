namespace Bridge.Lib
{
    public class Key : IHashable
    {
        private int keyCode;
        public Key(int keyCode)
        {
            this.keyCode = keyCode;
        }

        public static Key A
        {
            get { return new Key(65); }
        }

        public static Key S
        {
            get { return new Key(83); }
        }

        public static Key D
        {
            get { return new Key(68); }
        }

        public static Key W
        {
            get { return new Key(87); }
        }

        public int GetHash()
        {
            return keyCode;
        }
    }
}