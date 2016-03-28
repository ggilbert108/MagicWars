using System;
using System.Drawing;
using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public abstract class BossTemplate : EnemyTemplate
    {
        protected BossTemplate(Rectangle bounds) : base(bounds)
        {

        }

        protected string FollowerClassName;
        protected int FollowerAmount;

        public int CreateBoss(Manager manager)
        {
            int bossId = CreateEntity(manager);
            Vector2 bossPosition = manager.GetEntityById(bossId).GetComponent<Location>().Position;

            Type followerType = Type.GetType("Ecs.EntitySystem." + FollowerClassName);
            if (followerType != null)
            {
                const int radius = 200;
                const int diameter = 2*radius;
                Rectangle followerBounds = new Rectangle
                    ((int) (bossPosition.X - radius),
                    (int) (bossPosition.Y - radius),
                    diameter, diameter);

                EnemyTemplate template = (EnemyTemplate) 
                    Activator.CreateInstance(followerType, followerBounds);

                for (int i = 0; i < FollowerAmount; i++)
                {
                    template.FollowingId = bossId;
                    template.CreateEntity(manager);
                }
            }

            return bossId;
        }
    }
}