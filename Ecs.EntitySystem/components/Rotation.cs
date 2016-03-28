using System;
using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public class Rotation : Component
    {
        private float angle;
        private float baseAngle;

        public Rotation()
        {
            angle = baseAngle = 0;
            AddDependency<Shape>();
        }

        public override void OnCreate(Entity entity)
        {
            var shape = entity.GetComponent<Shape>();

            Vector2 side = shape.GetVertex(0) - shape.GetVertex(1);
            Vector2 horizontal = Vector2.UnitX;

            baseAngle = VectorUtil.AngleBetween(horizontal, side);
            baseAngle += (float) Math.PI;
            Angle = 0;
        }

        public float Angle
        {
            get { return angle; }
            set { angle = value + baseAngle; }
        }
    }
}