using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecs.Core
{
    public class Entity
    {
        public int Id { get; private set; }
        private Dictionary<Type, Component> components;

        public Entity(int id)
        {
            Id = id;
            components = new Dictionary<Type, Component>();
        }

        public void AddComponent(Component component)
        {
            components[component.GetType()] = component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            components.Remove(typeof (T));
        }

        public T GetComponent<T>() where T : Component
        {
            return (T) components[typeof (T)];
        }

        public bool HasComponent(Type componentType)
        {
            return components.ContainsKey(componentType);
        }

        public bool HasComponent<T>() where T : Component
        {
            return components.ContainsKey(typeof(T));
        }

        public List<Component> GetDependentComponents(Component component)
        {
            var result = from other in components.Values
                         where other.HasDependency(component.GetType())
                         select other;

            return result.ToList();
        }
    }
}