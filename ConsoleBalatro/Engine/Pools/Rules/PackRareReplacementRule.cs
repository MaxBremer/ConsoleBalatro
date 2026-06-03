using ConsoleBalatro.Engine.Pools.Rollables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public class PackRareReplacementRule : IMarketPoolRule
    {
        public int Priority => 101;

        public void ModifyCandidates(MarketPoolContext context)
        {
            if(context != null && context.Source == GenerationSource.Pack && (context.Pool == ItemPool.Tarot || context.Pool == ItemPool.Planet || context.Pool == ItemPool.Spectral))
            {
                if(context.Pool == ItemPool.Tarot && !ZoneManager.ConsumableZone.Cards.Any(c => c.isConsumable && c.ConsumableData.DBName == "SOUL") && !ZoneManager.PackOptionZone.Cards.Any(c => c.isConsumable && c.ConsumableData.DBName == "SOUL") && !ContextContainsCandidate(context, "SOUL") && Globals.randomNext(0, 1000) <= 3)
                {
                    context.Candidates.Clear();
                    context.Candidates.Add(new WeightedCandidate { Weight = 1, Definition = new SpectralRollableDefinition("SOUL") });
                }else if (context.Pool == ItemPool.Planet && !ZoneManager.ConsumableZone.Cards.Any(c => c.isConsumable && c.ConsumableData.DBName == "BLACK HOLE") && !ZoneManager.PackOptionZone.Cards.Any(c => c.isConsumable && c.ConsumableData.DBName == "BLACK HOLE") && !ContextContainsCandidate(context, "BLACK HOLE") && Globals.randomNext(0, 1000) <= 3)
                {
                    context.Candidates.Clear();
                    context.Candidates.Add(new WeightedCandidate { Weight = 1, Definition = new SpectralRollableDefinition("BLACK HOLE") });
                }else if(context.Pool == ItemPool.Spectral)
                {
                    if (!ZoneManager.ConsumableZone.Cards.Any(c => c.isConsumable && c.ConsumableData.DBName == "SOUL") && !ZoneManager.PackOptionZone.Cards.Any(c => c.isConsumable && c.ConsumableData.DBName == "SOUL") && !ContextContainsCandidate(context, "SOUL") && Globals.randomNext(0, 1000) <= 3)
                    {
                        context.Candidates.Clear();
                        context.Candidates.Add(new WeightedCandidate { Weight = 1, Definition = new SpectralRollableDefinition("SOUL") });
                    }
                    if (!ZoneManager.ConsumableZone.Cards.Any(c => c.isConsumable && c.ConsumableData.DBName == "BLACK HOLE") && !ZoneManager.PackOptionZone.Cards.Any(c => c.isConsumable && c.ConsumableData.DBName == "BLACK HOLE") && !ContextContainsCandidate(context, "BLACK HOLE") && Globals.randomNext(0, 1000) <= 3)
                    {
                        context.Candidates.Clear();
                        context.Candidates.Add(new WeightedCandidate { Weight = 1, Definition = new SpectralRollableDefinition("BLACK HOLE") });
                    }
                }
            }
        }

        private bool ContextContainsCandidate(MarketPoolContext context, string id)
        {
            return context.BatchContext != null && !context.BatchContext.AllowDuplicateResultsInSameBatch && context.BatchContext.GeneratedIds.Contains(id);
        }
    }
}
