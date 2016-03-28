using System.Drawing;
using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public class ProjectileAttack : IAttack
    {
        private int speed;
        private float lifetime;
        public ProjectileAttack(int speed, int range)
        {
            this.speed = speed;
            lifetime = 1f*range/speed;
        }

        public void DoAttack(Entity attacker, Vector2 target, Manager manager)
        {
            var weapon = EntityUtil.GetWeapon(attacker, manager);

            var attackerPosition = attacker.GetComponent<Location>().Position;
            var towardsTarget = target - attackerPosition;
            towardsTarget.Normalize();
            towardsTarget *= speed;

            Entity projectile = manager.AddAndGetEntity();
            manager.AddComponentToEntity(projectile, new Location(attackerPosition));
            manager.AddComponentToEntity(projectile, new Movement(speed, -1, towardsTarget));
            manager.AddComponentToEntity(projectile, new Shape(0, 5, Color.Red));
            manager.AddComponentToEntity(projectile, new CollisionEffect(
                new DamageEffect(weapon.GetDamage()),
                new DestroyedEffect(attacker.Id)));
            manager.AddComponentToEntity(projectile, new Faction(attacker.GetComponent<Faction>().Group));
            manager.AddComponentToEntity(projectile, new Lifetime(lifetime));
            manager.AddComponentToEntity(projectile, new Owner(attacker.Id));
        }
    }
}