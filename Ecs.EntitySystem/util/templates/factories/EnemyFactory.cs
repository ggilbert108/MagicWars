using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ecs.EntitySystem
{
    public static class EnemyFactory
    {
        public static EnemyTemplate GenerateEnemy(Rectangle bounds)
        {
            var subclasses = typeof (EnemyTemplate).Assembly.GetTypes().Where(
                type => type.IsSubclassOf(typeof (EnemyTemplate))).ToList();

            var byFrequency = MapByFrequency(subclasses);
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