using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
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
            {"LSTY JOKER", new JokerTypeData { DBName = "LSTY JOKER", Price = 5 } },
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
            {"STENCIL JOKER", new JokerTypeData { DBName = "STENCIL JOKER", Price = 8 } },
            {"FOUR FINGERS", new JokerTypeData { DBName = "FOUR FINGERS", Price = 7 } },
            {"MIME", new JokerTypeData { DBName = "MIME", Price = 5 } },
            {"CREDIT CARD", new JokerTypeData { DBName = "CREDIT CARD", Price = 1 } },
            {"GOLDEN JOKER", new JokerTypeData { DBName = "GOLDEN JOKER", Price = 5 } },
            {"TEMP UNCOMMON JOKER", new JokerTypeData { DBName = "TEMP UNCOMMON JOKER", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"TEMP RARE JOKER", new JokerTypeData { DBName = "TEMP RARE JOKER", Price = 5, Rarity = JokerRarity.RARE } },
        };

        public static List<string> JokerDbNames => JokerData.Keys.ToList();

        //Define our jokers by functions that build them out of a passed card.
        public static Dictionary<string, Func<Card, JokerCardDataBlock>> JokerData = new()
        {
            { "JIMBO", c =>
            {
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Basic Joker";
                ret.DBName = "JIMBO";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Greedy Joker";
                ret.DBName = "GREEDY JOKER";
                ret.DescriptionBuilder = _ => "Played cards with " + ret.DataDict["SUIT"].SpecificCardSuit + " Suit give +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when scored";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE});
                ret.DataDict.Add("SUIT", new JokerData() { SpecificCardSuit = Suit.DIAMONDS, MyDataType = JokerDataType.SUIT});
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
            { "LUSTY JOKER", c =>
            {
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Lusty Joker";
                ret.DBName = "LUSTY JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Wrathful Joker";
                ret.DBName = "WRATHFUL JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Gluttonous Joker";
                ret.DBName = "GLUTTONOUS JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Jolly Joker";
                ret.DBName = "JOLLY JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Zany Joker";
                ret.DBName = "ZANY JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Mad Joker";
                ret.DBName = "MAD JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Crazy Joker";
                ret.DBName = "CRAZY JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Droll Joker";
                ret.DBName = "DROLL JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Sly Joker";
                ret.DBName = "SLY JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Wily Joker";
                ret.DBName = "WILY JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Clever Joker";
                ret.DBName = "CLEVER JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Devious Joker";
                ret.DBName = "DEVIOUS JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Crafty Joker";
                ret.DBName = "CRAFTY JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Half Joker";
                ret.DBName = "HALF JOKER";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Stencil Joker";
                ret.DBName = "STENCIL JOKER";
                Func<double> getCurrentAmt = () => (ZoneManager.JokerZone.MaxCapacity - ZoneManager.JokerZone.Cards.Where(x => x.isJoker && x.JokerData.DBName != "STENCIL JOKER").Count()) * ret.DataDict["MULTMULTAMOUNT"].DoubleData;
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Four Fingers";
                ret.DBName = "FOUR FINGERS";
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
                    if(!ZoneManager.JokerZone.Cards.Where(x => x.isJoker && x.JokerData.DBName == "FOUR FINGERS").Any())
                    {
                        EngineUtils.LenFlush = ret.DataDict["OLDFLUSH"].IntData;
                        EngineUtils.LenStraight = ret.DataDict["OlDSTRAIGHT"].IntData;
                    }
                });

                return ret;
            } },
            { "MIME", c =>
            {
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Mime";
                ret.DBName = "MIME";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Credit Card";
                ret.DBName = "CREDIT CARD";
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
                var ret = new JokerCardDataBlock();
                ret.JokerName = "Golden Joker";
                ret.DBName = "GOLDEN JOKER";
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
            } },//TODO: REMOVE BELOW AFTER REAL UNCOMMON/RARE ADDED, THESE ONLY FOR UNIT TESTS.
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
    }
}
