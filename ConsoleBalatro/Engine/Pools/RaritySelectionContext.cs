using ConsoleBalatro.Engine.Pools.Rollables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools
{
    public sealed class RaritySelectionContext
    {
        public required ItemPool Pool { get; init; }
        public required GenerationSource Source { get; init; }

        public required IReadOnlyList<WeightedCandidate> Candidates { get; init; }
    }
}
