using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Jokers
{
    public static class JokerDb
    {
        public static string DEFAULT_JOKER_NAME = "JOLLY JOKER";

        //Costs of jokers... should prob just include in the db.
        //Or maybe better to separate this stuff out, idk.
        public static Dictionary<string, JokerTypeData> JokerMetadata = new()
        {
            {"JIMBO", new JokerTypeData { DBName = "JIMBO", Price = 2 } },
            {"GREEDY JOKER", new JokerTypeData { DBName = "GREEDY JOKER", Price = 5 } },
            {"LUSTY JOKER", new JokerTypeData { DBName = "LUSTY JOKER", Price = 5 } },
            {"WRATHFUL JOKER", new JokerTypeData { DBName = "WRATHFUL JOKER", Price = 5 } },
            {"GLUTTONOUS JOKER", new JokerTypeData { DBName = "GLUTTONOUS JOKER", Price = 5 } },
            {"JOLLY JOKER", new JokerTypeData { DBName = "JOLLY JOKER", Price = 3 } },
            {"ZANY JOKER", new JokerTypeData { DBName = "ZANY JOKER", Price = 4 } },
            {"MAD JOKER", new JokerTypeData { DBName = "MAD JOKER", Price = 4 } },
            {"CRAZY JOKER", new JokerTypeData { DBName = "CRAZY JOKER", Price = 4 } },
            {"DROLL JOKER", new JokerTypeData { DBName = "DROLL JOKER", Price = 4 } },
            {"SLY JOKER", new JokerTypeData { DBName = "SLY JOKER", Price = 3 } },
            {"WILY JOKER", new JokerTypeData { DBName = "WILY JOKER", Price = 4 } },
            {"CLEVER JOKER", new JokerTypeData { DBName = "CLEVER JOKER", Price = 4 } },
            {"DEVIOUS JOKER", new JokerTypeData { DBName = "DEVIOUS JOKER", Price = 4 } },
            {"CRAFTY JOKER", new JokerTypeData { DBName = "CRAFTY JOKER", Price = 4 } },
            {"HALF JOKER", new JokerTypeData { DBName = "HALF JOKER", Price = 5 } },
            {"STENCIL JOKER", new JokerTypeData { DBName = "STENCIL JOKER", Price = 8, Rarity = JokerRarity.UNCOMMON } },
            {"FOUR FINGERS", new JokerTypeData { DBName = "FOUR FINGERS", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"MIME", new JokerTypeData { DBName = "MIME", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"CREDIT CARD", new JokerTypeData { DBName = "CREDIT CARD", Price = 1 } },
            {"GOLDEN JOKER", new JokerTypeData { DBName = "GOLDEN JOKER", Price = 5 } },
            {"CEREMONIAL DAGGER", new JokerTypeData { DBName = "CEREMONIAL DAGGER", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"MYSTIC SUMMIT", new JokerTypeData { DBName = "MYSTIC SUMMIT", Price = 5 } },
            {"MARBLE JOKER", new JokerTypeData { DBName = "MARBLE JOKER", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"LOYALTY CARD", new JokerTypeData { DBName = "LOYALTY CARD", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"8 BALL", new JokerTypeData { DBName = "8 BALL", Price = 5 } },
            {"MISPRINT", new JokerTypeData { DBName = "MISPRINT", Price = 4 } },
            {"DUSK", new JokerTypeData { DBName = "DUSK", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"RAISED FIST", new JokerTypeData { DBName = "RAISED FIST", Price = 5 } },
            {"CHAOS THE CLOWN", new JokerTypeData { DBName = "CHAOS THE CLOWN", Price = 4 } },
            {"FIBONACCI", new JokerTypeData { DBName = "FIBONACCI", Price = 8, Rarity = JokerRarity.UNCOMMON } },
            {"STEEL JOKER", new JokerTypeData { DBName = "STEEL JOKER", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"SCARY FACE", new JokerTypeData { DBName = "SCARY FACE", Price = 4 } },
            {"ABSTRACT JOKER", new JokerTypeData { DBName = "ABSTRACT JOKER", Price = 4 } },
            {"DELAYED GRATIFICATION", new JokerTypeData { DBName = "DELAYED GRATIFICATION", Price = 4 } },
            {"HACK", new JokerTypeData { DBName = "HACK", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"PAREIDOLIA", new JokerTypeData { DBName = "PAREIDOLIA", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"GROS MICHEL", new JokerTypeData { DBName = "GROS MICHEL", Price = 5 } },
            {"EVEN STEVEN", new JokerTypeData { DBName = "EVEN STEVEN", Price = 4 } },
            {"ODD TODD", new JokerTypeData { DBName = "ODD TODD", Price = 4 } },
            {"SCHOLAR", new JokerTypeData { DBName = "SCHOLAR", Price = 4 } },
            {"BUSINESS CARD", new JokerTypeData { DBName = "BUSINESS CARD", Price = 4 } },
            {"SUPERNOVA", new JokerTypeData { DBName = "SUPERNOVA", Price = 5 } },
            {"RIDE THE BUS", new JokerTypeData { DBName = "RIDE THE BUS", Price = 6 } },
            {"SPACE JOKER", new JokerTypeData { DBName = "SPACE JOKER", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"EGG", new JokerTypeData { DBName = "EGG", Price = 4 } },
            {"BURGLAR", new JokerTypeData { DBName = "BURGLAR", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"BLACKBOARD", new JokerTypeData { DBName = "BLACKBOARD", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"RUNNER", new JokerTypeData { DBName = "RUNNER", Price = 5 } },
            {"ICE CREAM", new JokerTypeData { DBName = "ICE CREAM", Price = 5 } },
            {"DNA", new JokerTypeData { DBName = "DNA", Price = 8, Rarity = JokerRarity.RARE } },
            {"SPLASH", new JokerTypeData { DBName = "SPLASH", Price = 3 } },
            {"BLUE JOKER", new JokerTypeData { DBName = "BLUE JOKER", Price = 5 } },
            {"SIXTH SENSE", new JokerTypeData { DBName = "SIXTH SENSE", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"CONSTELLATION", new JokerTypeData { DBName = "CONSTELLATION", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"HIKER", new JokerTypeData { DBName = "HIKER", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"FACELESS JOKER", new JokerTypeData { DBName = "FACELESS JOKER", Price = 4 } },
            {"GREEN JOKER", new JokerTypeData { DBName = "GREEN JOKER", Price = 4 } },
            {"SUPERPOSITION", new JokerTypeData { DBName = "SUPERPOSITION", Price = 4 } },
            {"TO DO LIST", new JokerTypeData { DBName = "TO DO LIST", Price = 4 } },
            {"CAVENDISH", new JokerTypeData { DBName = "CAVENDISH", Price = 4 } },
            {"TEMP UNCOMMON JOKER", new JokerTypeData { DBName = "TEMP UNCOMMON JOKER", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"TEMP RARE JOKER", new JokerTypeData { DBName = "TEMP RARE JOKER", Price = 5, Rarity = JokerRarity.RARE } },
            {"TEMP LEGENDARY JOKER", new JokerTypeData { DBName = "TEMP LEGENDARY JOKER", Price = 6, Rarity = JokerRarity.LEGENDARY } },
        };

        public static List<string> JokerDbNames => JokerData.Keys.ToList();

        //Define our jokers by functions that build them out of a passed card.
        public static Dictionary<string, Func<Card, JokerCardDataBlock>> JokerData = new()
        {
            { "JIMBO", c =>
            {
                var ret = BasicDataBlock("Jimbo");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 4, MyDataType = JokerDataType.DOUBLE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger)
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });

                return ret;
            } },
            { "GREEDY JOKER", c =>
            {
                var ret = BasicDataBlock("Greedy Joker");
                ret.DescriptionBuilder = _ => "Played cards with " + ret.DataDict["SUIT"].SpecificCardSuit + " Suit give +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when scored";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("SUIT", new JokerData() { SpecificCardSuit = Suit.DIAMONDS, MyDataType = JokerDataType.SUIT});
                ret.Listeners.Add(BuildMultAddListener(c, ret));

                return ret;
            } },
            { "LUSTY JOKER", c =>
            {
                var ret = BasicDataBlock("Lusty Joker");
                ret.DescriptionBuilder = _ => "Played cards with " + ret.DataDict["SUIT"].SpecificCardSuit + " Suit give +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when scored";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("SUIT", new JokerData() { SpecificCardSuit = Suit.HEARTS, MyDataType = JokerDataType.SUIT});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering.IsSuit(ret.DataDict["SUIT"].SpecificCardSuit) && triggerArgs.isScoringTrigger)
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });

                return ret;
            } },
            { "WRATHFUL JOKER", c =>
            {
                var ret = BasicDataBlock("Wrathful Joker");
                ret.DescriptionBuilder = _ => "Played cards with " + ret.DataDict["SUIT"].SpecificCardSuit + " Suit give +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when scored";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("SUIT", new JokerData() { SpecificCardSuit = Suit.SPADES, MyDataType = JokerDataType.SUIT});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering.IsSuit(ret.DataDict["SUIT"].SpecificCardSuit) && triggerArgs.isScoringTrigger)
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });

                return ret;
            } },
            { "GLUTTONOUS JOKER", c =>
            {
                var ret = BasicDataBlock("Gluttonous Joker");
                ret.DescriptionBuilder = _ => "Played cards with " + ret.DataDict["SUIT"].SpecificCardSuit + " Suit give +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when scored";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("SUIT", new JokerData() { SpecificCardSuit = Suit.CLUBS, MyDataType = JokerDataType.SUIT});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering.IsSuit(ret.DataDict["SUIT"].SpecificCardSuit) && triggerArgs.isScoringTrigger)
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });

                return ret;
            } },
            { "JOLLY JOKER", c =>
            {
                var ret = BasicDataBlock("Jolly Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult if Played Hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 8, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = PlayedHandType.PAIR, MyDataType = JokerDataType.HANDTYPE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });

                return ret;
            } },
            { "ZANY JOKER", c =>
            {
                var ret = BasicDataBlock("Zany Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult if Played Hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 12, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = PlayedHandType.THREEOFAKIND, MyDataType = JokerDataType.HANDTYPE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });

                return ret;
            } },
            { "MAD JOKER", c =>
            {
                var ret = BasicDataBlock("Mad Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult if Played Hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 10, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = PlayedHandType.TWOPAIR, MyDataType = JokerDataType.HANDTYPE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });

                return ret;
            } },
            { "CRAZY JOKER", c =>
            {
                var ret = BasicDataBlock("Crazy Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult if Played Hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 12, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = PlayedHandType.STRAIGHT, MyDataType = JokerDataType.HANDTYPE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });

                return ret;
            } },
            { "DROLL JOKER", c =>
            {
                var ret = BasicDataBlock("Droll Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult if Played Hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 10, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = PlayedHandType.FLUSH, MyDataType = JokerDataType.HANDTYPE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });

                return ret;
            } },
            { "SLY JOKER", c =>
            {
                var ret = BasicDataBlock("Sly Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["CHIPAMOUNT"].IntData + " Chips if Played Hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 50, MyDataType = JokerDataType.INT});
                ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = PlayedHandType.PAIR, MyDataType = JokerDataType.HANDTYPE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                    },
                });

                return ret;
            } },
            { "WILY JOKER", c =>
            {
                var ret = BasicDataBlock("Wily Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["CHIPAMOUNT"].IntData + " Chips if Played Hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 100, MyDataType = JokerDataType.INT});
                ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = PlayedHandType.THREEOFAKIND, MyDataType = JokerDataType.HANDTYPE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                    },
                });

                return ret;
            } },
            { "CLEVER JOKER", c =>
            {
                var ret = BasicDataBlock("Clever Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["CHIPAMOUNT"].IntData + " Chips if Played Hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 80, MyDataType = JokerDataType.INT});
                ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = PlayedHandType.TWOPAIR, MyDataType = JokerDataType.HANDTYPE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                    },
                });

                return ret;
            } },
            { "DEVIOUS JOKER", c =>
            {
                var ret = BasicDataBlock("Devious Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["CHIPAMOUNT"].IntData + " Chips if Played Hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 100, MyDataType = JokerDataType.INT});
                ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = PlayedHandType.STRAIGHT, MyDataType = JokerDataType.HANDTYPE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                    },
                });

                return ret;
            } },
            { "CRAFTY JOKER", c =>
            {
                var ret = BasicDataBlock("Crafty Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["CHIPAMOUNT"].IntData + " Chips if Played Hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 80, MyDataType = JokerDataType.INT});
                ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = PlayedHandType.FLUSH, MyDataType = JokerDataType.HANDTYPE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                    },
                });

                return ret;
            } },
            { "HALF JOKER", c =>
            {
                var ret = BasicDataBlock("Half Joker");
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult if Played Hand contains " + ret.DataDict["INTAMOUNT"].HandTypeData.ToString() + " or fewer cards.";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 20, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("INTAMOUNT", new JokerData() { IntData = 3, MyDataType = JokerDataType.INT});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && triggerArgs.MyContext.ScoringContext.AllPlayingCardsSubmittedForHand.Count <= ret.DataDict["INTAMOUNT"].IntData)
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].IntData, c);
                    },
                });

                return ret;
            } },
            { "STENCIL JOKER", c =>
            {
                var ret = BasicDataBlock("Stencil Joker");
                Func<double> getCurrentAmt = () => (ZoneManager.JokerZone.MaxCapacity - ZoneManager.JokerZone.Cards.Count(x => x.isJoker && x.JokerData.DBName != "STENCIL JOKER")) * ret.DataDict["MULTMULTAMOUNT"].DoubleData;
                ret.DescriptionBuilder = _ => "* " + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult for each empty Joker slot, Stencil Joker included. Currently * " + getCurrentAmt().ToString() + " Mult";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger)
                            Globals.EmitMultMult(getCurrentAmt(), c);
                    },
                });

                return ret;
            } },
            { "FOUR FINGERS", c =>
            {
                var ret = BasicDataBlock("Four Fingers");
                ret.DescriptionBuilder = _ => "All Flushes and Straights can be made with only " + ret.DataDict["INTAMOUNT"].IntData + " cards.";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { IntData = 4, MyDataType = JokerDataType.INT});
                ret.DataDict.Add("OLDFLUSH", new JokerData() { IntData = 5, MyDataType = JokerDataType.INT});
                ret.DataDict.Add("OLDSTRAIGHT", new JokerData() { IntData = 5, MyDataType = JokerDataType.INT});
                //NOTE: If other cards also affect the length of flushes, saving the "old length" could cause problems. For example, losing the first of two FOUR FINGERS could reset to 5 even though you have another.
                //Then the other resets to 4 when you get rid of it, even though you no longer have FOUR FINGERS.
                //But not saving it causes issues too. Then getting rid of one at any time resets it to 5, even if you have another. Idk on this one.

                //Fix would be to make it a listener I guess. Listen for straight/flush check events that say it isn't one, check to see if it is one under 4-card conditions, if so overwrite.

                //TEMPORARY FIX: old val is hard-set to 5. Check for other FOUR FINGERS on removal; if none, reset to old val.
                //Sucks, but works, so long as no other cards affect flush/straight len.
                ret.OnJokerGainEffs.Add(() =>
                {
                    EngineUtils.LenFlush = ret.DataDict["INTAMOUNT"].IntData;
                    EngineUtils.LenStraight = ret.DataDict["INTAMOUNT"].IntData;
                });

                ret.OnJokerRemovalEffs.Add(() =>
                {
                    if(!ZoneManager.JokerZone.Cards.Any(x => x.isJoker && x.JokerData.DBName == "FOUR FINGERS"))
                    {
                        EngineUtils.LenFlush = ret.DataDict["OLDFLUSH"].IntData;
                        EngineUtils.LenStraight = ret.DataDict["OLDSTRAIGHT"].IntData;
                    }
                });

                return ret;
            } },
            { "MIME", c =>
            {
                var ret = BasicDataBlock("Mime");
                ret.DescriptionBuilder = _ => "Your card in hand effects trigger " + ret.DataDict["INTAMOUNT"].IntData + " times.";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardPreTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardPreTriggerArgs triggerArgs && triggerArgs.isInHandPostScoringTrigger)
                            triggerArgs.numTriggersToDo += ret.DataDict["INTAMOUNT"].IntData - 1;
                    },
                });

                return ret;
            } },
            { "CREDIT CARD", c =>
            {
                var ret = BasicDataBlock("Credit Card");
                ret.DescriptionBuilder = _ => "Can go into up to " + ret.DataDict["INTAMOUNT"].IntData + " in debt.";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { IntData = -20, MyDataType = JokerDataType.INT});
                ret.DataDict.Add("OLDMONEYAMOUNT", new JokerData() { IntData = 0, MyDataType = JokerDataType.INT});
                //See Four Fingers for issue with this approach.
                ret.OnJokerGainEffs.Add(() =>
                {
                    Globals.MinimumMoneyAllowed = ret.DataDict["INTAMOUNT"].IntData;
                });

                ret.OnJokerRemovalEffs.Add(() =>
                {
                    if(!ZoneManager.JokerZone.Cards.Where(x => x.isJoker && x.JokerData.DBName == "CREDIT CARD").Any())
                    {
                        Globals.MinimumMoneyAllowed = ret.DataDict["OLDMONEYAMOUNT"].IntData;
                    }
                });

                return ret;
            } },
            { "GOLDEN JOKER", c =>
            {
                var ret = BasicDataBlock("Golden Joker");
                ret.DescriptionBuilder = _ => "Gain " + ret.DataDict["INTAMOUNT"].IntData + "$ at end of round.";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { IntData = 4, MyDataType = JokerDataType.INT});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.GatherPostRoundMoney,
                    MyAction = args =>
                    {
                        if(args is EngineGatherPostRoundMoneyArgs roundMoney)
                            roundMoney.JokersContributed.Add((ret, ret.DataDict["INTAMOUNT"].IntData));
                    },
                });

                return ret;
            } },
            { "CEREMONIAL DAGGER", c =>
            {
                var ret = BasicDataBlock("Ceremonial Dagger");
                ret.DescriptionBuilder = _ => "When Blind is selected, destroy Joker to the right and permanently add double its sell value to this Mult (Currently +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult)";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 0, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.EndBlindSelection,
                    MyAction = args =>
                    {
                        var jokers = ZoneManager.JokerZone.Cards.Where(x => x.isJoker).ToList();
                        var myInd = jokers.IndexOf(c);
                        if (myInd >= 0 && myInd < jokers.Count - 1)
                        {
                            var toDestroy = jokers[myInd + 1];
                            ret.DataDict["MULTAMOUNT"].DoubleData += toDestroy.SellCost * 2;
                            ZoneManager.DestroyCard(toDestroy, ZoneManager.JokerZone);
                        }
                    },
                });
                ret.Listeners.Add(BuildMultAddListener(c, ret));

                return ret;
            } },
            { "MYSTIC SUMMIT", c =>
            {
                var ret = BasicDataBlock("Mystic Summit");
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when 0 discards remaining";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 15, MyDataType = JokerDataType.DOUBLE });
                var extraDisc = () => Globals.CurDiscardsRemaining == 0;
                ret.Listeners.Add(BuildMultAddListener(c, ret, extraDisc));
                return ret;
            } },
            { "MARBLE JOKER", c =>
            {
                var ret = BasicDataBlock("Marble Joker", "Adds one Stone card to deck when Blind is selected");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.EndBlindSelection,
                    MyAction = args =>
                    {
                        var newCard = CardFactory.PlayingCardFromRankSuit((Rank)Random.Shared.Next(1, 14), (Suit)Random.Shared.Next(0, 4));
                        newCard.SetEnhancementOfficial(Enhancement.STONE);
                        ZoneManager.DeckZone.AddCard(newCard);
                    },
                });
                return ret;
            } },
            { "LOYALTY CARD", c =>
            {
                var ret = BasicDataBlock("Loyalty Card");
                ret.DescriptionBuilder = _ => "X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult every " + ret.DataDict["HANDCOUNT"].IntData + " hands played (" + (ret.DataDict["REMAINING"].IntData - 1).ToString() + " remaining)";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 4, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("HANDCOUNT", new JokerData() { IntData = 6, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("REMAINING", new JokerData() { IntData = 6, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayDone,
                    MyAction = args =>
                    {
                        ret.DataDict["REMAINING"].IntData -= 1;
                        if (ret.DataDict["REMAINING"].IntData <= 0)
                            ret.DataDict["REMAINING"].IntData = ret.DataDict["HANDCOUNT"].IntData;
                    },
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && ret.DataDict["REMAINING"].IntData == 1)
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c);
                    },
                });
                return ret;
            } },
            { "8 BALL", c =>
            {
                var ret = BasicDataBlock("8 Ball");
                ret.DescriptionBuilder = _ => ret.DataDict["NUMERATOR"].IntData + " in " + ret.DataDict["DENOMINATOR"].IntData + " chance for each played 8 to create a Tarot card when scored (Must have room)";
                ret.DataDict.Add("NUMERATOR", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("DENOMINATOR", new JokerData() { IntData = 4, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger && triggerArgs.CardThatIsTriggering.Rank == Rank.EIGHT && ZoneManager.ConsumableZone.HasRoom && Globals.RollRandom(ret.DataDict["NUMERATOR"].IntData, ret.DataDict["DENOMINATOR"].IntData, c))
                        {
                            var tarotType = ConsumableManager.TarotNames[Random.Shared.Next(ConsumableManager.TarotNames.Count)];
                            ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeTarotCard(tarotType));
                        }
                    },
                });
                return ret;
            } },
            { "MISPRINT", c =>
            {
                var ret = BasicDataBlock("Misprint");
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 0, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger)
                        {
                            ret.DataDict["MULTAMOUNT"].DoubleData = Random.Shared.Next(24);
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                        }
                    },
                });
                return ret;
            } },
            { "DUSK", c =>
            {
                var ret = BasicDataBlock("Dusk", "Retrigger all played cards in final hand of round");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardPreTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardPreTriggerArgs triggerArgs && Globals.CurHandsRemaining == 1 && !triggerArgs.isInHandPostScoringTrigger && ZoneManager.CurrentlyBeingPlayedZone.Cards.Contains(triggerArgs.CardAboutToTrigger))
                            triggerArgs.numTriggersToDo += 1;
                    },
                });
                return ret;
            } },
            { "RAISED FIST", c =>
            {
                var ret = BasicDataBlock("Raised Fist", "Adds double the rank of lowest ranked card held in hand to Mult");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger)
                        {
                            var heldCards = ZoneManager.HandZone.Cards.Where(x => !x.isSelected).ToList();
                            if (!heldCards.Any())
                                return;
                            var lowestHeld = heldCards.OrderBy(x => EngineUtils.RankBaseChipAmounts[x.Rank]).First();
                            Globals.EmitMultAdd(EngineUtils.RankBaseChipAmounts[lowestHeld.Rank] * 2, c);
                        }
                    },
                });
                return ret;
            } },
            { "CHAOS THE CLOWN", c =>
            {
                var ret = BasicDataBlock("Chaos the Clown", "1 free Reroll per shop");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.MarketSetupDone,
                    MyAction = args =>
                    {
                        Globals.ChaosClownFreeRerollAvailable = true;
                        Globals.CurrentRerollCost = 0;
                    },
                });
                return ret;
            } },
            { "FIBONACCI", c =>
            {
                var ret = BasicDataBlock("Fibonacci");
                ret.DescriptionBuilder = _ => "Each played Ace, 2, 3, 5, or 8 gives +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when scored";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 8, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs
                            && triggerArgs.isScoringTrigger
                            && new List<Rank> { Rank.ACE, Rank.TWO, Rank.THREE, Rank.FIVE, Rank.EIGHT }.Contains(triggerArgs.CardThatIsTriggering.Rank))
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });
                return ret;
            } },
            { "STEEL JOKER", c =>
            {
                Func<double> GetSteelMult = () => 1 + (ZoneManager.GetFullDeckPlayingCards().Count(x => x.Enhancement == Enhancement.STEEL) * 0.2);
                var ret = BasicDataBlock("Steel Joker", _ => "Gives X0.2 Mult for each Steel Card in your full deck (Currently X" + GetSteelMult().ToString("0.0") + " Mult)");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger)
                        {
                            var steelCount = ZoneManager.GetFullDeckPlayingCards().Count(x => x.Enhancement == Enhancement.STEEL);
                            Globals.EmitMultMult(GetSteelMult(), c);
                        }
                    },
                });
                return ret;
            } },
            { "SCARY FACE", c =>
            {
                var ret = BasicDataBlock("Scary Face");
                ret.DescriptionBuilder = _ => "Played face cards give +" + ret.DataDict["CHIPAMOUNT"].IntData + " Chips when scored";
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 30, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger && EngineUtils.isFace(triggerArgs.CardThatIsTriggering))
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                    },
                });
                return ret;
            } },
            { "ABSTRACT JOKER", c =>
            {
                var ret = BasicDataBlock("Abstract Joker");
                Func<double> getMult = () => ZoneManager.JokerZone.Cards.Count(x => x.isJoker) * ret.DataDict["MULTAMOUNT"].DoubleData;
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult for each Joker card (Currently +" + getMult() + " Mult)";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger)
                            Globals.EmitMultAdd(getMult(), c);
                    },
                });
                return ret;
            } },
            { "DELAYED GRATIFICATION", c =>
            {
                var ret = BasicDataBlock("Delayed Gratification");
                ret.DescriptionBuilder = _ => "Earn $" + ret.DataDict["MONEYAMOUNT"].IntData + " per discard if no discards are used by end of round";
                ret.DataDict.Add("MONEYAMOUNT", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("DISCARDS_USED", new JokerData() { IntData = 0, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("DISCARDS_START", new JokerData() { IntData = 0, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.StartPlayRound,
                    MyAction = args =>
                    {
                        ret.DataDict["DISCARDS_USED"].IntData = 0;
                        ret.DataDict["DISCARDS_START"].IntData = Globals.CurDiscardsRemaining;
                    },
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandDiscardDone,
                    MyAction = _ => ret.DataDict["DISCARDS_USED"].IntData += 1,
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.GatherPostRoundMoney,
                    MyAction = args =>
                    {
                        if (args is EngineGatherPostRoundMoneyArgs roundMoney && ret.DataDict["DISCARDS_USED"].IntData == 0)
                        {
                            var moneyToAdd = ret.DataDict["DISCARDS_START"].IntData * ret.DataDict["MONEYAMOUNT"].IntData;
                            if (moneyToAdd > 0)
                                roundMoney.JokersContributed.Add((ret, moneyToAdd));
                        }
                    },
                });
                return ret;
            } },
            { "HACK", c =>
            {
                var ret = BasicDataBlock("Hack", "Retrigger each played 2, 3, 4, or 5");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardPreTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardPreTriggerArgs triggerArgs
                            && !triggerArgs.isInHandPostScoringTrigger
                            && ZoneManager.CurrentlyBeingPlayedZone.Cards.Contains(triggerArgs.CardAboutToTrigger)
                            && new List<Rank> { Rank.TWO, Rank.THREE, Rank.FOUR, Rank.FIVE }.Contains(triggerArgs.CardAboutToTrigger.Rank))
                            triggerArgs.numTriggersToDo += 1;
                    },
                });
                return ret;
            } },
            { "PAREIDOLIA", c =>
            {
                var ret = BasicDataBlock("Pareidolia", "All cards are considered face cards.");
                ret.OnJokerGainEffs.Add(() =>
                {
                    EngineUtils.RankGroups["FACE"] = Enum.GetValues<Rank>().Where(x => x != Rank.NONE).ToList();
                });
                ret.OnJokerRemovalEffs.Add(() =>
                {
                    if (!ZoneManager.JokerZone.Cards.Any(x => x.isJoker && x.JokerData.DBName == "PAREIDOLIA"))
                        EngineUtils.RankGroups["FACE"] = new List<Rank>() { Rank.JACK, Rank.QUEEN, Rank.KING };
                });
                return ret;
            } },
            { "GROS MICHEL", c =>
            {
                var ret = BasicDataBlock("Gros Michel");
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult, 1 in " + ret.DataDict["DENOMINATOR"].IntData + " chance this is destroyed at end of round";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 15, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("NUMERATOR", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("DENOMINATOR", new JokerData() { IntData = 6, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(BuildMultAddListener(c, ret));
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.EndPlayRound,
                    MyAction = _ =>
                    {
                        if (ZoneManager.JokerZone.Cards.Contains(c) && Globals.RollRandom(ret.DataDict["NUMERATOR"].IntData, ret.DataDict["DENOMINATOR"].IntData, c))
                            ZoneManager.DestroyCard(c, ZoneManager.JokerZone);
                    },
                });
                return ret;
            } },
            { "EVEN STEVEN", c =>
            {
                var ret = BasicDataBlock("Even Steven");
                ret.DescriptionBuilder = _ => "Played cards with even rank give +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when scored (10, 8, 6, 4, 2)";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 4, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger && new List<Rank> { Rank.TEN, Rank.EIGHT, Rank.SIX, Rank.FOUR, Rank.TWO }.Contains(triggerArgs.CardThatIsTriggering.Rank))
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    },
                });
                return ret;
            } },
            { "ODD TODD", c =>
            {
                var ret = BasicDataBlock("Odd Todd");
                ret.DescriptionBuilder = _ => "Played cards with odd rank give +" + ret.DataDict["CHIPAMOUNT"].IntData + " Chips when scored (A, 9, 7, 5, 3)";
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 31, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger && new List<Rank> { Rank.ACE, Rank.NINE, Rank.SEVEN, Rank.FIVE, Rank.THREE }.Contains(triggerArgs.CardThatIsTriggering.Rank))
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                    },
                });
                return ret;
            } },
            { "SCHOLAR", c =>
            {
                var ret = BasicDataBlock("Scholar");
                ret.DescriptionBuilder = _ => "Played Aces give +" + ret.DataDict["CHIPAMOUNT"].IntData + " Chips and +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when scored.";
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 20, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 4, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger && triggerArgs.CardThatIsTriggering.Rank == Rank.ACE)
                        {
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                        }
                    },
                });
                return ret;
            } },
            { "BUSINESS CARD", c =>
            {
                var ret = BasicDataBlock("Business Card");
                ret.DescriptionBuilder = _ => "Played face cards have a " + ret.DataDict["NUMERATOR"].IntData + " in " + ret.DataDict["DENOMINATOR"].IntData + " chance to give $" + ret.DataDict["MONEYAMOUNT"].IntData + " when scored";
                ret.DataDict.Add("NUMERATOR", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("DENOMINATOR", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("MONEYAMOUNT", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger && EngineUtils.isFace(triggerArgs.CardThatIsTriggering) && Globals.RollRandom(ret.DataDict["NUMERATOR"].IntData, ret.DataDict["DENOMINATOR"].IntData, c))
                            Globals.EmitMoneyGain(ret.DataDict["MONEYAMOUNT"].IntData, c);
                    },
                });
                return ret;
            } },
            { "SUPERNOVA", c =>
            {
                var ret = BasicDataBlock("Supernova", "Adds the number of times poker hand has been played this run to Mult");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger && triggerArgs.CardThatIsTriggering == c)
                            Globals.EmitMultAdd(ScoreHandler.HandNumTimesPlayed[triggerArgs.HandCurrentlyBeingPlayed], c);
                    },
                });
                return ret;
            } },
            { "RIDE THE BUS", c =>
            {
                var ret = BasicDataBlock("Ride the Bus");
                ret.DescriptionBuilder = _ => "This Joker gains +1 Mult per consecutive hand played without a scoring face card (Currently +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult)";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 0, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("HITFACE", new JokerData() { IntData = 0, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger)
                        {
                            if (triggerArgs.CardThatIsTriggering == c && ret.DataDict["MULTAMOUNT"].DoubleData > 0)
                                Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                            else if (EngineUtils.isFace(triggerArgs.CardThatIsTriggering))
                                ret.DataDict["HITFACE"].IntData = 1;
                        }
                    },
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayDone,
                    MyAction = _ =>
                    {
                        if (ret.DataDict["HITFACE"].IntData == 1)
                            ret.DataDict["MULTAMOUNT"].DoubleData = 0;
                        else
                            ret.DataDict["MULTAMOUNT"].DoubleData += 1;
                        ret.DataDict["HITFACE"].IntData = 0;
                    },
                });
                return ret;
            } },
            { "SPACE JOKER", c =>
            {
                var ret = BasicDataBlock("Space Joker");
                ret.DescriptionBuilder = _ => ret.DataDict["NUMERATOR"].IntData + " in " + ret.DataDict["DENOMINATOR"].IntData + " chance to upgrade level of played poker hand";
                ret.DataDict.Add("NUMERATOR", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("DENOMINATOR", new JokerData() { IntData = 4, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayDone,
                    MyAction = args =>
                    {
                        if (args is EngineHandPlayDoneArgs handArgs && Globals.RollRandom(ret.DataDict["NUMERATOR"].IntData, ret.DataDict["DENOMINATOR"].IntData, c))
                            ScoreHandler.LevelUpHand(handArgs.HandTypeThatWasPlayed);
                    },
                });
                return ret;
            } },
            { "EGG", c =>
            {
                var ret = BasicDataBlock("Egg");
                ret.DescriptionBuilder = _ => "Gains $" + ret.DataDict["SELLAMOUNT"].IntData + " of sell value at end of round (Currently $" + c.BonusSellValue + ")";
                ret.DataDict.Add("SELLAMOUNT", new JokerData() { IntData = 3, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.EndPlayRound,
                    MyAction = _ => c.BonusSellValue += ret.DataDict["SELLAMOUNT"].IntData,
                });
                return ret;
            } },
            { "BURGLAR", c =>
            {
                var ret = BasicDataBlock("Burglar", "When Blind is selected, gain +3 Hands and lose all discards");
                ret.DataDict.Add("HANDAMOUNT", new JokerData() { IntData = 3, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.StartPlayRoundSetupOver,
                    MyAction = _ =>
                    {
                        Globals.CurHandsRemaining += ret.DataDict["HANDAMOUNT"].IntData;
                        Globals.CurDiscardsRemaining = 0;
                    },
                });
                return ret;
            } },
            { "BLACKBOARD", c =>
            {
                var ret = BasicDataBlock("Blackboard", "X3 Mult if all cards held in hand are Spades or Clubs");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger)
                        {
                            var heldCards = ZoneManager.HandZone.Cards.Where(x => !x.isSelected).ToList();
                            if ((!heldCards.Any()) || heldCards.All(x => x.IsSuit(Suit.SPADES) || x.IsSuit(Suit.CLUBS)))
                                Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c);
                        }
                    },
                });
                return ret;
            } },
            { "RUNNER", c =>
            {
                var ret = BasicDataBlock("Runner");
                ret.DescriptionBuilder = _ => "Gains +15 Chips if played hand contains a Straight (Currently +" + ret.DataDict["CHIPAMOUNT"].IntData + " Chips)";
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 0, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("CHIPGAIN", new JokerData() { IntData = 15, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger)
                        {
                            if (EngineUtils.HandContainsOtherHand(triggerArgs.HandCurrentlyBeingPlayed, PlayedHandType.STRAIGHT))
                                ret.DataDict["CHIPAMOUNT"].IntData += ret.DataDict["CHIPGAIN"].IntData;
                            if (ret.DataDict["CHIPAMOUNT"].IntData > 0)
                                Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                        }
                    },
                });
                return ret;
            } },
            { "ICE CREAM", c =>
            {
                var ret = BasicDataBlock("Ice Cream");
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["CHIPAMOUNT"].IntData + " Chips, -" + ret.DataDict["CHIPLOSS"].IntData + " Chips for every hand played";
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 100, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("CHIPLOSS", new JokerData() { IntData = 5, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && ret.DataDict["CHIPAMOUNT"].IntData > 0)
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                    },
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayDone,
                    MyAction = _ =>
                    {
                        ret.DataDict["CHIPAMOUNT"].IntData = Math.Max(0, ret.DataDict["CHIPAMOUNT"].IntData - ret.DataDict["CHIPLOSS"].IntData);
                        if (ret.DataDict["CHIPAMOUNT"].IntData == 0 && ZoneManager.JokerZone.Cards.Contains(c))
                            ZoneManager.DestroyCard(c, ZoneManager.JokerZone);
                    },
                });
                return ret;
            } },
            { "DNA", c =>
            {
                var ret = BasicDataBlock("DNA", "If first hand of round has only 1 card, add a permanent copy to deck and draw it to hand.");
                ret.DataDict.Add("FIRSTHAND", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener(){ MyContextType = EventContextType.StartPlayRound, MyAction = _ => ret.DataDict["FIRSTHAND"].IntData = 1 });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayScoringDone,
                    MyAction = _ =>
                    {
                        if (ret.DataDict["FIRSTHAND"].IntData == 1 && ZoneManager.CurrentlyBeingPlayedZone.Cards.Count == 1)
                        {
                            var copy = new Card();
                            ZoneManager.CurrentlyBeingPlayedZone.Cards[0].TurnIntoCopyOfMe(copy);
                            ZoneManager.DeckZone.AddCard(copy);
                            ZoneManager.HandZone.DrawTargetFrom(ZoneManager.DeckZone, copy, ignoreSpaceLimits: true);
                        }
                        ret.DataDict["FIRSTHAND"].IntData = 0;
                    },
                });
                return ret;
            } },
            { "SPLASH", c =>
            {
                var ret = BasicDataBlock("Splash", "Every played card counts in scoring");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.SelectedCardBeingConsideredForCalc,
                    MyAction = args =>
                    {
                        if (args is EngineCardChosenForPlayedHandArgs handArgs)
                            handArgs.WillBeIncludedInCalc = true;
                    },
                });
                return ret;
            } },
            { "BLUE JOKER", c =>
            {
                var ret = BasicDataBlock("Blue Joker");
                ret.DescriptionBuilder = _ => "+2 Chips for each remaining card currently in deck (Currently +" + (ZoneManager.DeckZone.Cards.Count * ret.DataDict["CHIPAMOUNT"].IntData) + " Chips)";
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger)
                            Globals.EmitChipsAdd(ZoneManager.DeckZone.Cards.Count * ret.DataDict["CHIPAMOUNT"].IntData, c);
                    },
                });
                return ret;
            } },
            { "SIXTH SENSE", c =>
            {
                var ret = BasicDataBlock("Sixth Sense", "If first hand of round is a single 6, destroy it and create a Spectral card (Must have room)");
                ret.DataDict.Add("FIRSTHAND", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener(){ MyContextType = EventContextType.StartPlayRound, MyAction = _ => ret.DataDict["FIRSTHAND"].IntData = 1 });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayScoringDone,
                    MyAction = _ =>
                    {
                        if (ret.DataDict["FIRSTHAND"].IntData == 1 && ZoneManager.CurrentlyBeingPlayedZone.Cards.Count == 1 && ZoneManager.CurrentlyBeingPlayedZone.Cards[0].Rank == Rank.SIX)
                        {
                            ZoneManager.DestroyCard(ZoneManager.CurrentlyBeingPlayedZone.Cards[0], ZoneManager.CurrentlyBeingPlayedZone);
                            if (ZoneManager.ConsumableZone.HasRoom)
                            {
                                MarketOptionsManager.DrawMarketItem(BuyItemType.SPECTRAL_CARD, ZoneManager.ConsumableZone);
                            }
                        }
                        ret.DataDict["FIRSTHAND"].IntData = 0;
                    },
                });
                return ret;
            } },
            { "CONSTELLATION", c =>
            {
                var ret = BasicDataBlock("Constellation");
                ret.DescriptionBuilder = _ => "This Joker gains X0.1 Mult every time a Planet card is used (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.0") + " Mult)";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 0.1, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener(){ MyContextType = EventContextType.ConsumableUsed, MyAction = args => { if(args is EngineConsumableUseArgs conArgs && conArgs.TypeUsed == ConsumableType.PLANET) ret.DataDict["MULTMULTAMOUNT"].DoubleData += ret.DataDict["MULTMULTGAIN"].DoubleData; }});
                ret.Listeners.Add(new EngineEventListener(){ MyContextType = EventContextType.CardTrigger, MyAction = args => { if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger) Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); }});
                return ret;
            } },
            {"HIKER", c =>
            {
                var ret = BasicDataBlock("Hiker");
                ret.DescriptionBuilder = _ => "Every played card permanently gains +" + ret.DataDict["CHIPGAIN"].IntData + " Chips when scored";
                ret.DataDict.Add("CHIPGAIN", new JokerData() { IntData = 5, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardPreTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardPreTriggerArgs triggerArgs && ZoneManager.CurrentlyBeingPlayedZone.Cards.Contains(triggerArgs.CardAboutToTrigger))
                            triggerArgs.CardAboutToTrigger.ChipsBase += ret.DataDict["CHIPGAIN"].IntData;
                    },
                });
                return ret;
            } },
            {"FACELESS JOKER", c =>
            {
                var ret = BasicDataBlock("Faceless Joker");
                ret.DescriptionBuilder = _ => "Earn $" + ret.DataDict["MONEYAMOUNT"].IntData + " if " + ret.DataDict["INTAMOUNT"].IntData + " or more face cards are discarded at the same time.";
                ret.DataDict.Add("MONEYAMOUNT", new JokerData() { IntData = 5, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("INTAMOUNT", new JokerData() { IntData = 3, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandDiscardDone,
                    MyAction = args =>
                    {
                        if (args is EngineDiscardDoneArgs discardArgs && discardArgs.BeingDiscarded.Count(card => EngineUtils.isFace(card)) >= ret.DataDict["INTAMOUNT"].IntData)
                        {
                            Globals.EmitMoneyGain(ret.DataDict["MONEYAMOUNT"].IntData, c);
                        }
                    },
                });
                return ret;
            } },
            {"GREEN JOKER", c =>
            {
                var ret = BasicDataBlock("Green Joker");
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult per hand played, -" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult per discard (Currently +" + ret.DataDict["CURAMOUNT"].DoubleData + " Mult)";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("CURAMOUNT", new JokerData() { DoubleData = 0, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(BuildMultAddListener(c, ret, fieldName: "CURAMOUNT"));
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandDiscardDone,
                    MyAction = args =>
                    {
                        if (args is EngineDiscardDoneArgs discardArgs)
                        {
                            ret.DataDict["CURAMOUNT"].DoubleData -= ret.DataDict["MULTAMOUNT"].DoubleData;
                            if (ret.DataDict["CURAMOUNT"].DoubleData < 0)
                                ret.DataDict["CURAMOUNT"].DoubleData = 0;
                        }
                    },
                });
                ret.Listeners.Add(new EngineEventListener() {
                    MyContextType = EventContextType.HandPlayedCalculated,
                    MyAction = args =>
                    {
                        if (args is EngineHandPlayArgs)
                            ret.DataDict["CURAMOUNT"].DoubleData += ret.DataDict["MULTAMOUNT"].DoubleData;
                    },
                });
                return ret;
            } },
            {"SUPERPOSITION", c =>
            {
                var ret = BasicDataBlock("Superposition", "Create a Tarot card if poker hand contains an Ace and a Straight (Must have room)");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayDone,
                    MyAction = args =>
                    {
                        if (args is EngineHandPlayDoneArgs handArgs && EngineUtils.HandContainsOtherHand(handArgs.HandTypeThatWasPlayed, PlayedHandType.STRAIGHT) && handArgs.CardsInPlayedHand.Any(x => x.Rank == Rank.ACE) && ZoneManager.ConsumableZone.HasRoom)
                        {
                            MarketOptionsManager.DrawMarketItem(BuyItemType.TAROT_CARD, ZoneManager.ConsumableZone);
                        }
                    },
                });
                return ret;
            } },
            {"TO DO LIST", c =>
            {
                var ret = BasicDataBlock("To Do List");
                ret.DescriptionBuilder = _ => "Earn $" + ret.DataDict["MONEYAMOUNT"].IntData + " if poker hand is a " + ret.DataDict["HANDTYPE"].HandTypeData.ToString() + ", poker hand changes at end of round.";
                ret.DataDict.Add("MONEYAMOUNT", new JokerData() { IntData = 4, MyDataType = JokerDataType.INT });
                var getRandomHand = new Func<PlayedHandType, PlayedHandType>(oldHand =>
                {
                    var handTypes = Enum.GetValues<PlayedHandType>().Where(x => x != oldHand).ToList();//Makes sure we never roll to the same hand.
                    return handTypes[Random.Shared.Next(handTypes.Count)];
                });
                ret.DataDict.Add("HANDTYPE", new JokerData() { HandTypeData = getRandomHand(PlayedHandType.HIGHCARD), MyDataType = JokerDataType.HANDTYPE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.EndPlayRound,
                    MyAction = args =>
                    {
                        ret.DataDict["HANDTYPE"].HandTypeData = getRandomHand(ret.DataDict["HANDTYPE"].HandTypeData);
                    },
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayDone,
                    MyAction = args =>
                    {
                        if (args is EngineHandPlayDoneArgs handArgs && handArgs.HandTypeThatWasPlayed == ret.DataDict["HANDTYPE"].HandTypeData)
                        {
                            Globals.EmitMoneyGain(ret.DataDict["MONEYAMOUNT"].IntData, c);
                        }
                    },
                });
                return ret;
            } },
            { "CAVENDISH", c =>
            {
                var ret = BasicDataBlock("Cavendish");
                ret.DescriptionBuilder = _ => "X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult, 1 in " + ret.DataDict["DENOMINATOR"].IntData + " chance this is destroyed at end of round";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("NUMERATOR", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("DENOMINATOR", new JokerData() { IntData = 1000, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1)
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c);
                    },
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.EndPlayRound,
                    MyAction = _ =>
                    {
                        if (ZoneManager.JokerZone.Cards.Contains(c) && Globals.RollRandom(ret.DataDict["NUMERATOR"].IntData, ret.DataDict["DENOMINATOR"].IntData, c))
                            ZoneManager.DestroyCard(c, ZoneManager.JokerZone);
                    },
                });
                return ret;
            } },


            //TODO: REMOVE BELOW AFTER REAL UNCOMMON/RARE ADDED, THESE ONLY FOR UNIT TESTS.
            { "TEMP UNCOMMON JOKER", c =>
            {
                var ret = new JokerCardDataBlock();
                ret.JokerName = "TEMP UNCOMMON Joker";
                ret.DBName = "TEMP UNCOMMON JOKER";
                ret.DescriptionBuilder = _ => "Temporary Uncommon Joker TO BE REMOVED";

                return ret;
            } },
            { "TEMP RARE JOKER", c =>
            {
                var ret = new JokerCardDataBlock();
                ret.JokerName = "TEMP RARE Joker";
                ret.DBName = "TEMP RARE JOKER";
                ret.DescriptionBuilder = _ => "Temporary Rare Joker TO BE REMOVED";

                return ret;
            } },
            { "TEMP LEGENDARY JOKER", c =>
            {
                var ret = new JokerCardDataBlock();
                ret.JokerName = "TEMP LEGENDARY Joker";
                ret.DBName = "TEMP LEGENDARY JOKER";
                ret.DescriptionBuilder = _ => "Temporary Legendary Joker TO BE REMOVED";

                return ret;
            } },
        };

        public static string GetRandomJokerOfRarity(JokerRarity rarity)
        {
            var jokersOfRarity = JokerMetadata.Where(x => x.Value.Rarity == rarity).ToList();
            if (!jokersOfRarity.Any())
                return null;
            var randomIndex = Random.Shared.Next(jokersOfRarity.Count);
            return jokersOfRarity[randomIndex].Key;
        }

        public static void MakeCardJoker(Card c, string JokerName)
        {
            var toSet = JokerData[JokerName](c);
            c.JokerData = toSet;
            c.JokerData.MyCard = c;
            if (JokerMetadata.ContainsKey(JokerName))
            {
                c.BaseCost = JokerMetadata[JokerName].Price;
                c.JokerData.Rarity = JokerMetadata[JokerName].Rarity;
            }
        }

        //Generate and return a fresh Card object that is the named joker (DB NAME)
        public static Card GenerateJokerCard(string JokerName)
        {
            var c = new Card();
            MakeCardJoker(c, JokerName);
            return c;
        }

        public static Card GenerateDefaultJokerCard() => GenerateJokerCard(DEFAULT_JOKER_NAME);

        private static JokerCardDataBlock BasicDataBlock(string name)
        {
            var ret = new JokerCardDataBlock();
            ret.JokerName = name;
            ret.DBName = name.ToUpper();
            return ret;
        }

        private static JokerCardDataBlock BasicDataBlock(string name, string desc)
        {
            var ret = BasicDataBlock(name);
            ret.DescriptionBuilder = _ => desc;
            return ret;
        }

        private static JokerCardDataBlock BasicDataBlock(string name, Func<EventContext, string> descBuilder)
        {
            var ret = BasicDataBlock(name);
            ret.DescriptionBuilder = descBuilder;
            return ret;
        }

        private static EngineEventListener BuildMultAddListener(Card c, JokerCardDataBlock dBlock, string fieldName = "MULTAMOUNT")
        {
            return new EngineEventListener()
            {
                MyContextType = EventContextType.CardTrigger,
                MyAction = args =>
                {
                    if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && dBlock.DataDict[fieldName].DoubleData > 0)
                        Globals.EmitMultAdd(dBlock.DataDict[fieldName].DoubleData, c);
                },
            };
        }

        private static EngineEventListener BuildMultAddListener(Card c, JokerCardDataBlock dBlock, Func<bool> extraCondition, string fieldName = "MULTAMOUNT")
        {
            return new EngineEventListener()
            {
                MyContextType = EventContextType.CardTrigger,
                MyAction = args =>
                {
                    if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && dBlock.DataDict["MULTAMOUNT"].DoubleData > 0 && extraCondition())
                        Globals.EmitMultAdd(dBlock.DataDict["MULTAMOUNT"].DoubleData, c);
                },
            };
        }
    }
}
