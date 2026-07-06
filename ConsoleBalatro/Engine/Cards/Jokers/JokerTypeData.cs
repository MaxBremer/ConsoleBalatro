using ConsoleBalatro.Engine.Pools;
using ConsoleBalatro.Engine.Pools.Rollables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Jokers
{
    public class JokerTypeData
    {
        public string DBName;
        public int Price;
        public JokerRarity Rarity = JokerRarity.COMMON;
        public Func<MarketEligibilityContext, bool> isEligible = _ => true;
        public string? AchievementForUnlock = null;
    }
}
