﻿using System.Drawing;
using Ecs.Core;
using OpenTK;

namespace Ecs.EntitySystem
{
    public class Generator
    {
        private Manager manager;
        private int heroId;
        private Rectangle bounds;

        public Generator(Manager manager)
        {
            this.manager = manager;
        }

        public void SetupHero()
        {
            
        }

        public void SetupBounds(Rectangle bounds)
        {
            this.bounds = bounds;
        }

        public void GenerateLevel()
        {
            GenerateHero();
            GenerateObstacles();
            GenerateEnemies();
        }

        private void GenerateHero()
        {
            Entity hero = manager.AddAndGetEntity();
            manager.AddComponentToEntity(hero, new Location(new Vector2(100, 100)));
            manager.AddComponentToEntity(hero, new Shape(0, 30, Color.Red));
            manager.AddComponentToEntity(hero, new Intent());
            manager.AddComponentToEntity(hero, new Movement(200));
            manager.AddComponentToEntity(hero, new Camera(800, 600));
            manager.AddComponentToEntity(hero, new Weapon(new BasicWand()));
            manager.AddComponentToEntity(hero, new Faction("good"));
            manager.AddComponentToEntity(hero, new Health(300));
            manager.AddComponentToEntity(hero, new Stats());
            manager.AddComponentToEntity(hero, new Experience());

            heroId = hero.Id;
        }

        private void GenerateObstacles()
        {
            Entity obstacle = manager.AddAndGetEntity();
            manager.AddComponentToEntity(obstacle, new Location(new Vector2(300, 300)));
            manager.AddComponentToEntity(obstacle, new Shape(5, 50, Color.Gray));
            manager.AddComponentToEntity(obstacle, new CollisionEffect(new BlockEffect()));
            manager.AddComponentToEntity(obstacle, new Rotation());
        }

        private void GenerateEnemies()
        {
            for (int i = 0; i < 100; i++)
            {
                EnemyTemplate template = EnemyFactory.GenerateEnemy(bounds);
                template.CreateEntity(manager, heroId);
            }
        }

        public int GetHeroId()
        {
            return heroId;
        }
    }
}