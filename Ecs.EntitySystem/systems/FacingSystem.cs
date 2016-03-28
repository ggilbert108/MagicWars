using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public class FacingSystem : Core.System
    {
        public FacingSystem()
        {
            AddRequiredComponent<Rotation>();
            AddRequiredComponent<Movement>();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            var rotation = entity.GetComponent<Rotation>();
            var velocity = entity.GetComponent<Movement>().Velocity;

            Vector2 up = new Vector2(0, -1);
            float angle = VectorUtil.AngleBetween(up, velocity);
            rotation.Angle = angle;
        }
    }
}