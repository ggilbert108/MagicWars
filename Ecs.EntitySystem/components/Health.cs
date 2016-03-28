using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Health : Component
    {
        public int MaxHp;
        public int Hp;

        public Health(int maxHp)
        {
            MaxHp = maxHp;
            Hp = maxHp;
        }
    }
}