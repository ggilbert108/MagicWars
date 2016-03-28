using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class StatsSystem : Core.System
    {
        public StatsSystem()
        {
            AddRequiredComponent<Stats>();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            var stats = entity.GetComponent<Stats>();

            //Set movement speed
            const float tileSize = 50;
            const float baseSpeed = 4 * tileSize;
            const float speedModifier = 0.07467f * tileSize;

            float speed = baseSpeed + stats.Speed*speedModifier;
            entity.GetComponent<Movement>().MaxSpeed = speed;

            //Set fire rate
            const float baseAttackSpeed = 1.5f;
            const float attackSpeedModifier = 0.0867f;

            float attackSpeed = baseAttackSpeed + stats.Dexterity*attackSpeedModifier;
            EntityUtil.GetWeapon(entity, Manager).FireRate = 1/attackSpeed;
        }
    }
}