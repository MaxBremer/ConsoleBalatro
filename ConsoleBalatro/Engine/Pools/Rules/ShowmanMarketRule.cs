using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public sealed class ShowmanMarketRule : IMarketPoolRule
    {
        public int Priority => 10;

        public void ModifyCandidates(MarketPoolContext context)
        {
            bool hasShowman = ZoneManager.JokerZone.Cards
                .Any(j => j.isJoker && j.JokerData.DBName == "SHOWMAN");

            if (!hasShowman)
                return;

            if (context.Pool is ItemPool.Joker or ItemPool.Tarot or ItemPool.Planet or ItemPool.Spectral)
            {
                context.AllowOwnedDuplicates = true;
                context.AllowDuplicateResultsInSameBatch = true;
            }
        }
    }
}
