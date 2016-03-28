using Bridge.Lib;

namespace Ecs.EntitySystem
{
    public class PurpleBoss : BossTemplate
    {
        public const float FREQUENCY = 1f;

        public PurpleBoss(Rectangle bounds) : base(bounds)
        {
            Size = 100;
            Color = Color.Purple;
            Speed = 100;
            GrantedXp = 50;
            Health = 500;

            FollowerTemplate = new BlueEnemy(bounds);
            FollowerAmount = 20;
        }
    }
}