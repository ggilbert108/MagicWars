using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Location : Component
    {
        public Vector2 Position;

        public Location(Vector2 position)
        {
            Position = position;
        }
    }
}