using System;
using System.Drawing;
using OpenTK;

namespace Ecs.EntitySystem
{
    public static class VectorUtil
    { 
        public static Vector2 Trunctate(Vector2 vector, float max)
        {
            if (vector.Length > max)
            {
                vector.Normalize();
                vector *= max;
            }
            return vector;
        }

        public static Vector2 RotateVector(Vector2 vector, double angle)
        {
            float newX = (float) (vector.X * Math.Cos(angle) - vector.Y * Math.Sin(angle));
            float newY = (float) (vector.X * Math.Sin(angle) + vector.Y * Math.Cos(angle));

            return new Vector2(newX, newY);
        }

        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            float angleA = (float) Math.Atan2(a.Y, a.X);
            float angleB = (float) Math.Atan2(b.Y, b.X);

            return angleB - angleA;
        }

        public static Vector2 GetMidpoint(Vector2 p1, Vector2 p2)
        {
            float x = (p2.X - p1.X)/2f;
            float y = (p2.Y - p1.Y)/2f;
            return new Vector2(x, y);
        }

        public static Vector2 GetVectorInBounds(Rectangle bounds)
        {
            float x = (float) (Util.Rng.NextDouble()*bounds.Width + bounds.Left);
            float y = (float) (Util.Rng.NextDouble()*bounds.Height + bounds.Top);
            return new Vector2(x, y);
        }

        public static Vector2 GetVectorInRange(Vector2 vector, float range)
        {
            Vector2 result = Vector2.UnitX;
            float length = (float) (Util.Rng.NextDouble()*range);
            result *= length;

            float angle = (float) (Util.Rng.NextDouble()*Math.PI*2);
            result = RotateVector(result, angle);

            return result + vector;
        }
    }
}