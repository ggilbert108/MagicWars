using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public interface IAttack
    {
        void DoAttack(Entity attacker, Vector2 target, Manager manager);
    }
}