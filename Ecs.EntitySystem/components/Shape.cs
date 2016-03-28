using System;
using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Shape : Component
    {
        public int Sides;
        public int Radius;
        public Color Color;

        public Shape(int sides, int radius, Color color)
        {
            Sides = sides == 0 ? 360 : sides;
            Radius = radius;
            Color = color;
        }

        public Vector2 GetVertex(int index, float angleOffset = 0)
        {
            double angle = Math.PI*2*index/Sides;
            angle += angleOffset;
            float x = (float) (Radius * Math.Cos(angle));
            float y = (float) (Radius * Math.Sin(angle));
            return new Vector2(x, y);
        }

        public bool IsCircle
        {
            get { return Sides == 360; }
        }
    }
}