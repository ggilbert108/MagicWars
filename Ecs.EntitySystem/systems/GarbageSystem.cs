using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class GarbageSystem : Core.System
    {
        public GarbageSystem()
        {
            AddRequiredComponent<Lifetime>();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            var lifetime = entity.GetComponent<Lifetime>();
            lifetime.TimeLeft -= deltaTime;

            if (lifetime.TimeLeft <= 0)
            {
                Manager.DeleteEntity(entity.Id);
            }
        }
    }
}