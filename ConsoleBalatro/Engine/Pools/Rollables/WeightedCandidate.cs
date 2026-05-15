using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rollables
{
    public sealed class WeightedCandidate
    {
        public required RollableDefinition Definition { get; init; }
        public int Weight { get; set; }
    }
}
