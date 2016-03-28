using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public interface IAttack
    {
        void DoAttack(Entity attacker, Vector2 target, Manager manager);
    }
}