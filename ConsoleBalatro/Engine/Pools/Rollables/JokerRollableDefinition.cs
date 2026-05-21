using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rollables
{
    public class JokerRollableDefinition : RollableDefinition
    {
        private JokerTypeData _myInnerDefinition;
        private Func<MarketEligibilityContext, bool> _myInnerEligFunc;
        public JokerRollableDefinition(JokerTypeData jokerMetadata)
        {
            _myInnerDefinition = jokerMetadata;
            Id = jokerMetadata.DBName;
            _myInnerEligFunc = jokerMetadata.isEligible;
            Rarity = jokerMetadata.Rarity;

        }
        public override string Id { get; init; }
        public override ItemPool Pool => ItemPool.Joker;
        public override JokerRarity Rarity { get; init; }
        public override bool IsEligible(MarketEligibilityContext context)
        {
            return _myInnerEligFunc(context);
        }
    }
}
