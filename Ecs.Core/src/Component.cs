using System.Collections.Generic;
using System.Linq;

namespace Ecs.Core
{
    public abstract class Component
    {
        private readonly List<string> dependencies;

        protected Component()
        {
            dependencies = new List<string>();
        }

        public virtual void OnCreate(Entity entity) { }

        protected void AddDependency<T>() where T : Component
        {
            dependencies.Add(typeof(T).GetClassName());
        }

        public bool CanBeAddedToEntity(Entity entity)
        {
            return true;
            //return dependencies.All(
            //    dependency => entity.HasComponent(dependency));
        }

        public bool HasDependency(string componentType)
        {
            return dependencies.Contains(componentType);
        }

        public string GetLackingDependency(Entity entity)
        {
            string lacking = dependencies.First(dependency => !entity.HasComponent(dependency));

            return "The component " + GetClassName() +
                " has a missing dependency: " + lacking;
        }
    }

}