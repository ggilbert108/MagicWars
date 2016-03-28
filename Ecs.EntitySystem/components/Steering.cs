using System;
using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public class Steering : Component
    {
        private const int SLOW_RADIUS = 20;
        private const float DECISION_FREQUENCY = .5f;
        private const float WANDER_RADIUS = 300;

        private Entity owner;
        private Vector2 steer;

        private Vector2 wanderDecision;
        private float currentDecisionTime;

        public Steering(Entity owner)
        {
            this.owner = owner;
            steer = Vector2.Zero;
            wanderDecision = Vector2.Zero;
            currentDecisionTime = (float) (Util.Rng.NextDouble()*DECISION_FREQUENCY);
        }

        public void Update(float deltaTime, Vector2 heroPosition)
        {
            currentDecisionTime += deltaTime;

            if (currentDecisionTime >= DECISION_FREQUENCY)
            {
                currentDecisionTime = 0;
                wanderDecision = VectorUtil.GetVectorInRange(heroPosition, WANDER_RADIUS);
            }
        }

        public void Begin()
        {
            steer = Vector2.Zero;
        }

        public Vector2 GetSteeringForce()
        {
            return steer;
        }

        public void Seek(Vector2 target, float scale = 1)
        {
            steer += GetSeek(target) * scale;
        }

        public void Wander(float scale = 1)
        {
            steer += GetSeek(wanderDecision) * scale;
        }

        private Vector2 GetSeek(Vector2 target)
        {
            Vector2 desired = target - Position;
            float distance = desired.Length;
            desired.Normalize();

            if (distance <= SLOW_RADIUS)
            {
                desired *= distance/SLOW_RADIUS;
            }
            desired *= MaxSpeed;

            return desired - Velocity;
        }

        private Vector2 Position
        {
            get { return owner.GetComponent<Location>().Position; }
        }

        private Vector2 Velocity
        {
            get { return owner.GetComponent<Movement>().Velocity; }
        }

        private float MaxSpeed
        {
            get { return owner.GetComponent<Movement>().MaxSpeed; }
        }
    }
}