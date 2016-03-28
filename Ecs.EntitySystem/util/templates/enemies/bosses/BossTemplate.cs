using System;
using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public abstract class BossTemplate : EnemyTemplate
    {
        protected BossTemplate(Rectangle bounds) : base(bounds)
        {

        }

        protected EnemyTemplate FollowerTemplate;
        protected int FollowerAmount;

        public int CreateBoss(Manager manager)
        {
            int bossId = CreateEntity(manager);
            Vector2 bossPosition = manager.GetEntityById(bossId).GetComponent<Location>().Position;

            const int radius = 200;
            const int diameter = 2*radius;
            Rectangle followerBounds = new Rectangle
                ((int) (bossPosition.X - radius),
                (int) (bossPosition.Y - radius),
                diameter, diameter);


            for (int i = 0; i < FollowerAmount; i++)
            {
                FollowerTemplate.Bounds = followerBounds;
                FollowerTemplate.FollowingId = bossId;
                FollowerTemplate.CreateEntity(manager);
            }
            

            return bossId;
        }
    }
}