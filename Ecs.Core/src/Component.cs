using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecs.Core
{
    public abstract class Component
    {
        private readonly List<Type> dependencies;

        protected Component()
        {
            dependencies = new List<Type>();
        }

        public virtual void OnCreate(Entity entity) { }

        protected void AddDependency<T>() where T : Component
        {
            dependencies.Add(typeof(T));
        }

        public bool CanBeAddedToEntity(Entity entity)
        {
            return dependencies.All(
                dependency => entity.HasComponent(dependency));
        }

        public bool HasDependency(Type componentType)
        {
            return dependencies.Contains(componentType);
        }

        public string GetLackingDependency(Entity entity)
        {
            Type lacking = dependencies.First(
                dependency => !entity.HasComponent(dependency));

            return "The component " + GetType().Name +
                " has a missing dependency: " + lacking.Name;
        }
    }

}