using System.Drawing;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public abstract class EnemyTemplate
    {
        private Rectangle bounds;

        protected EnemyTemplate(Rectangle bounds)
        {
            this.bounds = bounds;
        }

        protected int Size;
        protected Color Color;
        protected int Speed;
        protected int GrantedXp;

        public void CreateEntity(Manager manager, int heroId)
        {
            Entity enemy = manager.AddAndGetEntity();
            manager.AddComponentToEntity(enemy, new Location(VectorUtil.GetVectorInBounds(bounds)));
            manager.AddComponentToEntity(enemy, new Shape(3, Size, Color));
            manager.AddComponentToEntity(enemy, new Rotation());
            manager.AddComponentToEntity(enemy, new Intent());
            manager.AddComponentToEntity(enemy, new Steering(enemy));
            manager.AddComponentToEntity(enemy, new Movement(Speed, 10));
            manager.AddComponentToEntity(enemy, new Faction("bad"));
            manager.AddComponentToEntity(enemy, new Weapon(new BasicWeapon()));
            manager.AddComponentToEntity(enemy, new Intelligence());
            manager.AddComponentToEntity(enemy, new Health(50));
            manager.AddComponentToEntity(enemy, new Death(GrantedXp));
        }
    }
}