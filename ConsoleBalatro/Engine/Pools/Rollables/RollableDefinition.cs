using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rollables
{
    public abstract class RollableDefinition
    {
        //THIS IS THE DB NAME!!!
        public abstract string Id { get; init; }
        public abstract ItemPool Pool { get; }
        public abstract JokerRarity Rarity { get; init; }

        public virtual int BaseWeight => 1;

        public virtual bool IsEligible(MarketEligibilityContext context)
        {
            return true;
        }
    }
}
