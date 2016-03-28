namespace Ecs.EntitySystem
{
    public class BasicWeapon : WeaponTemplate
    {
        public BasicWeapon()
        {
            FireRate = .5f;
            Attack = new ProjectileAttack(300, 400);
            MinDamage = 10;
            MaxDamage = 15;
        }
    }
}