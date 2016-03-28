using Bridge.Lib;

namespace Ecs.EntitySystem
{
    public class GreenEnemy : EnemyTemplate
    {
        public const float FREQUENCY = .3f;

        public GreenEnemy(Rectangle bounds) : base(bounds)
        {
            Size = 40;
            Color = Color.Green;
            Speed = 300;
            GrantedXp = 10;
            Health = 100;
        }
    }
}