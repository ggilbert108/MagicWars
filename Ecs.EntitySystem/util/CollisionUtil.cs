using System;
using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public static class CollisionUtil
    {
        public static Rectangle GetBounds(Entity entity)
        {
            var position = entity.GetComponent<Location>().Position;
            var radius = entity.GetComponent<Shape>().Radius;

            return new Rectangle(
                (int)position.X - radius,
                (int)position.Y - radius,
                radius * 2, radius * 2);
        }

        public static bool CouldCollide(Entity a, Entity b)
        {
            return GetBounds(a).IntersectsWith(GetBounds(b));
        }

        public static bool Collide(Entity mover, Entity blocker)
        {
            Vector2 normal = Vector2.Zero;
            return Collide(mover, blocker, ref normal);
        }

        public static Vector2 GetNormal(Entity mover, Entity blocker)
        {
            Vector2 normal = Vector2.Zero;
            Collide(mover, blocker, ref normal);
            return normal;
        }

        private static bool Collide(Entity mover, Entity blocker, ref Vector2 normal)
        {
            var moverShape = mover.GetComponent<Shape>();
            var blockerShape = blocker.GetComponent<Shape>();

            if (moverShape.IsCircle && blockerShape.IsCircle)
            {
                return CircleCollidesWithCircle(mover, blocker, ref normal);
            }
            else if (moverShape.IsCircle)
            {
                return CircleCollidesWithPolygon(mover, blocker, ref normal);
            }
            else if (blockerShape.IsCircle)
            {
                bool result = CircleCollidesWithPolygon(blocker, mover, ref normal);
                normal *= -1;
                return result;
            }
            else
            {
                return PolygonCollidesWithPolygon(mover, blocker, ref normal);
            }
        }

        private static bool CircleCollidesWithCircle(Entity mover, Entity blocker, ref Vector2 normal)
        {
            var moverPos = mover.GetComponent<Location>().Position;
            var blockerPos = blocker.GetComponent<Location>().Position;
            var moverRadius = mover.GetComponent<Shape>().Radius;
            var blockerRadius = blocker.GetComponent<Shape>().Radius;

            Vector2 between = moverPos - blockerPos;
            float distance = between.Length;

            if (distance > moverRadius + blockerRadius)
                return false;

            normal = VectorUtil.RotateVector(between, Math.PI/2);

            return true;
        }

        private static bool CircleCollidesWithPolygon(Entity circle, Entity polygon, ref Vector2 normal)
        {
            var circleShape = circle.GetComponent<Shape>();
            var polygonShape = polygon.GetComponent<Shape>();
            var circlePosition = circle.GetComponent<Location>().Position;
            var polygonPosition = polygon.GetComponent<Location>().Position;

            var angle = EntityUtil.GetRotation(polygon);

            normal = Vector2.Zero;
            double minDistance = double.MaxValue;
            for (int i = 0; i < polygonShape.Sides; i++)
            {
                Vector2 p1 = polygonShape.GetVertex(i, angle) + polygonPosition;
                Vector2 p2 = polygonShape.GetVertex(i + 1, angle) + polygonPosition;

                if (LineIntersectsCircle(p1, p2, circlePosition, circleShape.Radius))
                {
                    Vector2 lineCenter = VectorUtil.GetMidpoint(p1, p2);
                    double distance = (circlePosition - lineCenter).Length;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        float dx = p2.X - p1.X;
                        float dy = p2.Y - p1.Y;
                        normal = new Vector2(dy, -dx);
                    }
                }
            }

            return normal != Vector2.Zero;
        }

        private static bool PolygonCollidesWithPolygon(Entity mover, Entity blocker, ref Vector2 normal)
        {
            var moverShape = mover.GetComponent<Shape>();
            var blockerShape = blocker.GetComponent<Shape>();
            var moverPosition = mover.GetComponent<Location>().Position;
            var blockerPosition = blocker.GetComponent<Location>().Position;
            var moverAngle = EntityUtil.GetRotation(mover);
            var blockerAngle = EntityUtil.GetRotation(blocker);


            normal = new Vector2(0, 0);
            double minDistance = double.MaxValue;
            for (int i = 0; i < moverShape.Sides; i++)
            {
                Vector2 a1 = moverShape.GetVertex(i, moverAngle) + moverPosition;
                Vector2 a2 = moverShape.GetVertex(i + 1, moverAngle) + moverPosition;

                for (int j = 0; j < blockerShape.Sides; j++)
                {
                    Vector2 b1 = blockerShape.GetVertex(j, blockerAngle) + blockerPosition;
                    Vector2 b2 = blockerShape.GetVertex(j + 1, blockerAngle) + blockerPosition;

                    if (LinesIntersect(a1, a2, b1, b2))
                    {
                        Vector2 lineCenter = VectorUtil.GetMidpoint(b1, b2);
                        double distance = (moverPosition - lineCenter).Length;

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            float dx = b2.X - b1.X;
                            float dy = b2.Y - b1.Y;
                            normal = new Vector2(dy, -dx);
                        }
                    }
                }
            }

            return normal != Vector2.Zero;
        }

        private static bool LinesIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            Vector2 cmp = b1 - a1;
            Vector2 r = a2 - a1;
            Vector2 s = b2 - b1;

            double cmpxr = cmp.X * r.Y - cmp.Y * r.X;
            double cmpxs = cmp.X * s.Y - cmp.Y * s.X;
            double rxs = r.X * s.Y - r.Y * s.X;

            if (Math.Abs(cmpxr) < 0.01)
            {
                return ((b1.X - a1.X < 0) != (b1.X - a2.X < 0)) ||
                       ((b1.Y - a1.Y < 0) != (b1.Y - a2.Y < 0));

            }

            if (Math.Abs(rxs) < 0.01)
                return false;

            double rxsr = 1.0 / rxs;
            double t = cmpxs * rxsr;
            double u = cmpxr * rxsr;

            return (t >= 0) && (t <= 1) && (u >= 0) && (u <= 1);
        }

        private static bool LineIntersectsCircle(Vector2 p1, Vector2 p2, Vector2 center, int radius)
        {
            Vector2 d = p2 - p1;
            Vector2 f = p1 - center;

            double a = Vector2.Dot(d, d);
            double b = 2 * Vector2.Dot(f, d);
            double c = Vector2.Dot(f, f) - radius * radius;

            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                return false;
            }

            discriminant = Math.Sqrt(discriminant);

            double t1 = (-b - discriminant) / (2 * a);
            double t2 = (-b + discriminant) / (2 * a);

            if (t1 >= 0 && t1 <= 1)
            {
                return true;
            }

            if (t2 >= 0 && t2 <= 1)
            {
                return true;
            }

            return false;
        }
    }
}