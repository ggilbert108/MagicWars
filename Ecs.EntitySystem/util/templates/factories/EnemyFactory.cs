using System;
using System.Collections.Generic;
using System.Linq;
using Bridge;
using Bridge.Lib;

namespace Ecs.EntitySystem
{
    public static class EnemyFactory
    {
        public static EnemyTemplate GenerateLoneEnemy(Rectangle bounds)
        {
            List<EnemyTemplate> enemies = new List<EnemyTemplate>();
            List<float> frequencies = new List<float>();

            GetSmallEnemies(bounds, enemies, frequencies);

            Normalize(frequencies);

            double random = Util.Rng.NextDouble();

            EnemyTemplate template = enemies[0];
            for (int i = 0; i < frequencies.Count; i++)
            {
                if (random > frequencies[i])
                {
                    template = enemies[i];
                    break;
                }
            }

            return template;
        }

        public static BossTemplate GenerateBoss(Rectangle bounds)
        {
            List<BossTemplate> bosses = new List<BossTemplate>();
            List<float> frequencies = new List<float>();

            GetBosses(bounds, bosses, frequencies);

            Normalize(frequencies);

            double random = Util.Rng.NextDouble();

            BossTemplate template = bosses[0];
            for (int i = 0; i < frequencies.Count; i++)
            {
                if (random > frequencies[i])
                {
                    template = bosses[i];
                    break;
                }
            }

            return template;
        }

        private static void GetSmallEnemies(Rectangle bounds, List<EnemyTemplate> enemies, List<float> frequencies)
        {
            var enemies2 = new List<EnemyTemplate>()
            {
                new BlueEnemy(bounds), new GreenEnemy(bounds), new OrangeEnemy(bounds)
            };

            var frequencies2 = new List<float>()
            {
                BlueEnemy.FREQUENCY, GreenEnemy.FREQUENCY, OrangeEnemy.FREQUENCY
            };

            enemies.AddRange(enemies2);
            frequencies.AddRange(frequencies2);
        }

        private static void GetBosses(Rectangle bounds, List<BossTemplate> bosses, List<float> frequencies)
        {
            var bosses2 = new List<BossTemplate>()
            {
                new PurpleBoss(bounds)
            };

            var frequencies2 = new List<float>()
            {
                PurpleBoss.FREQUENCY
            };

            bosses.AddRange(bosses2);
            frequencies.AddRange(frequencies2);
        }

        private static void Normalize(List<float> frequencies)
        {
            float total = 0;

            foreach (float f in frequencies)
            {
                total += f;
            }

            for(int i = 0; i < frequencies.Count; i++)
            {
                frequencies[i] /= total;
            }
        }
    }
}