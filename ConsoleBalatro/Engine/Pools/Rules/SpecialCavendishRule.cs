using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public class SpecialCavendishRule : IMarketPoolRule
    {
        public int Priority => 1;

        public void ModifyCandidates(MarketPoolContext context)
        {
            if(context != null && context.Pool == ItemPool.Joker && context.Candidates.Any(c => c.Definition.Id == "CAVENDISH") && !Globals.Flags.Contains("GROS_MICHEL_SELF_DESTROY"))
            {
                context.Candidates.RemoveAll(c => c.Definition.Id == "CAVENDISH");
            }
        }
    }
}
