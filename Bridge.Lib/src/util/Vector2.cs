using System;

namespace Bridge.Lib
{
    public class Vector2
    {
        public float X, Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float Length
        {
            get { return (float) Math.Sqrt(X * X + Y * Y); } 
        }

        public void Normalize()
        {
            float length = Length;
            X /= length;
            Y /= length;
        }

        public void Rotate(float theta)
        {
            float newX = (float) (X*Math.Cos(theta) - Y*Math.Sin(theta));
            float newY = (float) (X*Math.Sin(theta) + Y*Math.Cos(theta));
            X = newX;
            Y = newY;
        }

        public void SetNaN()
        {
            X = float.NaN;
            Y = float.NaN;
        }

        public bool IsNaN()
        {
            return (float.IsNaN(X) && float.IsNaN(Y));
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator *(Vector2 v, float k)
        {
            return new Vector2(v.X*k, v.Y*k);
        }

        public static Vector2 operator /(Vector2 v, float k)
        {
            return new Vector2(v.X/k, v.Y/k);
        }

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }

        public static Vector2 GetMidpoint(Vector2 p1, Vector2 p2)
        {
            return new Vector2((p1.X + p2.X)/2, (p1.Y + p2.Y)/2);
        }

        public static double Dot(Vector2 a, Vector2 b)
        {
            return a.X*b.X + a.Y*b.Y;
        }

        public static double AngleBetween(Vector2 a, Vector2 b)
        {
            double angleA = Math.Atan2(a.Y, a.X);
            double angleB = Math.Atan2(b.Y, b.X);

            return angleB - angleA;
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (Math.Abs(dx) < 0.001) && (Math.Abs(dy) < 0.001);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }

        protected bool Equals(Vector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vector2)obj);
        }

        public override int GetHashCode()
        { 
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
        }

        public static Vector2 Zero
        {
            get
            {
                return new Vector2(0, 0);
            }
        }

        public static Vector2 UnitX
        {
            get
            {
                return new Vector2(1, 0);
            }
        }
    }
}