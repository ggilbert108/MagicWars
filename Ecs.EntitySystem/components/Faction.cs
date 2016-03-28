using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Faction : Component
    {
        public string Group;

        public Faction(string group)
        {
            Group = group;
        }
    }
}