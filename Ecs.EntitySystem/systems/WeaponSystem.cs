using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class WeaponSystem : Core.System
    {
        public WeaponSystem()
        {
            AddRequiredComponent<Weapon>();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            entity.GetComponent<Weapon>().CurrentTime += deltaTime;
        }
    }

}