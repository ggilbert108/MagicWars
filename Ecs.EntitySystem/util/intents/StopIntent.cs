using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class StopIntent : IIntent
    {
        public void DoIntent(Entity entity, Manager manager)
        {
            entity.GetComponent<Movement>().Stop();
        }
    }
}