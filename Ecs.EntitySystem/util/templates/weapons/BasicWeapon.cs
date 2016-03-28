namespace Ecs.EntitySystem
{
    public class BasicWeapon : WeaponTemplate
    {
        public BasicWeapon()
        {
            FireRate = 2f;
            Attack = new ProjectileAttack(500, 600);
            MinDamage = 10;
            MaxDamage = 15;
        }
    }
}