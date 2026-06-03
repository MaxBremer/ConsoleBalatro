using ConsoleBalatro.Engine.Pools.Rollables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public class SpectralNoSoulBlackHoleRule : IMarketPoolRule
    {
        public int Priority => 80;

        public void ModifyCandidates(MarketPoolContext context)
        {
            //Generally speaking, ALL sources should remove the specials from their pools. Specials can only generate via the 0.3% chance from PackRareReplacementRule.
            if (context != null && context.Pool == ItemPool.Spectral)
            {
                context.Candidates.RemoveAll(c => c.Definition is SpectralRollableDefinition def && (def.Id == "SOUL" || def.Id == "BLACK HOLE"));
            }
        }
    }
}
