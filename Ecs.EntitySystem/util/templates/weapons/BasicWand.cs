namespace Ecs.EntitySystem
{
    public class BasicWand : WeaponTemplate
    {
        public BasicWand()
        {
            FireRate = .2f;
            Attack = new ProjectileAttack(500, 800);
            MinDamage = 30;
            MaxDamage = 40;
        }
    }
}