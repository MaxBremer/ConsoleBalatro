using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rollables
{
    public class SpecialPlanetRollableDefinition : RollableDefinition
    {
        private PlayedHandType _myHandType;
        public SpecialPlanetRollableDefinition(string id, PlayedHandType handType)
        {
            Id = id;
            _myHandType = handType;
        }
        public override string Id { get; init; }
        public override ItemPool Pool => ItemPool.Planet;
        public override JokerRarity Rarity { get; init; } = JokerRarity.COMMON;
        public override bool IsEligible(MarketEligibilityContext context)
        {
            return ScoreHandler.HandNumTimesPlayed[_myHandType] > 0;
        }
    }
}
