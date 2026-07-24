using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Vouchers;
using ConsoleBalatro.Engine.Pools.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Market
{
    public static class MarketOptionsManager
    {
        public static List<PlayedHandType> HiddenPlanets => HiddenPlanetsRevealed.Keys.ToList();

        public static Dictionary<PlayedHandType, bool> HiddenPlanetsRevealed = new()
        {
            {PlayedHandType.FIVEOFAKIND, false },
            {PlayedHandType.FLUSHHOUSE, false },
            {PlayedHandType.FLUSHFIVE, false },
        };

        public static bool IsHiddenPlanet(PlayedHandType handType) => HiddenPlanets.Contains(handType);

        public static Dictionary<BuyItemType, int> MainMarketWeights = new()
        {
            {BuyItemType.JOKER, 40 },
            {BuyItemType.TAROT_CARD, 5 },
            {BuyItemType.PLANET_CARD, 5 },
        };

        //NOTE: All chances are odds in 1000 (to do decimal odds with ints, for some reason, its how balatro does it)
        //We index first on item type in case later items have diff edition odds.
        public static Dictionary<BuyItemType, Dictionary<Edition, int>> RandomEditionOdds = new()
        {
            {BuyItemType.JOKER, new Dictionary<Edition, int>()
            {
                { Edition.FOIL, 20 },
                { Edition.HOLOGRAPHIC, 14 },
                { Edition.POLYCHROME, 3 },
                { Edition.NEGATIVE, 3 },
            } },
        };

        public static int MainMarketWeightsTotal => MainMarketWeights.Values.Sum();

        public static Dictionary<BuyItemType, CardZone> MarketPoolsToDrawFrom = new();

        public static CardZone SpecialPool_LegendaryJokers;

        public static CardZone SpecialPool_HiddenPlanets;

        public static void InitializeMarketPools()
        {
            ////Initialize special pools
            //SpecialPool_LegendaryJokers = ZoneManager.MakeZone("LegendaryJokersPool");
            //SpecialPool_HiddenPlanets = ZoneManager.MakeZone("HiddenPlanetsPool");

            ////Reset hidden planet trackers
            //foreach (var k in HiddenPlanets)
            //{
            //    HiddenPlanetsRevealed[k] = false;
            //}

            ////Reset market odds
            //MainMarketWeights = new()
            //{
            //    {BuyItemType.JOKER, 40 },
            //    {BuyItemType.TAROT_CARD, 5 },
            //    {BuyItemType.PLANET_CARD, 5 },
            //};

            ////Reset edition odds
            //RandomEditionOdds = new()
            //{
            //    {BuyItemType.JOKER, new Dictionary<Edition, int>()
            //    {
            //        { Edition.FOIL, 20 },
            //        { Edition.HOLOGRAPHIC, 14 },
            //        { Edition.POLYCHROME, 3 },
            //        { Edition.NEGATIVE, 3 },
            //    } },
            //};

            //foreach (var k in MarketPoolsToDrawFrom.Keys)
            //{
            //    MarketPoolsToDrawFrom[k].ClearCards();
            //}
            //MarketPoolsToDrawFrom.Clear();
            //foreach (BuyItemType buyItem in Enum.GetValues(typeof(BuyItemType)))
            //{
            //    //Done I think????
            //    switch(buyItem)
            //    {
            //        case BuyItemType.PLAYING_CARD:
            //            var cardZoneToAdd = ZoneManager.MakeBasicDeck();
            //            cardZoneToAdd.Name = "CardPool";
            //            MarketPoolsToDrawFrom.Add(buyItem, cardZoneToAdd);
            //            break;
            //        case BuyItemType.PLANET_CARD:
            //            var planetPool = ZoneManager.MakeZone("PlanetPool");
            //            foreach (var planetType in DefaultPlanetsAvailable)
            //            {
            //                var pc = new Card();
            //                ConsumableManager.MakeCardPlanetCard(planetType, pc);
            //                planetPool.AddCard(pc, invisibleAdd: true);
            //            }
            //            MarketPoolsToDrawFrom.Add(buyItem, planetPool);
            //            foreach (var planetType in HiddenPlanets)
            //            {
            //                var pc = new Card();
            //                ConsumableManager.MakeCardPlanetCard(planetType, pc);
            //                SpecialPool_HiddenPlanets.AddCard(pc, invisibleAdd: true);
            //            }
            //            break;
            //        case BuyItemType.TAROT_CARD:
            //            var tarotPool = ZoneManager.MakeZone("TarotPool");
            //            foreach (var tarotType in ConsumableManager.TarotNames)
            //            {
            //                tarotPool.AddCard(ConsumableManager.MakeTarotCard(tarotType), invisibleAdd: true);
            //            }
            //            MarketPoolsToDrawFrom.Add(buyItem, tarotPool);
            //            break;
            //        case BuyItemType.SPECTRAL_CARD:
            //            var spectralPool = ZoneManager.MakeZone("SpectralPool");
            //            foreach (var spectralType in ConsumableManager.SpectralNames)
            //            {
            //                spectralPool.AddCard(ConsumableManager.MakeSpectralCard(spectralType), invisibleAdd: true);
            //            }
            //            MarketPoolsToDrawFrom.Add(buyItem, spectralPool);
            //            break;
            //        case BuyItemType.JOKER:
            //            var jokerPool = ZoneManager.MakeZone("JokerPool");
            //            foreach (var joker in JokerDb.JokerDbNames)
            //            {
            //                var card = new Card();
            //                JokerDb.MakeCardJoker(card, joker);
            //                //TODO: Need better approach here.
            //                if(card.JokerData.Rarity != JokerRarity.LEGENDARY)
            //                {
            //                    jokerPool.AddCard(card, invisibleAdd: true);
            //                }
            //                else
            //                {
            //                    SpecialPool_LegendaryJokers.AddCard(card, invisibleAdd: true);
            //                }
            //            }
            //            MarketPoolsToDrawFrom.Add(buyItem, jokerPool);
            //            break;
            //        case BuyItemType.VOUCHER:
            //            var voucherPool = ZoneManager.MakeZone("VoucherPool");
            //            foreach (var voucher in VoucherDb.VoucherDBNames)
            //            {
            //                var card = new Card();
            //                VoucherDb.MakeCardVoucher(card, voucher);
            //                if (card.JokerData.voucherIsBase)
            //                {
            //                    voucherPool.AddCard(card, invisibleAdd: true);
            //                }
            //                else
            //                {
            //                    VoucherDb.VoucherDependants.Add(card.JokerData.PredecessorVoucherDBName, card.JokerData.DBName);
            //                }
            //            }
            //            MarketPoolsToDrawFrom.Add(buyItem, voucherPool);
            //            break;
            //        default:
            //            MarketPoolsToDrawFrom.Add(buyItem, new());
            //            break;
            //    }
            //}

            //ShufflePools();

            VoucherPoolRules.CurrentValidVouchers.Clear();
            foreach (var voucher in VoucherDb.VoucherDBNames)
            {
                //TODO: Jesus christ this is bad. I mean really, really bad code.
                //because we never wrote voucher metadata into its own separate static thing beyond the builders, like we did with metadata......
                //we actually have to build a voucher before we can determine whether its a base or not.
                //holy hell.
                var card = new Card();
                VoucherDb.MakeCardVoucher(card, voucher);
                if (card.JokerData.voucherIsBase)
                {
                    VoucherPoolRules.CurrentValidVouchers.Add(voucher);
                }
                else
                {
                    VoucherDb.VoucherDependants.Add(card.JokerData.PredecessorVoucherDBName, card.JokerData.DBName);
                }
            }
        }

        public static void RevealHiddenPlanet(PlayedHandType handType)
        {
            /*MarketPoolsToDrawFrom[BuyItemType.PLANET_CARD].DrawTargetFrom(SpecialPool_HiddenPlanets, SpecialPool_HiddenPlanets.Cards.First(x => x.ConsumableData.PlanetHandType == handType), invisibleAdd: true);
            HiddenPlanetsRevealed[handType] = true;*/
            //ABOVE NO LONGER NEEDED: however, need this func to exist for the future.
        }

        public static void AddToVoucherPool(string DBNameToAdd)
        {
            //if (MarketPoolsToDrawFrom[BuyItemType.VOUCHER] != null && MarketPoolsToDrawFrom[BuyItemType.VOUCHER].Cards.Where(x => x.isVoucher && x.JokerData.DBName == DBNameToAdd).Any())
            //{
            //    return; //Don't add dupe of existing
            //}
            //Card c = VoucherDb.GenerateVoucherCard(DBNameToAdd);
            //MarketPoolsToDrawFrom[BuyItemType.VOUCHER].AddCard(c, invisibleAdd: true);
            if (VoucherPoolRules.CurrentValidVouchers.Contains(DBNameToAdd))
            {
                return;//don't add dupe of existing.
            }
            VoucherPoolRules.CurrentValidVouchers.Add(DBNameToAdd);
        }

        public static void AttemptToRemoveFromVoucherPool(string DBNameToRemove)
        {
            //if(!MarketPoolsToDrawFrom[BuyItemType.VOUCHER].Cards.Where(x => x.isVoucher && x.JokerData.DBName == DBNameToRemove).Any())
            //{
            //    return; //it's not there, stop.
            //}
            //var target = MarketPoolsToDrawFrom[BuyItemType.VOUCHER].Cards.Where(x => x.isVoucher && x.JokerData.DBName == DBNameToRemove).First();
            //MarketPoolsToDrawFrom[BuyItemType.VOUCHER].RemoveCard(target, invisibleRemove: true);
            if(VoucherPoolRules.CurrentValidVouchers.Contains(DBNameToRemove))
                VoucherPoolRules.CurrentValidVouchers.Remove(DBNameToRemove);
        }

        //returns whether it successfully returned the item.
        //public static bool ReturnMarketItemFromZone(Card cardToReturn, CardZone zoneFrom)
        //{
        //    if (cardToReturn.isConsumable
        //        && MarketPoolsToDrawFrom.ContainsKey(cardToReturn.ConsumableData.BuyType)
        //        && !MarketPoolsToDrawFrom[cardToReturn.ConsumableData.BuyType].Cards.Where(x => x.isConsumable && x.ConsumableData.ConsumableName == cardToReturn.ConsumableData.ConsumableName).Any()
        //        )
        //    {
        //        MarketPoolsToDrawFrom[cardToReturn.ConsumableData.BuyType].DrawTargetFrom(zoneFrom, cardToReturn, invisibleAdd: true);
        //        cardToReturn.ClearExtras();
        //        MarketPoolsToDrawFrom[cardToReturn.ConsumableData.BuyType].Shuffle();
        //    }
        //    else if (cardToReturn.isJoker
        //        && !MarketPoolsToDrawFrom[BuyItemType.JOKER].Cards.Where(x => x.isJoker && x.JokerData.JokerName == cardToReturn.JokerData.JokerName).Any())
        //    {
        //        MarketPoolsToDrawFrom[BuyItemType.JOKER].DrawTargetFrom(zoneFrom, cardToReturn, invisibleAdd: true);
        //        cardToReturn.ClearExtras();
        //        MarketPoolsToDrawFrom[BuyItemType.JOKER].Shuffle();
        //    }else if(cardToReturn.isVoucher && !MarketPoolsToDrawFrom[BuyItemType.VOUCHER].Cards.Where(x => x.isVoucher && x.JokerData.JokerName == cardToReturn.JokerData.JokerName).Any())
        //    {
        //        MarketPoolsToDrawFrom[BuyItemType.VOUCHER].DrawTargetFrom(zoneFrom, cardToReturn, invisibleAdd: true);
        //        cardToReturn.ClearExtras();
        //        MarketPoolsToDrawFrom[BuyItemType.VOUCHER].Shuffle();
        //    }
        //    else
        //    {
        //        return false;
        //    }
                
        //    return true;
        //}
        public static bool ReturnMarketItemFromZone(Card cardToReturn, CardZone zoneFrom)
        {
            //ZoneManager.DestroyCard(cardToReturn, zoneFrom);
            ZoneManager.DeleteCard(cardToReturn);
            return true;
        }

        public static void DrawMarketItem(BuyItemType itemType, CardZone zoneToDrawTo, bool applyMarketModifiers = false, bool overrideSpaceLimits = false)
        {
            //TODO: When duplicates allowed, draw from some static pool.
            var cardDrawn = zoneToDrawTo.DrawFromAndReturn(MarketPoolsToDrawFrom[itemType], ignoreSpaceLimits: overrideSpaceLimits);

            //TODO: Wow this is so gross. Look at that nesting.
            //like I get why I did it but... please. make it better.
            if(applyMarketModifiers && cardDrawn != null)
            {
                //First, editions. Later maybe others?
                if (RandomEditionOdds.ContainsKey(itemType) && cardDrawn.Edition == Edition.BASE)
                {
                    foreach (var ed in RandomEditionOdds[itemType].Keys)
                    {
                        if(Globals.randomNext(1000) < RandomEditionOdds[itemType][ed])
                        {
                            cardDrawn.Edition = ed;
                            break; //Not do this? later rolls override?
                        }
                    }
                }

                //NOTE: TEMP IMPLEMENTATION OF ILLUSION
                if(itemType == BuyItemType.PLAYING_CARD && Globals.ShopPlayingCardsGetModifiers)
                {
                    var doEdition = Globals.ChooseRandomInclusive(1, 10) <= 2;//20% odds
                    List<Edition> validEditions = new List<Edition>() { Edition.FOIL, Edition.HOLOGRAPHIC, Edition.POLYCHROME };
                    var doEnhancement = Globals.ChooseRandomInclusive(1, 10) <= 4;//40% odds
                    List<Enhancement> validEnhancements = new List<Enhancement>() { Enhancement.MULT, Enhancement.BONUSCHIPS, Enhancement.LUCKY, Enhancement.GLASS, Enhancement.WILD, Enhancement.GOLD, Enhancement.STEEL };
                    var doSeal = Globals.MIRROR_ILLUSION_SEAL_GLITCH ? false : Globals.ChooseRandomInclusive(1, 10) <= 2;//20% if enabled.
                    List<Seal> validSeals = new List<Seal>() { Seal.GOLD, Seal.RED, Seal.BLUE, Seal.PURPLE };
                    if (doEdition)
                    {
                        cardDrawn.SetEditionOfficial(validEditions[Globals.randomNext(validEditions.Count)]);
                    }
                    if (doEnhancement)
                    {
                        cardDrawn.SetEnhancementOfficial(validEnhancements[Globals.randomNext(validEnhancements.Count)]);
                    }
                    if (doSeal)
                    {
                        cardDrawn.SetSealOfficial(validSeals[Globals.randomNext(validSeals.Count)]);
                    }
                }
            }
        }

        public static void DrawMarketItems(List<BuyItemType> itemTypes, CardZone zoneToDrawTo)
        {
            foreach (var type in itemTypes)
            {
                DrawMarketItem(type, zoneToDrawTo);
            }
        }

        public static void DrawItemsByMainMarketOdds(int numItems, CardZone zoneToDrawTo, bool applyMarketModifiers = false)
        {
            //Simply draws based on market odds. Other tweaks (edition mainly) happen elsewhere (in DrawMarketItem).
            DrawItemsByOdds(numItems, zoneToDrawTo, MainMarketWeights, applyMarketModifiers);
        }

        public static void DrawItemsByOdds(int numItems, CardZone zoneToDrawTo, Dictionary<BuyItemType, int> odds, bool applyMarketModifiers = false)
        {
            var total = odds.Values.Sum();
            for (int i = 0; i < numItems; i++)
            {
                var roll = Globals.randomNext(total);
                BuyItemType chosenType = BuyItemType.NONE;
                foreach (var typeOpt in odds)
                {
                    if (roll < typeOpt.Value)
                    {
                        chosenType = typeOpt.Key;
                        break;
                    }
                    else
                    {
                        roll -= typeOpt.Value;
                    }
                }
                if (chosenType == BuyItemType.NONE)
                {
                    //TEMPORARILY: default to Joker
                    chosenType = BuyItemType.JOKER;
                }
                DrawMarketItem(chosenType, zoneToDrawTo, applyMarketModifiers: applyMarketModifiers);
            }
        }
    }

    public enum BuyItemType
    {
        NONE,
        PLAYING_CARD,
        TAROT_CARD,
        PLANET_CARD,
        SPECTRAL_CARD,
        JOKER,
        VOUCHER,
    }
}
