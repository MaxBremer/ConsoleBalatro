using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools
{
    public sealed class ContentRollBatchContext
    {
        public HashSet<string> GeneratedIds { get; } = new();

        public bool AllowDuplicateResultsInSameBatch { get; set; } = false;
    }
}
