using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Intelligence : Component
    {
        public const int SIGHT_RANGE = 500; //All entities will have the same sight range
        public const int MAX_SIGHT_RANGE = 1000;

        public int FollowingId;

        public Intelligence(int followingId = -1)
        {
            FollowingId = followingId;
        }
    }
}