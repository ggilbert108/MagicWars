using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public abstract class EnemyTemplate
    {
        public Rectangle Bounds;

        protected EnemyTemplate(Rectangle bounds)
        {
            this.Bounds = bounds;
        }

        protected int Size;
        protected Color Color;
        protected int Speed;
        protected int GrantedXp;
        protected int Health;
        public int FollowingId = -1;
        

        public int CreateEntity(Manager manager)
        {
            Vector2 location = VectorUtil.GetVectorInBounds(Bounds);
            Entity enemy = manager.AddAndGetEntity();
            manager.AddComponentToEntity(enemy, new Location(location));
            manager.AddComponentToEntity(enemy, new Shape(3, Size, Color));
            manager.AddComponentToEntity(enemy, new Rotation());
            manager.AddComponentToEntity(enemy, new Intent());
            manager.AddComponentToEntity(enemy, new Steering(enemy));
            manager.AddComponentToEntity(enemy, new Movement(Speed, 10));
            manager.AddComponentToEntity(enemy, new Faction("bad"));
            manager.AddComponentToEntity(enemy, new Weapon(new BasicWeapon()));
            manager.AddComponentToEntity(enemy, new Intelligence(FollowingId));
            manager.AddComponentToEntity(enemy, new Health(Health));
            manager.AddComponentToEntity(enemy, new Death(GrantedXp));

            return enemy.Id;
        }
    }
}