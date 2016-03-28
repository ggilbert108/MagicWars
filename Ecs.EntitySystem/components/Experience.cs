using System;
using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Experience : Component
    {
        private const float XP_GROWTH_BASE = 1.5f;
        private const int INITIAL_LEVEL_XP = 100;

        public int Level;
        public int Xp;
        public int ToNextLevel;

        public Experience()
        {
            Xp = 0;
            ToNextLevel = INITIAL_LEVEL_XP;
            Level = 1;
        }

        public void LevelUp()
        {
            Level++;
            ToNextLevel = (int) (INITIAL_LEVEL_XP + Math.Pow(XP_GROWTH_BASE, Level));
            Xp = 0;
        }
    }
}