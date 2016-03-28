namespace Bridge.Lib
{
    public class Rectangle
    {
        public int X, Y, Width, Height;

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool IntersectsWith(Rectangle other)
        {
            bool xOverlap = Util.ValueInRange(Left, other.Left, other.Right) ||
                            Util.ValueInRange(other.Left, Left, Right);

            bool yOverlap = Util.ValueInRange(Top, other.Top, other.Bottom) ||
                            Util.ValueInRange(other.Top, Top, Bottom);

            return xOverlap && yOverlap;
        }

        public bool Contains(Rectangle other)
        {
            return  other.Left >= Left && other.Right <= Right &&
                    other.Top >= Top && other.Bottom <= Bottom;
        }

        public Vector2 Center
        {
            get
            {
               return new Vector2(
                   (Left + Right)/2.0f,
                   (Top + Bottom)/2.0f);
            }
        }


        public int Top
        {
            get { return Y; }
        }

        public int Bottom
        {
            get { return Y + Height; }
        }

        public int Left
        {
            get { return X; }
        }

        public int Right
        {
            get { return X + Width; }
        }
    }
}