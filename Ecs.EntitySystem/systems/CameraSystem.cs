using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class CameraSystem : Core.System
    {
        public CameraSystem()
        {
            AddRequiredComponent<Camera>();
            AddRequiredComponent<Location>();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            var camera = entity.GetComponent<Camera>();
            var position = entity.GetComponent<Location>().Position;
            camera.SetPosition(position);
        }
    }
}