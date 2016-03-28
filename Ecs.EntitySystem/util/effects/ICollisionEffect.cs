using Ecs.Core;

namespace Ecs.EntitySystem
{
    public interface ICollisionEffect
    {
        bool AffectsEntity(Entity entity, Entity affected);
        void ResolveEffect(Entity entity, Entity affected, Manager manager);
    }
}