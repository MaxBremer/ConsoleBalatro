using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public sealed class JokerNoLegosRule : IMarketPoolRule
    {
        public static readonly List<GenerationSource> LegendariesAllowed = new List<GenerationSource>()
        { 
            GenerationSource.SoulCard
        };
        public int Priority => 10;

        public void ModifyCandidates(MarketPoolContext context)
        {
            if (context.Pool == ItemPool.Joker && (!LegendariesAllowed.Contains(context.Source)))
            {
                context.Candidates.RemoveAll(x => JokerDb.JokerMetadata[x.Definition.Id].Rarity == JokerRarity.LEGENDARY);
            }
        }
    }
}
