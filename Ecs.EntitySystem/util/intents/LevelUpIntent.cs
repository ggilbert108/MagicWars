using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class LevelUpIntent : IIntent
    {
        public void DoIntent(Entity entity, Manager manager)
        {
            var experience = entity.GetComponent<Experience>();
            var health = entity.GetComponent<Health>();

            experience.LevelUp();
            health.Hp = health.MaxHp;
        }
    }
}