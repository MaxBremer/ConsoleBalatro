using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools
{
    public static class RaritySelector
    {
        public static List<IRarityWeightRule> RarityWeightRules = new List<IRarityWeightRule>();

        public static JokerRarity SelectRarity(RaritySelectionContext context)
        {
            var weights = GetBaseWeights(context);

            RemoveUnavailableRarities(context, weights);

            foreach (var rule in RarityWeightRules.OrderBy(r => r.Priority))
            {
                rule.ModifyWeights(context, weights);
            }

            RemoveUnavailableRarities(context, weights);

            return PickWeighted(weights);
        }

        private static Dictionary<JokerRarity, int> GetBaseWeights(RaritySelectionContext context)
        {
            if(context.Pool == ItemPool.Joker)
            {
                return new Dictionary<JokerRarity, int>
                {
                    { JokerRarity.COMMON, 70 },
                    { JokerRarity.UNCOMMON, 25 },
                    { JokerRarity.RARE, 5 },
                };
            }

            return new Dictionary<JokerRarity, int>
            {
                [JokerRarity.COMMON] = 100,
            };
        }

        private static void RemoveUnavailableRarities(RaritySelectionContext context, Dictionary<JokerRarity, int> weights)
        {
            foreach (var rarity in weights.Keys.ToList())
            {
                bool hasCandidate = context.Candidates.Any(c =>
                    c.Definition.Rarity == rarity);

                if (!hasCandidate)
                {
                    weights[rarity] = 0;
                }
            }
        }

        private static JokerRarity PickWeighted(Dictionary<JokerRarity, int> weights)
        {
            int totalWeight = weights.Values.Sum();

            if (totalWeight <= 0)
                throw new InvalidOperationException("No available rarity to roll.");

            // Assume injected random in real version.
            var random = new Random();
            int roll = random.Next(1, totalWeight + 1);

            int running = 0;

            foreach (var pair in weights)
            {
                running += pair.Value;

                if (roll <= running)
                    return pair.Key;
            }

            return weights.Keys.Last();
        }
    }
}
