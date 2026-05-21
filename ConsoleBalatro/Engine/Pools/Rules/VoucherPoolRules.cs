using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools.Rules
{
    public sealed class VoucherPoolRules : IMarketPoolRule
    {
        public static List<string> CurrentValidVouchers = new List<string>();

        public int Priority => 10;

        public void ModifyCandidates(MarketPoolContext context)
        {
            if(context.Pool == ItemPool.Voucher)
            {
                context.Candidates.RemoveAll(x => !CurrentValidVouchers.Contains(x.Definition.Id));
            }
        }
    }
}
