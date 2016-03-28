namespace Ecs.EntitySystem
{
    public abstract class WeaponTemplate
    {
        public float FireRate;
        public IAttack Attack;
        public object AttackTemplate;
        public int MinDamage;
        public int MaxDamage;
        public int Range;
    }

}