using System.Collections.Generic;
using System.Linq;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class CollisionEffect : Component
    {
        public readonly List<ICollisionEffect> Effects;

        public CollisionEffect(params ICollisionEffect[] effects)
        {
            Effects = effects.ToList();
        }

        public bool ContainsEffect<T>() where T : ICollisionEffect
        {
            foreach (var effect in Effects)
            {
                if (effect is T)
                    return true;
            }
            return false;
        }
    }

}