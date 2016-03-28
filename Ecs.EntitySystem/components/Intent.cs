using System.Collections.Generic;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Intent : Component
    {
        public List<IIntent> Queue;

        public Intent()
        {
            Queue = new List<IIntent>();
        }
    }
}