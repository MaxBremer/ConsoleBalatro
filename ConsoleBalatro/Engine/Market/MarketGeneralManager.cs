using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Market
{
    public static class MarketGeneralManager
    {
        public static void FillFreshMarket()
        {
            MarketOptionsManager.DrawItemsByMainMarketOddsUntilFull(ZoneManager.MainMarketZone, true);

            for (int i = 0; i < ZoneManager.PackMarketZone.MaxCapacity; i++)
            {
                ZoneManager.PackMarketZone.AddCard(ConsumableManager.MakePackByOdds(), invisibleAdd: false); //not an invisible add due to graphics, idk maybe needed later.
            }
        }

        public static void MarketClosing()
        {
            var toRem = new List<Card>();
            toRem.AddRange(ZoneManager.MainMarketZone.Cards);
            foreach (var c in toRem)
            {
                MarketOptionsManager.ReturnMarketItemFromZone(c, ZoneManager.MainMarketZone);
            }

            //No pool to return packs to.
            ZoneManager.PackMarketZone.ClearCards(); //NOT INVISIBLE DUE TO UI. for now at least.
        }

        public static void RerollMainMarket()
        {
            if (!Globals.CanAfford(Globals.CurrentRerollCost))
            {
                return;
            }
            Globals.EmitMoneyLoss(Globals.CurrentRerollCost, Globals.RerollButtonCard, false);
            //TODO: increase reroll cost
            var toRem = new List<Card>();
            toRem.AddRange(ZoneManager.MainMarketZone.Cards);
            foreach (var c in toRem)
            {
                MarketOptionsManager.ReturnMarketItemFromZone(c, ZoneManager.MainMarketZone);
            }

            MarketOptionsManager.DrawItemsByMainMarketOddsUntilFull(ZoneManager.MainMarketZone, true);
        }

        //Tho in game player can't actually refresh pack market, this is for debug.
        public static void DebugRefreshPackMarket()
        {
            var toRem = new List<Card>();
            toRem.AddRange(ZoneManager.PackMarketZone.Cards);
            foreach (var c in toRem)
            {
                ZoneManager.DestroyCard(c, c.MyZone);
            }

            for (int i = 0; i < ZoneManager.PackMarketZone.MaxCapacity; i++)
            {
                ZoneManager.PackMarketZone.AddCard(ConsumableManager.MakePackByOdds(), invisibleAdd: false); //not an invisible add for now, due to graphics display, idk maybe needed later.
            }
        }

        //Moved voucher change to here so we can trigger manually at new ante.
        public static void ResetVoucher()
        {
            if (ZoneManager.VoucherMarketZone.Cards.Count > 0)
            {
                var toRem = new List<Card>();
                toRem.AddRange(ZoneManager.VoucherMarketZone.Cards);
                foreach (var c in toRem)
                {
                    MarketOptionsManager.ReturnMarketItemFromZone(c, ZoneManager.VoucherMarketZone);
                }
            }

            while (ZoneManager.VoucherMarketZone.HasRoom)
            {
                MarketOptionsManager.DrawMarketItem(BuyItemType.VOUCHER, ZoneManager.VoucherMarketZone);
            }
        }
    }
}
