using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools
{
    public interface IRarityWeightRule
    {
        int Priority { get; }

        void ModifyWeights(
            RaritySelectionContext context,
            Dictionary<JokerRarity, int> weights);
    }
}
