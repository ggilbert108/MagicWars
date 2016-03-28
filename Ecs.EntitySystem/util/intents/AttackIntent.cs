using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class AttackIntent : IIntent
    {
        private Vector2 target;

        public AttackIntent(Vector2 target = null)
        {
            this.target = (target == null) ? Vector2.Zero : target;
        }

        public void DoIntent(Entity entity, Manager manager)
        {
            var weapon = EntityUtil.GetWeapon(entity, manager);

            if (weapon.CurrentTime < weapon.FireRate) return;

            weapon.CurrentTime = 0;
            weapon.Attack.DoAttack(entity, target, manager);
        }
    }
}