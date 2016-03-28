using System;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class DestroyedEffect : ICollisionEffect
    {
        private int ignoreId;

        public DestroyedEffect(int ignoreId)
        {
            this.ignoreId = ignoreId;
        }

        public bool AffectsEntity(Entity entity, Entity affected)
        {
            if (affected.HasComponent<CollisionEffect>())
            {
                var effect = affected.GetComponent<CollisionEffect>();
                if (effect.ContainsEffect<DestroyedEffect>())
                {
                    return false;
                }
            }

            return affected.Id != ignoreId;
        }

        public void ResolveEffect(Entity entity, Entity affected, Manager manager)
        {
            manager.DeleteEntity(entity.Id);
        }
    }
}