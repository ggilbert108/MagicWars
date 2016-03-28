using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecs.Core
{
    public class Entity
    {
        public int Id { get; private set; }
        private Dictionary<string, Component> components;

        public Entity(int id)
        {
            Id = id;
            components = new Dictionary<string, Component>();
        }

        public void AddComponent(Component component)
        {
            components[component.GetClassName()] = component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            components.Remove(typeof(T).GetClassName());
        }

        public T GetComponent<T>() where T : Component
        {
            return (T) components[typeof(T).GetClassName()];
        }

        public bool HasComponent(string componentType)
        {
            return components.ContainsKey(componentType);
        }

        public bool HasComponent<T>() where T : Component
        {
            return components.ContainsKey(typeof(T).GetClassName());
        }

        public List<Component> GetDependentComponents(Component component)
        {
            var result = from other in components.Values
                         where other.HasDependency(component.GetClassName())
                         select other;

            return result.ToList();
        }
    }
}