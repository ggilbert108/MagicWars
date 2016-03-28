using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class IntentSystem : Core.System
    {
        public IntentSystem()
        {
            AddRequiredComponent<Intent>();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            var intent = entity.GetComponent<Intent>();

            foreach (IIntent queued in intent.Queue)
            {
                queued.DoIntent(entity, Manager);
            }

            intent.Queue.Clear();
        }
    }

}