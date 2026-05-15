using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public sealed class NoOwnedDuplicatesRule : IMarketPoolRule
    {
        public int Priority => 100;

        public void ModifyCandidates(MarketPoolContext context)
        {
            if (context.AllowOwnedDuplicates)
                return;
            //FOR NOW, ONLY JOKER ZONE AND CONSUMABLE ZONE CHECK FOR DUPLICATES.
            var ownedIds = ZoneManager.JokerZone.Cards
                .Select(j => j.JokerData.DBName)
                .ToHashSet();

            foreach (var card in ZoneManager.ConsumableZone.Cards.Where(x => x.isConsumable))
            {
                ownedIds.Add(card.ConsumableData.DBName);
            }

            context.Candidates.RemoveAll(c => ownedIds.Contains(c.Definition.Id));
        }
    }
}
