using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public interface IMarketPoolRule
    {
        int Priority { get; }

        void ModifyCandidates(MarketPoolContext context);
    }
}
