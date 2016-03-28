using System.Collections.Generic;
using Bridge.Lib;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class QuadTree
    {
        private const int MAX_LEVELS = 10;
        private const int MAX_ENTITIES = 5;

        private int level;
        private Rectangle bounds;
        private QuadTree[] children;
        private List<Entity> entities;

        public QuadTree(int level, Rectangle bounds)
        {
            this.level = level;
            this.bounds = bounds;
            children = new QuadTree[4];

            for (int i = 0; i < 4; i++)
            {
                children[i] = null;
            }

            entities = new List<Entity>();
        }

        public void Insert(Entity entity)
        {
            var entityBounds = CollisionUtil.GetBounds(entity);
            if (level == 0 && !entityBounds.IntersectsWith(bounds))
            {
                return;
            }

            if (IsSplit)
            {
                int index = GetIndex(entityBounds);

                if (index != -1)
                {
                    children[index].Insert(entity);
                    return;
                }
            }

            entities.Add(entity);

            if (level < MAX_LEVELS && entities.Count > MAX_ENTITIES)
            {
                if (!IsSplit)
                {
                    Split();
                }

                List<Entity> newEntities = new List<Entity>();

                foreach (Entity other in entities)
                {
                    var otherBounds = CollisionUtil.GetBounds(other);
                    int index = GetIndex(otherBounds);

                    if (index != -1)
                    {
                        children[index].Insert(other);
                    }
                    else
                    {
                        newEntities.Add(other);
                    }
                }
                entities = newEntities;
            }
        }

        public int Total()
        {
            int total = entities.Count;

            if (IsSplit)
            {
                for (int i = 0; i < 4; i++)
                {
                    total += children[i].Total();
                }
            }
            return total;
        }

        public List<Entity> CouldCollide(Rectangle entityBounds)
        {
            List<Entity> could = new List<Entity>();
            CouldCollide(could, entityBounds);
            return could;
        }

        private void CouldCollide(List<Entity> could, Rectangle entityBounds)
        {
            if (!bounds.IntersectsWith(entityBounds))
                return;

            if (IsSplit)
            {
                int index = GetIndex(entityBounds);

                if (index != -1)
                {
                    children[index].CouldCollide(could, entityBounds);
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        children[i].CouldCollide(could, entityBounds);
                    }
                }
            }

            could.AddRange(entities);
        }

        private int GetIndex(Rectangle bounds)
        {
            Rectangle[] childBounds = GetChildBounds();

            for (int i = 0; i < 4; i++)
            {
                if (childBounds[i].Contains(bounds))
                {
                    return i;
                }
            }
            return -1;
        }

        private void Split()
        {
            Rectangle[] childBounds = GetChildBounds();

            for (int i = 0; i < 4; i++)
            {
                children[i] = new QuadTree(level + 1, childBounds[i]);
            }
        }

        private Rectangle[] GetChildBounds()
        {
            int x = bounds.X;
            int y = bounds.Y;
            int width = bounds.Width/2;
            int height = bounds.Height/2;
            int centerX = (bounds.Left + bounds.Right)/2;
            int centerY = (bounds.Top + bounds.Bottom)/2;

            return new Rectangle[]
            {
                new Rectangle(x, y, width, height),
                new Rectangle(centerX, y, width, height),
                new Rectangle(x, centerY, width, height),
                new Rectangle(centerX, centerY, width, height)
            };
        }

        private bool IsSplit
        {
            get { return children[0] != null; }

        }
    }
}