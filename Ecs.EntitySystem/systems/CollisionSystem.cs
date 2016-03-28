using System.Collections.Generic;
using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class CollisionSystem : Core.System
    {
        private QuadTree partition;
        private HashSet<string, string> checkedPairs;


        public CollisionSystem()
        {
            AddRequiredComponent<Shape>();
            AddRequiredComponent<Location>();
        }

        public override void UpdateAll(float deltaTime)
        {
            checkedPairs = new HashSet<string, string>();
            partition = new QuadTree(0, HeroViewport);
            foreach (Entity entity in Entities)
            {
                partition.Insert(entity);
            }
            base.UpdateAll(deltaTime);
        }

        protected override void Update(Entity entity, float deltaTime)
        {
            Rectangle entityBounds = CollisionUtil.GetBounds(entity);
            var couldCollide = partition.CouldCollide(entityBounds);

            foreach (Entity other in couldCollide)
            {
                if (entity.Id == other.Id) continue;

                string pair1 = GetPairString(entity, other);
                string pair2 = GetPairString(other, entity);

                if (checkedPairs.Contains(pair1) || checkedPairs.Contains(pair2))
                    continue;

                CheckCollsion(entity, other);
            }
        }

        public void CheckCollsion(Entity a, Entity b)
        {
            if (!CollisionUtil.CouldCollide(a, b))
                return;

            if (!CollisionUtil.Collide(a, b))
                return;

            checkedPairs.Add(GetPairString(a, b));

            if (a.HasComponent<CollisionEffect>())
            {
                var effects = a.GetComponent<CollisionEffect>();
                foreach (var effect in effects.Effects)
                {
                    if (effect.AffectsEntity(a, b))
                    {
                        effect.ResolveEffect(a, b, Manager);
                    }
                }
            }

            if (b.HasComponent<CollisionEffect>())
            {
                var effects = b.GetComponent<CollisionEffect>();
                foreach (var effect in effects.Effects)
                {
                    if (effect.AffectsEntity(b, a))
                    {
                        effect.ResolveEffect(b, a, Manager);
                    }
                }
            }

        }

        private string GetPairString(Entity a, Entity b)
        {
            return a.Id + "_" + b.Id;
        }

        private Rectangle HeroViewport
        {
            get { return Hero.GetComponent<Camera>().Viewport; }
        }
    }
}