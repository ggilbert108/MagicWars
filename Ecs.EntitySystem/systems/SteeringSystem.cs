using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public class SteeringSystem : Core.System
    {
        public SteeringSystem()
        {
            AddRequiredComponent<Steering>();
            AddRequiredComponent<Intelligence>();
            AddRequiredComponent<Location>();
            AddRequiredComponent<Intent>();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            var steering = entity.GetComponent<Steering>();
            var intelligence = entity.GetComponent<Intelligence>();
            var position = entity.GetComponent<Location>().Position;
            var intent = entity.GetComponent<Intent>();

            Vector2 wanderAround;
            if (intelligence.FollowingId != -1)
            {
                Entity following = Manager.GetEntityById(intelligence.FollowingId);
                wanderAround = following.GetComponent<Location>().Position;
            }
            else
            {
                wanderAround = position;
            }

            steering.Update(deltaTime, wanderAround);
            steering.Begin();
            steering.Wander();

            Vector2 force = steering.GetSteeringForce();
            intent.Queue.Add(new MoveIntent(force));
        }

        private Vector2 HeroPosition
        {
            get { return Hero.GetComponent<Location>().Position; }
        }
    }
}