using ConsoleBalatro.Engine.Pools.Rollables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public sealed class MarketPoolContext
    {
        public required ItemPool Pool { get; init; }
        public required GenerationSource Source { get; init; }

        public List<WeightedCandidate> Candidates { get; } = new();

        public ContentRollBatchContext? BatchContext { get; set; }

        public bool AllowOwnedDuplicates { get; set; } = false;
        public bool AllowDuplicateResultsInSameBatch { get; set; } = false;
    }
}
