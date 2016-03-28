using System.Drawing;

namespace Ecs.EntitySystem
{
    public class GreenEnemy : EnemyTemplate
    {
        public const float FREQUENCY = 1;

        public GreenEnemy(Rectangle bounds) : base(bounds)
        {
            Size = 20;
            Color = Color.Green;
            Speed = 300;
            GrantedXp = 5;
        }
    }
}