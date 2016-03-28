using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Owner : Component
    {
        public int OwnerId;

        public Owner(int ownerId)
        {
            OwnerId = ownerId;
        }
    }
}