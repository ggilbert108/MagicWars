using System.Drawing;

namespace Ecs.EntitySystem
{
    public class OrangeEnemy : EnemyTemplate
    {
        public const float FREQUENCY = .5f;

        public OrangeEnemy(Rectangle bounds) : base(bounds)
        {
            Size = 20;
            Color = Color.Orange;
            Speed = 300;
            GrantedXp = 10;
            Health = 50;
        }
    }
}