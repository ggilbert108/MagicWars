using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Death : Component
    {
        public int GrantedXp;

        public Death(int grantedXp)
        {
            GrantedXp = grantedXp;
        }
    }
}