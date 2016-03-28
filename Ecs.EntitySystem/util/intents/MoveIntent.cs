using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public class MoveIntent : IIntent
    {
        private Vector2 force;

        public MoveIntent(Vector2 force)
        {
            this.force = force;
        }

        public void DoIntent(Entity entity, Manager manager)
        {
            var movement = entity.GetComponent<Movement>();
            movement.Move(force);
        }
    }
}