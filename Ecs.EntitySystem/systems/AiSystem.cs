using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class AiSystem : Core.System
    {
        public AiSystem()
        {
            AddRequiredComponent<Intelligence>();
            AddRequiredComponent<Weapon>();
            AddRequiredComponent<Intent>();
            AddRequiredComponent<Movement>();
            AddRequiredComponent<Location>();
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            var intelligence = entity.GetComponent<Intelligence>();
            var weapon = EntityUtil.GetWeapon(entity, Manager);
            var intent = entity.GetComponent<Intent>();
            var velocity = entity.GetComponent<Movement>().LastMoved;
            var position = entity.GetComponent<Location>().Position;
            var heroPosition = Hero.GetComponent<Location>().Position;

            float distance = (position - heroPosition).Length;
            if (intelligence.FollowingId == -1)
            {
                if (distance < Intelligence.SIGHT_RANGE)
                {
                    intelligence.FollowingId = Hero.Id;
                }
            }
            else if (intelligence.FollowingId == Hero.Id)
            {
                if (distance > Intelligence.MAX_SIGHT_RANGE)
                {
                    intelligence.FollowingId = -1;
                }
            }

            if (distance < Intelligence.SIGHT_RANGE)
            {
                if (weapon.CurrentTime > weapon.FireRate)
                {
                    intent.Queue.Add(new AttackIntent(heroPosition));
                }
            }
        }

        public override void DeleteEntity(int id)
        {
            foreach (Entity entity in Entities)
            {
                var intelligence = entity.GetComponent<Intelligence>();
                if (intelligence.FollowingId == id)
                {
                    intelligence.FollowingId = -1;
                }
            }

            base.DeleteEntity(id);
        }
    }
}