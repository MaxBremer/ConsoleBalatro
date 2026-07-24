using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
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
            FillMainMarket();

            Globals.CurrentRerollCost = Globals.BaseRerollCost;
            Globals.ChaosClownFreeRerollAvailable = false;

            for (int i = 0; i < ZoneManager.PackMarketZone.MaxCapacity; i++)
            {
                ZoneManager.PackMarketZone.AddCard(ConsumableManager.MakePackByOdds(), invisibleAdd: false); //not an invisible add due to graphics, idk maybe needed later.
            }

            EngineEventHandler.TriggerEvent(new EngineEventArgs()
            {
                MyContext = new EventContext() { Context = EventContextType.MarketSetupDone },
            });
        }

        //public static void FillMainMarket() => MarketOptionsManager.DrawItemsByMainMarketOddsUntilFull(ZoneManager.MainMarketZone, true);
        public static void FillMainMarket() => MarketPullManager.FillMainMarket();

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
            var costPaid = Globals.CurrentRerollCost;
            Globals.EmitMoneyLoss(Globals.CurrentRerollCost, Globals.RerollButtonCard, false);
            if (Globals.ChaosClownFreeRerollAvailable && Globals.CurrentRerollCost == 0)
            {
                Globals.ChaosClownFreeRerollAvailable = false;
                Globals.CurrentRerollCost = Globals.BaseRerollCost;
            }
            else
            {
                Globals.CurrentRerollCost += 1;
            }

            var toRem = new List<Card>();
            toRem.AddRange(ZoneManager.MainMarketZone.Cards);
            foreach (var c in toRem)
            {
                MarketOptionsManager.ReturnMarketItemFromZone(c, ZoneManager.MainMarketZone);
            }

            FillMainMarket();

            EngineEventHandler.TriggerEvent(new EngineShopRerollArgs() { CostPaid = costPaid });
        }

        //Tho in game player can't actually refresh pack market, this is for debug.
        public static void DebugRefreshPackMarket()
        {
            var toRem = new List<Card>();
            toRem.AddRange(ZoneManager.PackMarketZone.Cards);
            foreach (var c in toRem)
            {
                ZoneManager.DeleteCard(c);
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
                MarketPullManager.DrawMarketItem(BuyItemType.VOUCHER, ZoneManager.VoucherMarketZone, source: Pools.GenerationSource.Market);
                //MarketOptionsManager.DrawMarketItem(BuyItemType.VOUCHER, ZoneManager.VoucherMarketZone);
            }
        }

        //Just used for coupon voucher: makes current market free
        public static void MakeMarketFree(bool includeVouchers = false)
        {
            var allTargets = new List<Card>();
            allTargets.AddRange(ZoneManager.MainMarketZone.Cards);
            allTargets.AddRange(ZoneManager.PackMarketZone.Cards);
            if (includeVouchers)
                allTargets.AddRange(ZoneManager.VoucherMarketZone.Cards);

            foreach (var c in allTargets)
            {
                c.BuyCostOverride = 0;
            }
        }
    }
}
