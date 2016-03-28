namespace Bridge.Lib
{
    public class Color
    {
        public string ColorName { get; private set; }

        public Color(string name)
        {
            ColorName = name;
        }

        public static Color Black
        {
            get { return new Color("black");}
        }

        public static Color Gray
        {
            get { return new Color("gray"); }
        }

        public static Color Green
        {
            get { return new Color("green"); }
        }

        public static Color Orange
        {
            get { return new Color("orange"); }
        }

        public static Color Red
        {
            get { return new Color("red");}
        }

        public static Color Blue
        {
            get { return new Color("blue"); }
        }

        public static Color Purple
        {
            get { return new Color("purple"); }
        }
    }
}