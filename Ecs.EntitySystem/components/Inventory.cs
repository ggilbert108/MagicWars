using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Inventory : Component
    {
        public int WeaponId;

        public Inventory()
        {
            WeaponId = -1;
        }
    }
}