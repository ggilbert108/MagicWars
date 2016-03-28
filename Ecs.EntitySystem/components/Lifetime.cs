using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Lifetime : Component
    {
        public float TimeLeft;

        public Lifetime(float timeLeft)
        {
            TimeLeft = timeLeft;
        }
    }
}