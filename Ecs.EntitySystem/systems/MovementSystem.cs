using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public class MovementSystem : Core.System
    {
        public MovementSystem()
        {
            AddRequiredComponent<Movement>();
            AddRequiredComponent<Location>();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            var movement = entity.GetComponent<Movement>();
            var location = entity.GetComponent<Location>();

            Vector2 moved = movement.Velocity * deltaTime;
            location.Position += moved;

            movement.LastMoved = moved;
        }
    }
}