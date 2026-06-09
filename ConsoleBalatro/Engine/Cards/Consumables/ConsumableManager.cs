using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using ConsoleBalatro.Engine.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Consumables
{
    public static class ConsumableManager
    {
        public class PackData
        {
            public int BasePackPrice;
            public int NumOptionsPresented;
            public int NumCanBeTaken;
            public int ChanceToAppear; //This is out of 10,000 (for percentages).
            public BuyItemType RelevantBuyItemType = BuyItemType.NONE;
            public string PackName;//TODO: THESE FIELDS ARE DISPLAY FIELDS. MOVE TO DISPLAY CLASSES.
            public string TopLine;
            public string BottomLine;
            public string ID => PackName.ToUpper();


            public string GetItemTypeString()
            {
                if (ItemTypeString.ContainsKey(RelevantBuyItemType))
                {
                    return ItemTypeString[RelevantBuyItemType];
                }
                return "";
            }
        }

        public static Dictionary<BuyItemType, string> ItemTypeString = new()
        {
            {BuyItemType.JOKER, "Jokers to obtain." },
            {BuyItemType.PLAYING_CARD, "Playing Cards to add to your deck." },
            {BuyItemType.TAROT_CARD, "Tarot Cards to use immediately." },
            {BuyItemType.PLANET_CARD, "Planet Cards to use immediately." },
            {BuyItemType.SPECTRAL_CARD, "Spectral Cards to use immediately." },
        };

        private static Dictionary<Seal, string> SealString = new()
        {
            { Seal.NONE, "No Seal" },
            { Seal.RED, "Red Seal" },
            { Seal.GOLD, "Gold Seal" },
            { Seal.BLUE, "Blue Seal" },
            { Seal.PURPLE, "Purple Seal" },
        };

        public static Dictionary<PackType, PackData> PackBasicNums = new()
        {
            { PackType.NONE, new() { 
                NumCanBeTaken = 1,
                NumOptionsPresented = 1,
                BasePackPrice = 1,
                ChanceToAppear = 0,
                RelevantBuyItemType= BuyItemType.NONE,
                PackName = "",
                TopLine = "",
                BottomLine = "",
            } },
            { PackType.BASIC_JOKER, new() {
                NumCanBeTaken = 1,
                NumOptionsPresented = 2,
                BasePackPrice = 4,
                ChanceToAppear = 535,
                RelevantBuyItemType= BuyItemType.JOKER,
                PackName = "Basic Joker Pack",
                TopLine = "BAS",
                BottomLine = "JOK",
            } },
            { PackType.BASIC_STANDARD, new() {
                NumCanBeTaken = 1,
                NumOptionsPresented = 3,
                BasePackPrice = 4,
                ChanceToAppear = 1784,
                RelevantBuyItemType= BuyItemType.PLAYING_CARD,
                PackName = "Basic Card Pack",
                TopLine = "BAS",
                BottomLine = "CAR",
            } },
            { PackType.BASIC_TAROT, new() {
                NumCanBeTaken = 1,
                NumOptionsPresented = 3,
                BasePackPrice = 4,
                ChanceToAppear = 1784,
                RelevantBuyItemType= BuyItemType.TAROT_CARD,
                PackName = "Basic Tarot Pack",
                TopLine = "BAS",
                BottomLine = "TAR",
            } },
            { PackType.BASIC_PLANET, new() {
                NumCanBeTaken = 1,
                NumOptionsPresented = 3,
                BasePackPrice = 4,
                ChanceToAppear = 1784,
                RelevantBuyItemType= BuyItemType.PLANET_CARD,
                PackName = "Basic Planet Pack",
                TopLine = "BAS",
                BottomLine = "PLA",
            } },
            { PackType.BASIC_SPECTRAL, new() {
                NumCanBeTaken = 1,
                NumOptionsPresented = 2,
                BasePackPrice = 4,
                ChanceToAppear = 267,
                RelevantBuyItemType= BuyItemType.SPECTRAL_CARD,
                PackName = "Basic Spectral Pack",
                TopLine = "BAS",
                BottomLine = "SPE",
            } },
            { PackType.JUMBO_JOKER, new() {
                NumCanBeTaken = 1,
                NumOptionsPresented = 4,
                BasePackPrice = 6,
                ChanceToAppear = 267,
                RelevantBuyItemType= BuyItemType.JOKER,
                PackName = "Jumbo Joker Pack",
                TopLine = "JUM",
                BottomLine = "JOK",
            } },
            { PackType.JUMBO_STANDARD, new() {
                NumCanBeTaken = 1,
                NumOptionsPresented = 5,
                BasePackPrice = 6,
                ChanceToAppear = 892,
                RelevantBuyItemType= BuyItemType.PLAYING_CARD,
                PackName = "Jumbo Card Pack",
                TopLine = "JUM",
                BottomLine = "CAR",
            } },
            { PackType.JUMBO_TAROT, new() {
                NumCanBeTaken = 1,
                NumOptionsPresented = 5,
                BasePackPrice = 6,
                ChanceToAppear = 892,
                RelevantBuyItemType= BuyItemType.TAROT_CARD,
                PackName = "Jumbo Tarot Pack",
                TopLine = "JUM",
                BottomLine = "TAR",
            } },
            { PackType.JUMBO_PLANET, new() {
                NumCanBeTaken = 1,
                NumOptionsPresented = 5,
                BasePackPrice = 6,
                ChanceToAppear = 892,
                RelevantBuyItemType= BuyItemType.PLANET_CARD,
                PackName = "Jumbo Planet Pack",
                TopLine = "JUM",
                BottomLine = "PLA",
            } },
            { PackType.JUMBO_SPECTRAL, new() {
                NumCanBeTaken = 1,
                NumOptionsPresented = 4,
                BasePackPrice = 6,
                ChanceToAppear = 134,
                RelevantBuyItemType= BuyItemType.SPECTRAL_CARD,
                PackName = "Jumbo Spectral Pack",
                TopLine = "JUM",
                BottomLine = "SPE",
            } },
            { PackType.MEGA_JOKER, new() {
                NumCanBeTaken = 2,
                NumOptionsPresented = 4,
                BasePackPrice = 8,
                ChanceToAppear = 66,
                RelevantBuyItemType= BuyItemType.JOKER,
                PackName = "Mega Joker Pack",
                TopLine = "MEG",
                BottomLine = "JOK",
            } },
            { PackType.MEGA_STANDARD, new() {
                NumCanBeTaken = 2,
                NumOptionsPresented = 5,
                BasePackPrice = 8,
                ChanceToAppear = 223,
                RelevantBuyItemType= BuyItemType.PLAYING_CARD,
                PackName = "Mega Card Pack",
                TopLine = "MEG",
                BottomLine = "CAR",
            } },
            { PackType.MEGA_TAROT, new() {
                NumCanBeTaken = 2,
                NumOptionsPresented = 5,
                BasePackPrice = 8,
                ChanceToAppear = 223,
                RelevantBuyItemType= BuyItemType.TAROT_CARD,
                PackName = "Mega Tarot Pack",
                TopLine = "MEG",
                BottomLine = "TAR",
            } },
            { PackType.MEGA_PLANET, new() {
                NumCanBeTaken = 2,
                NumOptionsPresented = 5,
                BasePackPrice = 8,
                ChanceToAppear = 223,
                RelevantBuyItemType= BuyItemType.PLANET_CARD,
                PackName = "Mega Planet Pack",
                TopLine = "MEG",
                BottomLine = "PLA",
            } },
            { PackType.MEGA_SPECTRAL, new() {
                NumCanBeTaken = 2,
                NumOptionsPresented = 4,
                BasePackPrice = 8,
                ChanceToAppear = 31,
                RelevantBuyItemType= BuyItemType.SPECTRAL_CARD,
                PackName = "Mega Spectral Pack",
                TopLine = "MEG",
                BottomLine = "SPE",
            } },
        };

        public static int PackTotalOdds => PackBasicNums.Values.Select(x => x.ChanceToAppear).Sum();

        public static List<string> SpecialSpectralNames = new() { "BLACK HOLE", "SOUL" };
        public static List<string> SpectralNames => SpectralConsumablesDb.Keys.ToList();
        public static Dictionary<string, Func<Card, ConsumableCardDataBlock>> SpectralConsumablesDb = new()
        {
            {"TALISMAN", c => BuildSealSpectral("Talisman", Seal.GOLD, c) },
            {"DEJA VU", c => BuildSealSpectral("Deja Vu", Seal.RED, c) },
            {"TRANCE", c => BuildSealSpectral("Trance", Seal.BLUE, c) },
            {"MEDIUM", c => BuildSealSpectral("Medium", Seal.PURPLE, c) },
            { "GRIM", c => BuildCardGenSpectral("Grim", "ACE", 2, c)},
            { "FAMILIAR", c => BuildCardGenSpectral("Familiar", "FACE", 3, c)},
            { "INCANTATION", c => BuildCardGenSpectral("Incantation", "NUMBERED", 4, c)},
            {"SIGIL", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Sigil";
                ret.DBName = "SIGIL";

                ret.DescriptionBuilder = _ => "Convert all cards in hand to a single random suit.";

                ret.IsActivatable = _ => ZoneManager.HandZone.Cards.Count > 0;
                ret.Use = _ =>
                {
                    var selSuit =  (Suit)Globals.ChooseRandomInclusive(1, 4);
                    foreach (var card in ZoneManager.HandZone.Cards)
                    {
                        card.SetSuitOfficial(selSuit);
                    }
                };

                return ret;
            } },
            {"OUIJA", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Ouija";
                ret.DBName = "OUIJA";

                ret.DataDict.Add("INTAMOUNT", new() {IntData = 1, MyDataType = JokerDataType.INT});
                ret.DescriptionBuilder = _ => "Convert all cards in hand to a single random rank. -" + ret.DataDict["INTAMOUNT"].IntData + " hand size.";

                ret.IsActivatable = _ => ZoneManager.HandZone.Cards.Count > 0;
                ret.Use = _ =>
                {
                    var selRank =  (Rank)Globals.ChooseRandomInclusive(1, 13);
                    foreach (var card in ZoneManager.HandZone.Cards)
                    {
                        card.SetRankOfficial(selRank);
                    }
                    Globals.HandSize -= ret.DataDict["INTAMOUNT"].IntData;
                };

                return ret;
            } },
            {"ECTOPLASM", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Ectoplasm";
                ret.DBName = "ECTOPLASM";

                ret.DataDict.Add("INTAMOUNT", new() {IntData = 1, MyDataType = JokerDataType.INT});
                ret.DescriptionBuilder = _ => "Add Negative to a random Joker. -" + ret.DataDict["INTAMOUNT"].IntData + " hand size.";

                ret.IsActivatable = _ => ZoneManager.JokerZone.Cards.Where(x => x.Edition == Edition.BASE).Count() > 0;
                ret.Use = _ =>
                {
                    var opts = ZoneManager.JokerZone.Cards.Where(x => x.Edition == Edition.BASE).ToList();
                    var selOpt = opts[Globals.ChooseRandomInclusive(0, opts.Count - 1)];
                    selOpt.SetEditionOfficial(Edition.NEGATIVE);
                    Globals.HandSize -= ret.DataDict["INTAMOUNT"].IntData;
                };

                return ret;
            } },
            {"IMMOLATE", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Immolate";
                ret.DBName = "IMMOLATE";

                ret.DataDict.Add("INTAMOUNT", new() {IntData = 5, MyDataType = JokerDataType.INT});
                ret.DataDict.Add("MONEYAMOUNT", new() {IntData = 20, MyDataType = JokerDataType.INT});
                
                ret.DescriptionBuilder = _ => "Destroy " + ret.DataDict["INTAMOUNT"].IntData + " random cards in hand, gain $"  + ret.DataDict["MONEYAMOUNT"].IntData;

                ret.IsActivatable = _ => ZoneManager.HandZone.Cards.Count >= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    var handCards = ZoneManager.HandZone.Cards.ToList();
                    var selCards = new List<Card>();
                    for (int i = 0; i < ret.DataDict["INTAMOUNT"].IntData; i++)
                    {
                        var chosenInd = Globals.ChooseRandomInclusive(0, handCards.Count - 1);
                        selCards.Add(handCards[chosenInd]);
                        handCards.RemoveAt(chosenInd);
			        }
                    foreach (var c in selCards)
                    {
                        ZoneManager.DestroyCard(c, ZoneManager.HandZone);
	                }
                    Globals.EmitMoneyGain(ret.DataDict["MONEYAMOUNT"].IntData, ret.MyCard);
                };

                return ret;
            } },
            {"AURA", c =>             {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Aura";
                ret.DBName = "AURA";
                ret.DataDict.Add("INTAMOUNT", new() {IntData = 1, MyDataType = JokerDataType.INT});
                ret.DescriptionBuilder = _ => "Add FOIL, HOLOGRAPHIC, or POLYCHROME edition (chosen at random) to " + ret.DataDict["INTAMOUNT"].IntData + " selected card(s) in hand.";
                ret.IsActivatable = _ => ZoneManager.CardsSelectedInHand.Count > 0 && ZoneManager.CardsSelectedInHand.Count <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    var opts = new List<Edition>() { Edition.FOIL, Edition.HOLOGRAPHIC, Edition.POLYCHROME };
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        var chosenOpt = opts[Globals.ChooseRandomInclusive(0, opts.Count - 1)];
                        card.SetEditionOfficial(chosenOpt);
                    }
                };
                return ret;
            } },

            {"WRAITH", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Wraith";
                ret.DBName = "WRAITH";
                ret.DescriptionBuilder = _ => "Generates a random Rare Joker (must have room), but sets money to 0.";
                ret.IsActivatable = _ => ZoneManager.JokerZone.HasRoom;
                ret.Use = _ =>
                {
                    //var rareJoker = MarketOptionsManager.PullRandomJokerFromPool(JokerRarity.RARE, removeFromPool: true);
                    var rareJoker = MarketPullManager.PickMarketCard(BuyItemType.JOKER, source: Pools.GenerationSource.WraithCard);
                    if (rareJoker != null)
                    {
                        ZoneManager.JokerZone.AddCard(rareJoker);
                    }
                    Globals.EmitMoneyLoss(Globals.Money, c, false);
                };
                return ret;
            } },
            {"CRYPTID", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Cryptid";
                ret.DBName = "CRYPTID";
                ret.DataDict.Add("INTAMOUNT", new() {IntData = 2, MyDataType = JokerDataType.INT});
                ret.DescriptionBuilder = _ => "Creates " + ret.DataDict["INTAMOUNT"].IntData + " exact copies of a selected card in your hand.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand == 1;
                ret.Use = _ =>
                {
                    var targetCard = ZoneManager.CardsSelectedInHand[0];
                    for (int i = 0; i < ret.DataDict["INTAMOUNT"].IntData; i++)
                    {
                        var newCard = new Card();
                        targetCard.TurnIntoCopyOfMe(newCard);
                        ZoneManager.HandZone.AddCard(newCard, overrideSpace: true);
                    }
                };
                return ret;
            } },
            {"ANKH", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Ankh";
                ret.DBName = "ANKH";
                ret.DescriptionBuilder = _ => "Create a copy of one of your Jokers. Destroy the rest.";
                ret.IsActivatable = _ => ZoneManager.JokerZone.Cards.Count > 0;
                ret.Use = _ =>
                {
                    var targetCard = ZoneManager.JokerZone.Cards[Globals.ChooseRandomInclusive(0, ZoneManager.JokerZone.Cards.Count - 1)];
                    var toRem = new List<Card>();
                    toRem.AddRange(ZoneManager.JokerZone.Cards.Where(x => x != targetCard));
                    foreach (var otherJoke in toRem)
                    {
                        ZoneManager.DestroyCard(otherJoke, ZoneManager.JokerZone);
                    }
                    ZoneManager.JokerZone.AddCard(targetCard.MakeCopy(), overrideSpace: true);
                };
                return ret;
            } },
            {"HEX", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Hex";
                ret.DBName = "HEX";
                ret.DescriptionBuilder = _ => "Make one of your Jokers POLYCHROME. Destroy the rest.";
                ret.IsActivatable = _ => ZoneManager.JokerZone.Cards.Where(x => x.Edition == Edition.BASE).Any();
                ret.Use = _ =>
                {
                    var targetCard = ZoneManager.JokerZone.Cards[Globals.ChooseRandomInclusive(0, ZoneManager.JokerZone.Cards.Count - 1)];
                    var toRem = new List<Card>();
                    toRem.AddRange(ZoneManager.JokerZone.Cards.Where(x => x != targetCard));
                    foreach (var otherJoke in toRem)
                    {
                        ZoneManager.DestroyCard(otherJoke, ZoneManager.JokerZone);
                    }
                    targetCard.SetEditionOfficial(Edition.POLYCHROME);
                };
                return ret;
            } },
            {"SOUL", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Soul";
                ret.DBName = "SOUL";
                ret.DescriptionBuilder = _ => "Generates a random Legendary Joker (Must have room).";
                ret.IsActivatable = _ => ZoneManager.JokerZone.HasRoom;
                ret.Use = _ =>
                {
                    //var rareJoker = MarketOptionsManager.PullRandomJokerFromPool(JokerRarity.LEGENDARY, removeFromPool: true);
                    var legendJoker = MarketPullManager.PickMarketCard(BuyItemType.JOKER, Pools.GenerationSource.SoulCard, forcedRarity: JokerRarity.LEGENDARY);
                    if (legendJoker != null)
                    {
                        ZoneManager.JokerZone.AddCard(legendJoker);
                    }
                };
                return ret;
            } },
            {"BLACK HOLE", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Black Hole";
                ret.DBName = "BLACK HOLE";
                ret.DescriptionBuilder = _ => "Level up ALL hands.";
                ret.IsActivatable = _ => true;
                ret.Use = _ =>
                {
                    foreach (var hand in Enum.GetValues(typeof(PlayedHandType)).Cast<PlayedHandType>())
                    {
                        ScoreHandler.LevelUpHand(hand);
                    }
                };
                return ret;
            } },
        };

        public static List<string> TarotNames => TarotConsumableDb.Keys.ToList();
        public static Dictionary<string, Func<Card, ConsumableCardDataBlock>> TarotConsumableDb = new()
        {
            { "FOOL", c => 
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Fool";
                ret.DBName = "FOOL";
                ret.DescriptionBuilder = _ =>
                {
                    var retStr = "Gain a copy of last used Planet or Tarot card (NONE)";
                    var targetC = EngineEventHandler.LastSavedOfTypeConditional(EventContextType.ConsumableUsed, EvaluateCardForFool);
                    if(targetC != null && targetC is EngineConsumableUseArgs conArgs)
                    {
                        retStr = retStr.Replace("NONE", conArgs.ConsumableName);
                    }
                    return retStr;
                };
                ret.IsActivatable = _ => EngineEventHandler.LastSavedOfTypeConditional(EventContextType.ConsumableUsed, EvaluateCardForFool) != null && ZoneManager.ConsumableZone.HasRoomFor(-1);
                ret.Use = _ =>
                {
                    var lastEv = EngineEventHandler.LastSavedOfTypeConditional(EventContextType.ConsumableUsed, EvaluateCardForFool);
                    if(lastEv != null && lastEv is EngineConsumableUseArgs conArg && (conArg.TypeUsed == ConsumableType.TAROT || conArg.TypeUsed == ConsumableType.PLANET))
                    {
                        Card toDraw = new();
                        if(conArg.TypeUsed == ConsumableType.TAROT)
                        {
                            MakeCardTarotCard(conArg.ConsumableDBName, toDraw);
                        }
                        else
                        {
                            MakeCardPlanetCard(conArg.HandOfItemUsed, toDraw);
                        }

                        ZoneManager.ConsumableZone.AddCard(toDraw);
                    }
                };

                return ret;
            } },
            { "HIGHPRIESTESS", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "High Priestess";
                ret.DBName = "HIGHPRIESTESS";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 2});
                ret.DescriptionBuilder = _ => "Add " + ret.DataDict["INTAMOUNT"].IntData + " planet cards. (Must have room)";
                ret.IsActivatable = _ => ZoneManager.ConsumableZone.HasRoom || ZoneManager.ConsumableZone.Cards.Contains(ret.MyCard);
                ret.Use = _ =>
                {
                    MarketPullManager.DrawNumMarketItems(BuyItemType.PLANET_CARD, ret.DataDict["INTAMOUNT"].IntData, ZoneManager.ConsumableZone, source: GenerationSource.HighPriestessCard, batchContext: new ContentRollBatchContext());
                };

                return ret;
            } },
            { "EMPEROR", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Emperor";
                ret.DBName = "EMPEROR";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 2});
                ret.DescriptionBuilder = _ => "Add " + ret.DataDict["INTAMOUNT"].IntData + " tarot cards. (Must have room)";
                ret.IsActivatable = _ => ZoneManager.ConsumableZone.HasRoom || ZoneManager.ConsumableZone.Cards.Contains(ret.MyCard);
                ret.Use = _ =>
                {
                    var context = new ContentRollBatchContext();
                    context.GeneratedIds.Add("EMPEROR");//TODO: Janky hack so it can't make more emperors.
                    MarketPullManager.DrawNumMarketItems(BuyItemType.TAROT_CARD, ret.DataDict["INTAMOUNT"].IntData, ZoneManager.ConsumableZone, source: GenerationSource.EmperorCard, batchContext: context);
                };

                return ret;
            } },
            { "CHARIOT", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Chariot";
                ret.DBName = "CHARIOT";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 1});
                ret.DescriptionBuilder = _ => "Make " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card(s) in hand steel.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetEnhancementOfficial(Enhancement.STEEL);
                    }
                };

                return ret;
            } },
            { "EMPRESS", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Empress";
                ret.DBName = "EMPRESS";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 2});
                ret.DescriptionBuilder = _ => "Make " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card(s) in hand mult cards.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetEnhancementOfficial(Enhancement.MULT);
                    }
                };

                return ret;
            } },
            { "MAGICIAN", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Magician";
                ret.DBName = "MAGICIAN";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 2});
                ret.DescriptionBuilder = _ => "Make " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card(s) in hand lucky cards.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetEnhancementOfficial(Enhancement.LUCKY);
                    }
                };

                return ret;
            } },
            { "HIEROPHANT", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Hierophant";
                ret.DBName = "HIEROPHANT";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 2});
                ret.DescriptionBuilder = _ => "Make " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card(s) in hand bonus cards.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetEnhancementOfficial(Enhancement.BONUSCHIPS);
                    }
                };

                return ret;
            } },
            { "LOVERS", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Lovers";
                ret.DBName = "LOVERS";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 1});
                ret.DescriptionBuilder = _ => "Make " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card(s) in hand wild cards.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetEnhancementOfficial(Enhancement.WILD);
                    }
                };

                return ret;
            } },
            { "JUSTICE", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Justice";
                ret.DBName = "JUSTICE";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 1});
                ret.DescriptionBuilder = _ => "Make " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card(s) in hand glass cards.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetEnhancementOfficial(Enhancement.GLASS);
                    }
                };

                return ret;
            } },
            { "HERMIT", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Hermit";
                ret.DBName = "HERMIT";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 20});
                ret.DescriptionBuilder = _ => "Double your money (max " + ret.DataDict["INTAMOUNT"].IntData + ")";
                ret.IsActivatable = _ => true;
                ret.Use = _ =>
                {
                    var moneyGainAmt = Math.Min(ret.DataDict["INTAMOUNT"].IntData, Globals.Money);
                    Globals.EmitMoneyGain(moneyGainAmt, c);
                };

                return ret;
            } },
            { "WHEEL", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Wheel of Fortune";
                ret.DBName = "WHEEL";
                ret.DataDict.Add("MINCHANCEAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 1});
                ret.DataDict.Add("MAXCHANCEAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 4});
                ret.DescriptionBuilder = _ => ret.DataDict["MINCHANCEAMOUNT"].IntData + " in " + ret.DataDict["MAXCHANCEAMOUNT"].IntData + " chance to make a random Joker Foil, Holographic, or Polychrome.";
                ret.IsActivatable = _ => ZoneManager.JokerZone.Cards.Count > 0 && ZoneManager.JokerZone.Cards.Where(x => x.Edition == Edition.BASE).Any();
                ret.Use = _ =>
                {
                    var doAtAll = Globals.RollRandom(ret.DataDict["MINCHANCEAMOUNT"].IntData, ret.DataDict["MAXCHANCEAMOUNT"].IntData, c);
                    //TODO: if failure emit event, so print can show
                    if (doAtAll)
                    {
                        var editionToGiveRoll = Globals.ChooseRandomInclusive(0, 2);
                        Edition editionToGive = Edition.BASE;
                        switch (editionToGiveRoll)
                        {
                            case 0:
                                editionToGive = Edition.FOIL;
                                break;
                            case 1:
                                editionToGive = Edition.HOLOGRAPHIC;
                                break;
                            case 2:
                                editionToGive = Edition.POLYCHROME;
                                break;
                        }
                        var chosenJokerSeq = ZoneManager.JokerZone.Cards.Where(x => x.Edition == Edition.BASE);
                        var chosenJoker = chosenJokerSeq.ToArray()[Globals.ChooseRandomInclusive(0, chosenJokerSeq.Count() - 1)];
                        chosenJoker.SetEditionOfficial(editionToGive);
                    }
                };

                return ret;
            } },
            { "STRENGTH", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Strength";
                ret.DBName = "STRENGTH";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 2});
                ret.DescriptionBuilder = _ => "Increase the rank of " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card(s) in hand.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        //TODO: what happens if you apply Strength to a card with modified bonus chips a la Hiker? Research.
                        var oldRank = card.Rank;
                        Rank newRank;
                        if(oldRank == Rank.ACE)
                        {
                            newRank = Rank.TWO;
                        }
                        else
                        {
                            newRank = EngineUtils.StandardOrderAceHigh[EngineUtils.StandardOrderAceHigh.IndexOf(oldRank) + 1];
                        }
                        card.SetRankOfficial(newRank);
                    }
                };

                return ret;
            } },
            { "DEVIL", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Devil";
                ret.DBName = "DEVIL";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 1});
                ret.DescriptionBuilder = _ => "Make " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card(s) in hand golden.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetEnhancementOfficial(Enhancement.GOLD);
                    }
                };

                return ret;
            } },
            { "TOWER", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Tower";
                ret.DBName = "TOWER";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 1});
                ret.DescriptionBuilder = _ => "Make " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card(s) in hand stone.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetEnhancementOfficial(Enhancement.STONE);
                    }
                };

                return ret;
            } },
            { "HANGED", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Hanged Man";
                ret.DBName = "HANGED";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 2});
                ret.DescriptionBuilder = _ => "Destroy up to " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card(s) in hand.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    var toDest = ZoneManager.CardsSelectedInHand.ToList();
                    foreach (var card in toDest)
                    {
                        ZoneManager.DestroyCard(card, card.MyZone);
                    }
                };

                return ret;
            } },
            { "DEATH", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Death";
                ret.DBName = "DEATH";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 1});
                ret.DescriptionBuilder = _ => "Transform the left " + ret.DataDict["INTAMOUNT"].IntData + " card(s) into the rightmost card.";
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand >= 2 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData + 1;
                ret.Use = _ =>
                {
                    var selCards = ZoneManager.CardsSelectedInHand.ToList();
                    var baseToCopy = selCards.Last();
                    selCards.Remove(baseToCopy);
                    foreach (var card in selCards)
                    {
                        baseToCopy.TurnIntoCopyOfMe(card);
                    }
                };

                return ret;
            } },
            { "TEMPERANCE", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Temperance";
                ret.DBName = "TEMPERANCE";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 50});
                ret.DescriptionBuilder = _ =>
                {
                    var firstMon = ZoneManager.JokerZone.Cards.Select(x => x.SellCost).Sum();
                    var moneyGainAmt = Math.Min(ret.DataDict["INTAMOUNT"].IntData, firstMon);
                    return "Gain $" + moneyGainAmt + ", the sell value of your jokers (max " + ret.DataDict["INTAMOUNT"].IntData + ").";
                };
                ret.IsActivatable = _ => ZoneManager.JokerZone.Cards.Count > 0;
                ret.Use = _ =>
                {
                    var firstMon = ZoneManager.JokerZone.Cards.Select(x => x.SellCost).Sum();
                    var moneyGainAmt = Math.Min(ret.DataDict["INTAMOUNT"].IntData, firstMon);
                    Globals.EmitMoneyGain(moneyGainAmt, c);
                };

                return ret;
            } },
            { "JUDGEMENT", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Judgement";
                ret.DBName = "JUDGEMENT";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 1});
                ret.DescriptionBuilder = _ =>
                {
                    var cardSect = ret.DataDict["INTAMOUNT"].IntData > 1 ? "" + ret.DataDict["INTAMOUNT"].IntData + " random Jokers" : "a random Joker";
                    return "Gain " + cardSect + ". (Must have room)";
                };
                ret.IsActivatable = _ => ZoneManager.JokerZone.HasRoom;
                ret.Use = _ =>
                {
                    MarketPullManager.DrawNumMarketItems(BuyItemType.JOKER, ret.DataDict["INTAMOUNT"].IntData, ZoneManager.JokerZone, source: Pools.GenerationSource.JudgementCard);
                };

                return ret;
            } },
            { "STARS", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Stars";
                ret.DBName = "STARS";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 3});
                ret.DataDict.Add("SUIT", new JokerData() { MyDataType = JokerDataType.SUIT, SpecificCardSuit = Suit.DIAMONDS});
                ret.DescriptionBuilder = _ =>
                {
                    var possPlural = ret.DataDict["INTAMOUNT"].IntData < 2 ? "" : "s";
                    return "Change the suit of " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card" + possPlural + " to " + ret.DataDict["SUIT"].SpecificCardSuit + ".";
                };
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetSuitOfficial(ret.DataDict["SUIT"].SpecificCardSuit);
	                }
                };

                return ret;
            } },
            { "MOON", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Moon";
                ret.DBName = "MOON";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 3});
                ret.DataDict.Add("SUIT", new JokerData() { MyDataType = JokerDataType.SUIT, SpecificCardSuit = Suit.CLUBS});
                ret.DescriptionBuilder = _ =>
                {
                    var possPlural = ret.DataDict["INTAMOUNT"].IntData < 2 ? "" : "s";
                    return "Change the suit of " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card" + possPlural + " to " + ret.DataDict["SUIT"].SpecificCardSuit + ".";
                };
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetSuitOfficial(ret.DataDict["SUIT"].SpecificCardSuit);
                    }
                };

                return ret;
            } },
            { "SUN", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The Sun";
                ret.DBName = "SUN";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 3});
                ret.DataDict.Add("SUIT", new JokerData() { MyDataType = JokerDataType.SUIT, SpecificCardSuit = Suit.HEARTS});
                ret.DescriptionBuilder = _ =>
                {
                    var possPlural = ret.DataDict["INTAMOUNT"].IntData < 2 ? "" : "s";
                    return "Change the suit of " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card" + possPlural + " to " + ret.DataDict["SUIT"].SpecificCardSuit + ".";
                };
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetSuitOfficial(ret.DataDict["SUIT"].SpecificCardSuit);
                    }
                };

                return ret;
            } },
            { "WORLD", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "The World";
                ret.DBName = "WORLD";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 3});
                ret.DataDict.Add("SUIT", new JokerData() { MyDataType = JokerDataType.SUIT, SpecificCardSuit = Suit.SPADES});
                ret.DescriptionBuilder = _ =>
                {
                    var possPlural = ret.DataDict["INTAMOUNT"].IntData < 2 ? "" : "s";
                    return "Change the suit of " + ret.DataDict["INTAMOUNT"].IntData + " selected playing card" + possPlural + " to " + ret.DataDict["SUIT"].SpecificCardSuit + ".";
                };
                ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
                ret.Use = _ =>
                {
                    foreach (var card in ZoneManager.CardsSelectedInHand)
                    {
                        card.SetSuitOfficial(ret.DataDict["SUIT"].SpecificCardSuit);
                    }
                };

                return ret;
            } },

        };

        //Creating a planet card is a bit simpler, just take its name and hand-type and you're good.
        public static Dictionary<PlayedHandType, string> PlanetCardNames = new()
        {
            {PlayedHandType.HIGHCARD, "Pluto" },
            {PlayedHandType.PAIR, "Mercury" },
            {PlayedHandType.TWOPAIR, "Uranus" },
            {PlayedHandType.THREEOFAKIND, "Venus" },
            {PlayedHandType.FOUROFAKIND, "Mars" },
            {PlayedHandType.FLUSH, "Jupiter" },
            {PlayedHandType.FULLHOUSE, "Earth" },
            {PlayedHandType.STRAIGHT, "Saturn" },
            {PlayedHandType.STRAIGHTFLUSH, "Neptune" },
            {PlayedHandType.FIVEOFAKIND, "Planet X" },
            {PlayedHandType.FLUSHFIVE, "Eris" },
            {PlayedHandType.FLUSHHOUSE, "Ceres" },
        };

        public static Dictionary<string, PlayedHandType> PlanetsToHandType => PlanetCardNames.ToDictionary(pair => pair.Value.ToUpper(), pair => pair.Key);

        public static void UseConsumable(Card c)
        {
            UseConsumable(c, c.MyZone);
        }

        //Use a consumable card, given the card and the zone it's from.
        //TODO: shouldn't the cards MyZone field suffice?
        //I think it's cause I'm not consistently setting MyZone well. Def should fix that. TODO.
        public static void UseConsumable(Card c, CardZone zoneFrom)
        {
            if(!(ZoneManager.ConsumableZone.Cards.Contains(c) || ZoneManager.PackOptionZone.Cards.Contains(c) || ZoneManager.MainMarketZone.Cards.Contains(c)))
            {
                //ERROR: card not in consumable, pack options, OR main market zone (buy and use).
                //TODO: Maybe remove this? Like the modded thing where playing cards can be turned into consumables.
                return;
            }
            ZoneManager.CurrentlyActivatingConsumable.DrawTargetFrom(zoneFrom, c);
            if (!c.isConsumable)
            {
                //ERROR: card not a consumable.
                return;
            }
            //TODO: Any args to pass here?
            if (!c.ConsumableData.IsActivatable(null))
            {
                //ERROR: Not activatable;
                return;
            }

            //TODO: Again, args to pass?
            c.ConsumableData.Use(null);

            var args = new EngineConsumableUseArgs
            {
                MyContext = new EventContext() { Context = EventContextType.ConsumableUsed },
                ConsumableName = c.ConsumableData.ConsumableName,
                ConsumableDBName = c.ConsumableData.DBName,
                HandOfItemUsed = c.ConsumableData.PlanetHandType,
                TypeUsed = c.ConsumableData.Type,
                BuyItemUsed = c.ConsumableData.BuyType
            };
            EngineEventHandler.TriggerEvent(args);

            MarketOptionsManager.ReturnMarketItemFromZone(c, ZoneManager.CurrentlyActivatingConsumable);
        }

        public static Card MakeTarotCard(string TarotCardDbName)
        {
            if (!TarotConsumableDb.ContainsKey(TarotCardDbName))
            {
                return null;
            }
            var c = new Card();
            MakeCardTarotCard(TarotCardDbName, c);
            return c;
        }

        public static Card MakeSpectralCard(string SpectralCardDbName)
        {
            if (!SpectralNames.Contains(SpectralCardDbName))
            {
                return null;
            }
            var c = new Card();
            MakeCardSpectralCard(SpectralCardDbName, c);
            return c;
        }

        //Give the passed card the data necessary to transform it into the named tarot card (DB NAME)
        public static void MakeCardTarotCard(string TarotCardDbName, Card target)
        {
            if (!TarotConsumableDb.ContainsKey(TarotCardDbName))
            {
                return;
            }
            var retDataBlock = TarotConsumableDb[TarotCardDbName](target);
            retDataBlock.Type = ConsumableType.TAROT;
            retDataBlock.BuyType = BuyItemType.TAROT_CARD;
            retDataBlock.MyCard = target;
            target.ConsumableData = retDataBlock;
            target.BaseCost = 3;//default tarot cost.
        }

        //Give the passed card the data necessary to transform it into the named spectral card (DB NAME)
        public static void MakeCardSpectralCard(string SpectralCardDbName, Card target)
        {
            if (!SpectralConsumablesDb.ContainsKey(SpectralCardDbName))
            {
                return;
            }
            var retDataBlock = SpectralConsumablesDb[SpectralCardDbName](target);
            retDataBlock.Type = ConsumableType.SPECTRAL;
            retDataBlock.BuyType = BuyItemType.SPECTRAL_CARD;
            retDataBlock.MyCard = target;
            target.ConsumableData = retDataBlock;
            target.BaseCost = 4;//default spectral cost.
        }

        //Generate and return a card that is a planet card for the passed hand type.
        public static Card MakePlanetCard(PlayedHandType hand)
        {
            var ret = new Card();
            MakeCardPlanetCard(hand, ret);
            return ret;
        }

        public static Card MakePlanetCard(string planetName)
        {
            var handType = PlanetsToHandType[planetName];
            return MakePlanetCard(handType);
        }

        //Generate and return a card that is a pack of the passed type
        public static Card MakePack(PackType packType)
        {
            var ret = new Card
            {
                MyPackType = packType
            };
            ret.BaseCost = PackBasicNums[ret.MyPackType].BasePackPrice;
            return ret;
        }

        public static Card MakePackByOdds()
        {
            PackType chosenPackType = PackType.BASIC_JOKER;
            var roll = Globals.randomNext(PackTotalOdds);
            foreach (var kv in PackBasicNums)
            {
                if(roll < kv.Value.ChanceToAppear)
                {
                    chosenPackType = kv.Key;
                    break;
                }
                else
                {
                    roll -= kv.Value.ChanceToAppear;
                }
            }
            return MakePack(chosenPackType);
        }

        //Give the passed card the data necessary to transform it into planet card for the passed hand type.
        public static void MakeCardPlanetCard(PlayedHandType hand, Card target)
        {
            var dataBlock = new ConsumableCardDataBlock
            {
                ConsumableName = PlanetCardNames[hand],
                DBName = PlanetCardNames[hand].ToUpper(),
                Type = ConsumableType.PLANET,
                BuyType = BuyItemType.PLANET_CARD,
                PlanetHandType = hand,
            };
            dataBlock.DataDict.Add("HANDTYPE", new Jokers.JokerData() { MyDataType = Jokers.JokerDataType.HANDTYPE, HandTypeData = hand });
            dataBlock.DescriptionBuilder = _ => "Level up hand " + dataBlock.DataDict["HANDTYPE"].HandTypeData.ToString();
            dataBlock.Use = _ =>
            {
                ScoreHandler.LevelUpHand(dataBlock.DataDict["HANDTYPE"].HandTypeData);
            };
            dataBlock.MyCard = target;
            target.ConsumableData = dataBlock;
            target.BaseCost = 3; //default planet card cost.
        }

        #region Helpers

        private static bool EvaluateCardForFool(EngineEventArgs args) => args is EngineConsumableUseArgs conArgs && (conArgs.TypeUsed == ConsumableType.TAROT || conArgs.TypeUsed == ConsumableType.PLANET);

        private static ConsumableCardDataBlock BuildSealSpectral(string conName, Seal sealType, Card c)
        {
            var ret = new ConsumableCardDataBlock();
            ret.ConsumableName = conName;
            ret.DBName = conName.ToUpper();

            ret.DataDict.Add("INTAMOUNT", new() { IntData = 1, MyDataType = JokerDataType.INT });
            ret.DescriptionBuilder = _ => "Add a " + SealString[sealType] + " to " + ret.DataDict["INTAMOUNT"].IntData + " selected card(s) in hand.";

            ret.IsActivatable = _ => EngineUtils.NumCardsSelectedInHand > 0 && EngineUtils.NumCardsSelectedInHand <= ret.DataDict["INTAMOUNT"].IntData;
            ret.Use = _ =>
            {
                foreach (var card in ZoneManager.CardsSelectedInHand)
                {
                    card.SetSealOfficial(sealType);
                }
            };

            return ret;
        }

        private static ConsumableCardDataBlock BuildCardGenSpectral(string conName, string rankGroupToPull, int numberToMake, Card c)
        {
            var ret = new ConsumableCardDataBlock();
            ret.ConsumableName = conName;
            ret.DBName = conName.ToUpper();
            ret.DataDict.Add("INTAMOUNT", new() { MyDataType = JokerDataType.INT, IntData = numberToMake });
            ret.DataDict.Add("DESTROYAMOUNT", new() { MyDataType = JokerDataType.INT, IntData = 1 });
            ret.DescriptionBuilder = _ => "Destroy " + ret.DataDict["DESTROYAMOUNT"].IntData + " random card in your hand, but add " + ret.DataDict["INTAMOUNT"].IntData + " random " + rankGroupToPull + " instead.";
            ret.IsActivatable = _ => ZoneManager.HandZone.Cards.Count > 0;
            ret.Use = _ =>
            {
                for (int i = 0; i < ret.DataDict["DESTROYAMOUNT"].IntData; i++)
                {
                    var toRem = ZoneManager.HandZone.Cards[Globals.randomNext(ZoneManager.HandZone.Cards.Count)];
                    ZoneManager.DestroyCard(toRem, ZoneManager.HandZone);
                }
                var toAdd = new List<Card>();
                var validRanks = EngineUtils.RankGroups[rankGroupToPull];
                var validEnhancements = Enum.GetValues(typeof(Enhancement)).Cast<Enhancement>().Where(r => r != Enhancement.NONE && r != Enhancement.STONE).ToList();
                for (int i = 0; i < ret.DataDict["INTAMOUNT"].IntData; i++)
                {
                    var newCard = new Card();
                    EngineUtils.RandomizePlayingCard(newCard, validRanks: validRanks);
                    newCard.Enhancement = validEnhancements[Globals.randomNext(validEnhancements.Count)];
                    toAdd.Add(newCard);
                }
                ZoneManager.HandZone.AddCards(toAdd, overrideSpace: true);
            };

            return ret;
        }

        #endregion
    }
}
