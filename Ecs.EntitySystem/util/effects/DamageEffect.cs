using System;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class DamageEffect : ICollisionEffect
    {
        private int damage;

        public DamageEffect(int damage)
        {
            this.damage = damage;
        }

        public bool AffectsEntity(Entity entity, Entity affected)
        {
            if (!affected.HasComponent<Faction>() ||
                !affected.HasComponent<Health>())
                return false;

            var entityFaction = entity.GetComponent<Faction>().Group;
            var affectedFaction = affected.GetComponent<Faction>().Group;

            return entityFaction != affectedFaction;
        }

        public void ResolveEffect(Entity entity, Entity affected, Manager manager)
        {
            var health = affected.GetComponent<Health>();

            health.Hp -= damage;
            if (health.Hp <= 0)
            {
                var intent = affected.GetComponent<Intent>();

                var ownerId = entity.GetComponent<Owner>().OwnerId;
                Entity killer = manager.GetEntityById(ownerId);
                intent.Queue.Add(new DeathIntent(killer));
            }
        }
    }
}