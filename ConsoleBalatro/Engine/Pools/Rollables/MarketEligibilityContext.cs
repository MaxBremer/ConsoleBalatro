using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rollables
{
    public sealed class MarketEligibilityContext
    {
        public required ItemPool Pool { get; init; }
        public required GenerationSource Source { get; init; }
    }
}
