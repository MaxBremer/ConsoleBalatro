using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rollables
{
    public class TarotRollableDefinition : RollableDefinition
    {
        public TarotRollableDefinition(string id)
        {
            Id = id;
        }
        public override string Id { get; init; }
        public override ItemPool Pool => ItemPool.Tarot;
        public override JokerRarity Rarity { get; init; } = JokerRarity.COMMON;
    }
}
