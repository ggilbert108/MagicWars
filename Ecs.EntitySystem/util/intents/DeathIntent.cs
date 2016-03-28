using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class DeathIntent : IIntent
    {
        private Entity killer;

        public DeathIntent(Entity killer)
        {
            this.killer = killer;
        }

        public void DoIntent(Entity entity, Manager manager)
        {
            if (entity.HasComponent<Death>())
            {
                var death = entity.GetComponent<Death>();

                if (killer.HasComponent<Experience>())
                {
                    var experience = killer.GetComponent<Experience>();
                    experience.Xp += death.GrantedXp;

                    if (experience.Xp >= experience.ToNextLevel)
                    {
                        var intent = killer.GetComponent<Intent>();
                        intent.Queue.Add(new LevelUpIntent());
                    }
                }
            }
            manager.DeleteEntity(entity.Id);
        }
    }
}