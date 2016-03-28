using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Weapon : Component
    {
        public IAttack Attack;
        public double FireRate;
        public double CurrentTime;

        private int minDamage;
        private int maxDamage;

        public Weapon(WeaponTemplate template)
        {
            Attack = template.Attack;
            FireRate = template.FireRate;
            minDamage = template.MinDamage;
            maxDamage = template.MaxDamage;

            CurrentTime = Util.Rng.NextDouble() * FireRate;
        }

        public int GetDamage()
        {
            return Util.Rng.Next(minDamage, maxDamage);
        }
    }
}