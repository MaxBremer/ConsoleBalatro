using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public sealed class JokerForcedRarityRule : IMarketPoolRule
    {
        public static readonly Dictionary<GenerationSource, JokerRarity> ForcedRaritySources = new()
        {
            [GenerationSource.RareTag] = JokerRarity.RARE,
            [GenerationSource.UncommonTag] = JokerRarity.UNCOMMON,
            [GenerationSource.SoulCard] = JokerRarity.LEGENDARY,
            [GenerationSource.WraithCard] = JokerRarity.RARE,
            [GenerationSource.TopUpTag] = JokerRarity.COMMON,
        };

        public int Priority => 10;

        public void ModifyCandidates(MarketPoolContext context)
        {
            if (context.Pool == ItemPool.Joker && ForcedRaritySources.ContainsKey(context.Source))
            {
                context.Candidates.RemoveAll(x => JokerDb.JokerMetadata[x.Definition.Id].Rarity != ForcedRaritySources[context.Source]);
            }
        }
    }
}
