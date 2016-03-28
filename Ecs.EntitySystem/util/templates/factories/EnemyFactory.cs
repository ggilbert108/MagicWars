using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ecs.EntitySystem
{
    public static class EnemyFactory
    {
        public static EnemyTemplate GenerateLoneEnemy(Rectangle bounds)
        {
            var allTypes = typeof (EnemyTemplate).Assembly.GetTypes();

            var subclasses = 
                from type in allTypes
                where type.IsSubclassOf(typeof (EnemyTemplate))
                where type != typeof(BossTemplate)
                where !type.IsSubclassOf(typeof (BossTemplate))
                select type;

            var templateTypes = subclasses.ToList();

            var byFrequency = MapByFrequency(templateTypes);
            float random = (float) Util.Rng.NextDouble();

            Type templateType = byFrequency[byFrequency.Keys.First()];
            foreach (float frequency in byFrequency.Keys)
            {
                templateType = byFrequency[frequency];
                if (random < frequency)
                    break;
            }

            EnemyTemplate template = (EnemyTemplate) Activator.CreateInstance(templateType, bounds);
            return template;
        }

        public static BossTemplate GenerateBoss(Rectangle bounds)
        {
            var allTypes = typeof(EnemyTemplate).Assembly.GetTypes();

            var subclasses =
                from type in allTypes
                where type.IsSubclassOf(typeof(BossTemplate))
                select type;

            var templateTypes = subclasses.ToList();

            var byFrequency = MapByFrequency(templateTypes);
            float random = (float)Util.Rng.NextDouble();

            Type templateType = byFrequency[byFrequency.Keys.First()];
            foreach (float frequency in byFrequency.Keys)
            {
                templateType = byFrequency[frequency];
                if (random < frequency)
                    break;
            }

            BossTemplate template = (BossTemplate)Activator.CreateInstance(templateType, bounds);
            return template;
        }

        private static SortedDictionary<float, Type> MapByFrequency(List<Type> templateTypes)
        {
            var byFrequency = new SortedDictionary<float, Type>();

            float total = 0f; //for normalization
            foreach (Type templateType in templateTypes)
            {
                float frequency = (float) templateType.GetField("FREQUENCY").GetRawConstantValue();
                while (byFrequency.ContainsKey(frequency))
                {
                    frequency += 0.1f;
                }

                byFrequency[frequency] = templateType;
                total += frequency;
            }
            
            var normalized = new SortedDictionary<float, Type>();
            foreach (float frequency in byFrequency.Keys)
            {
                normalized[frequency/total] = byFrequency[frequency];
            }

            return normalized;
        }
    }
}