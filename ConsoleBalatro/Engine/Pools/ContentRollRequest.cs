using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools
{
    public class ContentRollRequest
    {
        public required GameState Game { get; init; }
        public required ItemPool Pool { get; init; }
        public required GenerationSource Source { get; init; }

        public JokerRarity? ForcedRarity { get; init; }

        public ContentRollBatchContext? Batch { get; init; }
    }
}
