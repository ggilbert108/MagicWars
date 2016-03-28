using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public class Movement : Component
    {
        public Vector2 Velocity;
        public Vector2 LastMoved;
        public float MaxSpeed;
        public int MaxForce;

        public Movement(float maxSpeed, int maxForce = -1, Vector2? velocity = null)
        {
            MaxSpeed = maxSpeed;
            MaxForce = maxForce;
            LastMoved = Vector2.Zero;
            Velocity = velocity ?? Vector2.Zero;
        }

        public void Move(Vector2 force)
        {
            if (DoesAccelerate)
            {
                Velocity += VectorUtil.Trunctate(force, MaxForce);
                Velocity = VectorUtil.Trunctate(Velocity, MaxSpeed);
            }
            else
            {
                force.Normalize();
                Velocity = force * MaxSpeed;
            }
        }

        public void Stop()
        {
            Velocity = Vector2.Zero;
        }

        public bool DoesAccelerate
        {
            get { return MaxForce >= 0; }
        }
    }
}