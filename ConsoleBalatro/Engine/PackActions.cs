using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using ConsoleBalatro.Engine.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public static class PackActions
    {
        public class PackOpeningData
        {
            public BuyItemType RelevantBuyItemType = BuyItemType.NONE;
            public bool isImmediateActivation = false;
            public bool drawHandOnOpen = false;
            public bool DrawsToZone = false;
            public CardZone ZoneToDrawTo = null;
        }

        private static Dictionary<BuyItemType, PackOpeningData> PackDataForOpenings;

        public static void InitializePackData()
        {
            PackDataForOpenings = new()
            {
                {
                    BuyItemType.PLAYING_CARD,
                    new PackOpeningData()
                    {
                        RelevantBuyItemType = BuyItemType.PLAYING_CARD,
                        ZoneToDrawTo = ZoneManager.DeckZone,
                        DrawsToZone = true,
                    }
                },
                {
                    BuyItemType.JOKER,
                    new PackOpeningData()
                    {
                        RelevantBuyItemType = BuyItemType.JOKER,
                        ZoneToDrawTo = ZoneManager.JokerZone,
                        DrawsToZone = true,
                    }
                },
                {
                    BuyItemType.PLANET_CARD,
                    new PackOpeningData()
                    {
                        RelevantBuyItemType = BuyItemType.PLANET_CARD,
                        isImmediateActivation = true,
                    }
                },
                {
                    BuyItemType.TAROT_CARD,
                    new PackOpeningData()
                    {
                        RelevantBuyItemType = BuyItemType.TAROT_CARD,
                        isImmediateActivation = true,
                        drawHandOnOpen = true,
                    }
                },
                {
                    BuyItemType.SPECTRAL_CARD,
                    new PackOpeningData()
                    {
                        RelevantBuyItemType = BuyItemType.SPECTRAL_CARD,
                        isImmediateActivation = true,
                        drawHandOnOpen = true,
                    }
                }
            };
        }

        public static void OpenPack(Card pack)
        {
            if (!pack.isPack)
            {
                return;
            }

            FlowHandler.OpenPackSelectionRound(pack);

            //Draw relevant items to pack option zone.
            var packInfo = ConsumableManager.PackBasicNums[Globals.CurrentGameStateObj.TargetPack.MyPackType];
            var odds = new Dictionary<BuyItemType, int>() { { packInfo.RelevantBuyItemType, 1} };
            var args = new EngineOddsEstablishedForPackArgs() { Odds = odds, PackBeingOpened = Globals.CurrentGameStateObj.TargetPack.MyPackType, PackDataBeingOpened = packInfo };
            args.MyContext = new EventContext()
            {
                Context = EventContextType.PackOddsEstablished
            };
            EngineEventHandler.TriggerEvent(args);
            //MarketOptionsManager.DrawNumMarketItems(packInfo.RelevantBuyItemType, packInfo.NumOptionsPresented, ZoneManager.PackOptionZone);
            var batchInfo = new ContentRollBatchContext();
            for (int i = ZoneManager.PackOptionZone.Cards.Count; i < packInfo.NumOptionsPresented; i++) //lookit my pretty lil hack i got here :)
            {
                var type = MarketPullManager.ChooseRollItemByOdds(args.Odds);
                MarketPullManager.DrawMarketItem(type, ZoneManager.PackOptionZone, source: GenerationSource.Pack, batchContext: batchInfo);
            }
            //MarketOptionsManager.DrawItemsByOdds(packInfo.NumOptionsPresented, ZoneManager.PackOptionZone, args.Odds);

            //OPTIONS:
            //JOKER: choose to add to jokerzone
            //CARD: choose to add to deck
            //TAROT/SPECTRAL: draw a hand of cards first. Then choose one to activate immediately.
            //PLANET: Choose one to activate immediately.

            var packOpenData = PackDataForOpenings[packInfo.RelevantBuyItemType];
            if (packOpenData.drawHandOnOpen)
            {
                ZoneManager.ShuffleDeck();
                ZoneManager.DrawHandful();
            }
        }

        public static void SkipCurrentPack()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.PackSkip } });
            EndCurrentPack();
        }

        public static void EndCurrentPack()
        {
            if(Globals.CurrentGameState != GameState.SelectingPackOption)
            {
                return;
            }
            //TODO: Other stuff? At least emit an event later.
            //Is this even necessary? Can we just go direct to FlowHandler call.

            FlowHandler.ClosePackSelectionRound();
        }

        public static bool CanAcceptPackOption(Card optionAttempted)
        {
            var packInfo = ConsumableManager.PackBasicNums[Globals.CurrentGameStateObj.TargetPack.MyPackType];
            switch (packInfo.RelevantBuyItemType)
            {
                case BuyItemType.PLAYING_CARD:
                    return true;
                case BuyItemType.TAROT_CARD:
                case BuyItemType.PLANET_CARD:
                case BuyItemType.SPECTRAL_CARD:
                    //TODO: pass args here? So far none need them.
                    return optionAttempted.ConsumableData.IsActivatable(null);
                case BuyItemType.JOKER:
                    return ZoneManager.JokerZone.HasRoomFor(optionAttempted);
                case BuyItemType.VOUCHER:
                case BuyItemType.NONE:
                default:
                    return false;
            }
        }

        public static void PackOptionSelectionMade(Card optionSelected)
        {
            if(Globals.CurrentGameState != GameState.SelectingPackOption || (!OptionMatchesPackType(optionSelected, Globals.CurrentGameStateObj.TargetPack.MyPackType)))
            {
                return;
            }

            var packInfo = ConsumableManager.PackBasicNums[Globals.CurrentGameStateObj.TargetPack.MyPackType];

            switch (packInfo.RelevantBuyItemType)
            {
                case BuyItemType.NONE:
                    break;
                case BuyItemType.PLAYING_CARD:
                    //ADD OPT TO DECK
                    ZoneManager.DeckZone.DrawTargetFrom(ZoneManager.PackOptionZone, optionSelected);
                    break;
                case BuyItemType.TAROT_CARD:
                case BuyItemType.PLANET_CARD:
                case BuyItemType.SPECTRAL_CARD:
                    //ACTIVATE OPT
                    ConsumableManager.UseConsumable(optionSelected, ZoneManager.PackOptionZone);
                    break;
                case BuyItemType.JOKER:
                    //ADD TO ZONE
                    //TODO: ONLY ALLOW IF THERE IS ROOM
                    ZoneManager.JokerZone.DrawTargetFrom(ZoneManager.PackOptionZone, optionSelected);
                    break;
                case BuyItemType.VOUCHER:
                    //HUH?
                    break;
                default:
                    break;
            }
            //INCREMENT NUM OPTIONS SELECTED IN GAMESTATE OBJ.
            //IF OUT OF OPTIONS TO CHOOSE, CLEAR OUT SELECTION ZONE.
            //IF THAT OR NO OPTIONS REMAINING, EXIT PACK SELECTION GAME STATE.
            Globals.CurrentGameStateObj.NumChoicesAlreadyMade++;
            var optRemNum = ZoneManager.PackOptionZone.Cards.Count;
            if (packInfo.NumCanBeTaken <= Globals.CurrentGameStateObj.NumChoicesAlreadyMade || optRemNum == 0)
            {
                EndCurrentPack();
            }
        }

        public static bool OptionMatchesPackType(Card option, PackType packType)
        {
            var packBuyType = ConsumableManager.PackBasicNums[packType].RelevantBuyItemType;
            switch (packBuyType)
            {
                case BuyItemType.PLAYING_CARD:
                    return true;
                case BuyItemType.JOKER:
                    return option.isJoker;
                case BuyItemType.PLANET_CARD:
                    return option.isConsumable && option.ConsumableData.Type == ConsumableType.PLANET;
                case BuyItemType.TAROT_CARD:
                    return option.isConsumable && option.ConsumableData.Type == ConsumableType.TAROT;
                case BuyItemType.SPECTRAL_CARD:
                    return option.isConsumable && option.ConsumableData.Type == ConsumableType.SPECTRAL;
                default:
                    return false;
            }
        }
    }
}
