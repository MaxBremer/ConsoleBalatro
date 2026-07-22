using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public sealed class AchievementUnlockJokerRule : IMarketPoolRule
    {
        public int Priority => 11;

        public void ModifyCandidates(MarketPoolContext context)
        {
            if(context.Pool == ItemPool.Joker)
            {
                context.Candidates.RemoveAll(x => 
                    JokerDb.JokerMetadata.ContainsKey(x.Definition.Id) && 
                    !string.IsNullOrEmpty(JokerDb.JokerMetadata[x.Definition.Id].AchievementForUnlock) && 
                    !UnlockManager.IsAchievementAchieved(JokerDb.JokerMetadata[x.Definition.Id].AchievementForUnlock)
                );
            }
        }
    }
}
