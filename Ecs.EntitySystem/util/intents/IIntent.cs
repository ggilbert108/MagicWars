using Ecs.Core;

namespace Ecs.EntitySystem
{
    public interface IIntent
    {
        void DoIntent(Entity entity, Manager manager);
    }
}