using System;
using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public class BlockEffect : ICollisionEffect
    {
        public bool AffectsEntity(Entity entity, Entity affected)
        {
            return affected.HasComponent<Movement>();
        }

        public void ResolveEffect(Entity entity, Entity affected, Manager manager)
        {
            Vector2 normal = CollisionUtil.GetNormal(affected, entity);
            HandleBlockingCollision(affected, entity, normal);
        }

        private void HandleBlockingCollision(Entity mover, Entity blocker, Vector2 normal)
        {
            normal.Normalize();
            Vector2 correction = AdjustCorrectionVector(mover, blocker, normal);
            mover.GetComponent<Location>().Position += correction;
            EntityUtil.AdjustCamera(mover);
        }

        private Vector2 AdjustCorrectionVector(Entity mover, Entity blocker, Vector2 correction)
        {
            float min = 0;
            float max = mover.GetComponent<Movement>().LastMoved.Length * 2;

            const double minResolution = 1;
            while ((max - min) > minResolution) //binary search
            {
                float mid = (max + min) / 2;
                if (CollideAfterCorrection(mover, blocker, correction * mid))
                {
                    min = mid;
                }
                else
                {
                    max = mid;
                }
            }
            return correction * max;
        }

        private bool CollideAfterCorrection(Entity mover, Entity blocker, Vector2 correction)
        {
            var location = mover.GetComponent<Location>();
            location.Position += correction;

            bool result = CollisionUtil.Collide(mover, blocker);

            location.Position -= correction;
            return result;
        }
    }
}