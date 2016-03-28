using System.Drawing;

namespace Ecs.EntitySystem
{
    public class BlueEnemy : EnemyTemplate
    {
        public const float FREQUENCY = 1;

        public BlueEnemy(Rectangle bounds) : base(bounds)
        {
            Size = 20;
            Color = Color.Blue;
            Speed = 250;
            GrantedXp = 3;
        }
    }
}