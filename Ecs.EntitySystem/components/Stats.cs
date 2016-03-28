using Ecs.Core;

namespace Ecs.EntitySystem
{
    public class Stats : Component
    {
        #region Data

        public int BaseAtt;
        public int BaseDef;
        public int BaseSpd;
        public int BaseDex;
        public int BaseVit;
        public int BaseWis;

        public int ModAtt;
        public int ModDef;
        public int ModSpd;
        public int ModDex;
        public int ModVit;
        public int ModWis;
        
        #endregion

        public Stats(int att = 10, int def = 10, int spd = 10, int dex = 10, int vit = 10, int wis = 10)
        {
            BaseAtt = att;
            BaseDef = def;
            BaseSpd = spd;
            BaseDex = dex;
            BaseVit = vit;
            BaseWis = wis;
        }

        #region Stats

        public int Attack
        {
            get { return BaseAtt + ModAtt; }
        }

        public int Defense
        {
            get { return BaseDef + ModDef; }
        }

        public int Speed
        {
            get { return BaseSpd + ModSpd; }
        }

        public int Dexterity
        {
            get { return BaseDex + ModDex; }
        }

        public int Vitality
        {
            get { return BaseVit + ModVit; }
        }

        public int Wisdom
        {
            get { return BaseWis + ModWis; }
        }

        #endregion
    }
}