using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Tags;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using ConsoleBalatro.Engine.Pools;
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
        public static HashSet<string> JokersNotCopyable = new()
        {
            "ASTRONOMER",
            "CHAOS THE CLOWN",
            "CHICOT",
            "CLOUD 9",
            "CREDIT CARD",
            "DELAYED GRATIFICATION",
            "DRUNKARD",
            "EGG",
            "FOUR FINGERS",
            "GIFT CARD",
            "GOLDEN JOKER",
            "INVISIBLE JOKER",
            "JUGGLER",
            "MERRY ANDY",
            "MIDAS MASK",
            "MR. BONES",
            "OOPS! ALL 6S",
            "PAREIDOLIA",
            "ROCKET",
            "SATELLITE",
            "SHORTCUT",
            "SHOWMAN",
            "SIXTH SENSE",
            "SMEARED JOKER",
            "SPLASH",
            "TO THE MOON",
            "TRADING CARD",
            "TROUBADOUR",
            "TURTLE BEAN",
        };


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
            {"CARD SHARP", new JokerTypeData { DBName = "CARD SHARP", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"RED CARD", new JokerTypeData { DBName = "RED CARD", Price = 5 } },
            {"MADNESS", new JokerTypeData { DBName = "MADNESS", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"SQUARE JOKER", new JokerTypeData { DBName = "SQUARE JOKER", Price = 4 } },
            {"SEANCE", new JokerTypeData { DBName = "SEANCE", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"RIFF-RAFF", new JokerTypeData { DBName = "RIFF-RAFF", Price = 6 } },
            {"VAMPIRE", new JokerTypeData { DBName = "VAMPIRE", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"SHORTCUT", new JokerTypeData { DBName = "SHORTCUT", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"HOLOGRAM", new JokerTypeData { DBName = "HOLOGRAM", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"VAGABOND", new JokerTypeData { DBName = "VAGABOND", Price = 8, Rarity = JokerRarity.RARE } },
            {"BARON", new JokerTypeData { DBName = "BARON", Price = 8, Rarity = JokerRarity.RARE } },
            {"CLOUD 9", new JokerTypeData { DBName = "CLOUD 9", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"ROCKET", new JokerTypeData { DBName = "ROCKET", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"OBELISK", new JokerTypeData { DBName = "OBELISK", Price = 8, Rarity = JokerRarity.RARE } },
            {"MIDAS MASK", new JokerTypeData { DBName = "MIDAS MASK", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"LUCHADOR", new JokerTypeData { DBName = "LUCHADOR", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"PHOTOGRAPH", new JokerTypeData { DBName = "PHOTOGRAPH", Price = 5 } },
            {"GIFT CARD", new JokerTypeData { DBName = "GIFT CARD", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"TURTLE BEAN", new JokerTypeData { DBName = "TURTLE BEAN", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"STONE JOKER", new JokerTypeData { DBName = "STONE JOKER", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"EROSION", new JokerTypeData { DBName = "EROSION", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"RESERVED PARKING", new JokerTypeData { DBName = "RESERVED PARKING", Price = 6 } },
            {"MAIL-IN REBATE", new JokerTypeData { DBName = "MAIL-IN REBATE", Price = 4 } },
            {"TO THE MOON", new JokerTypeData { DBName = "TO THE MOON", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"HALLUCINATION", new JokerTypeData { DBName = "HALLUCINATION", Price = 4 } },
            {"FORTUNE TELLER", new JokerTypeData { DBName = "FORTUNE TELLER", Price = 6 } },
            {"JUGGLER", new JokerTypeData { DBName = "JUGGLER", Price = 4 } },
            {"DRUNKARD", new JokerTypeData { DBName = "DRUNKARD", Price = 4 } },
            {"LUCKY CAT", new JokerTypeData { DBName = "LUCKY CAT", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"BASEBALL CARD", new JokerTypeData { DBName = "BASEBALL CARD", Price = 8, Rarity = JokerRarity.RARE } },
            {"BULL", new JokerTypeData { DBName = "BULL", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"DIET COLA", new JokerTypeData { DBName = "DIET COLA", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"TRADING CARD", new JokerTypeData { DBName = "TRADING CARD", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"FLASH CARD", new JokerTypeData { DBName = "FLASH CARD", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"POPCORN", new JokerTypeData { DBName = "POPCORN", Price = 5 } },
            {"SPARE TROUSERS", new JokerTypeData { DBName = "SPARE TROUSERS", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"ANCIENT JOKER", new JokerTypeData { DBName = "ANCIENT JOKER", Price = 8, Rarity = JokerRarity.RARE } },
            {"RAMEN", new JokerTypeData { DBName = "RAMEN", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"WALKIE TALKIE", new JokerTypeData { DBName = "WALKIE TALKIE", Price = 4 } },
            {"SELTZER", new JokerTypeData { DBName = "SELTZER", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"CASTLE", new JokerTypeData { DBName = "CASTLE", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"SMILEY FACE", new JokerTypeData { DBName = "SMILEY FACE", Price = 4 } },
            {"CAMPFIRE", new JokerTypeData { DBName = "CAMPFIRE", Price = 9, Rarity = JokerRarity.RARE } },
            {"GOLDEN TICKET", new JokerTypeData { DBName = "GOLDEN TICKET", Price = 5 } },
            {"MR. BONES", new JokerTypeData { DBName = "MR. BONES", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"ACROBAT", new JokerTypeData { DBName = "ACROBAT", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"SOCK AND BUSKIN", new JokerTypeData { DBName = "SOCK AND BUSKIN", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"SWASHBUCKLER", new JokerTypeData { DBName = "SWASHBUCKLER", Price = 4 } },
            {"TROUBADOUR", new JokerTypeData { DBName = "TROUBADOUR", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"CERTIFICATE", new JokerTypeData { DBName = "CERTIFICATE", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"SMEARED JOKER", new JokerTypeData { DBName = "SMEARED JOKER", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"THROWBACK", new JokerTypeData { DBName = "THROWBACK", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"HANGING CHAD", new JokerTypeData { DBName = "HANGING CHAD", Price = 4 } },
            {"ROUGH GEM", new JokerTypeData { DBName = "ROUGH GEM", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"BLOODSTONE", new JokerTypeData { DBName = "BLOODSTONE", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"ARROWHEAD", new JokerTypeData { DBName = "ARROWHEAD", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"ONYX AGATE", new JokerTypeData { DBName = "ONYX AGATE", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"GLASS JOKER", new JokerTypeData { DBName = "GLASS JOKER", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"SHOWMAN", new JokerTypeData { DBName = "SHOWMAN", Price = 5, Rarity = JokerRarity.UNCOMMON } },
            {"FLOWER POT", new JokerTypeData { DBName = "FLOWER POT", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"BLUEPRINT", new JokerTypeData { DBName = "BLUEPRINT", Price = 10, Rarity = JokerRarity.RARE } },
            {"WEE JOKER", new JokerTypeData { DBName = "WEE JOKER", Price = 8, Rarity = JokerRarity.RARE } },
            {"MERRY ANDY", new JokerTypeData { DBName = "MERRY ANDY", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"OOPS! ALL 6S", new JokerTypeData { DBName = "OOPS! ALL 6S", Price = 4, Rarity = JokerRarity.UNCOMMON } },
            {"THE IDOL", new JokerTypeData { DBName = "THE IDOL", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"SEEING DOUBLE", new JokerTypeData { DBName = "SEEING DOUBLE", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"MATADOR", new JokerTypeData { DBName = "MATADOR", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"HIT THE ROAD", new JokerTypeData { DBName = "HIT THE ROAD", Price = 8, Rarity = JokerRarity.RARE } },
            {"THE DUO", new JokerTypeData { DBName = "THE DUO", Price = 8, Rarity = JokerRarity.RARE } },
            {"THE TRIO", new JokerTypeData { DBName = "THE TRIO", Price = 8, Rarity = JokerRarity.RARE } },
            {"THE FAMILY", new JokerTypeData { DBName = "THE FAMILY", Price = 8, Rarity = JokerRarity.RARE } },
            {"THE ORDER", new JokerTypeData { DBName = "THE ORDER", Price = 8, Rarity = JokerRarity.RARE } },
            {"THE TRIBE", new JokerTypeData { DBName = "THE TRIBE", Price = 8, Rarity = JokerRarity.RARE } },
            {"STUNTMAN", new JokerTypeData { DBName = "STUNTMAN", Price = 7, Rarity = JokerRarity.RARE } },
            {"INVISIBLE JOKER", new JokerTypeData { DBName = "INVISIBLE JOKER", Price = 8, Rarity = JokerRarity.RARE } },
            {"BRAINSTORM", new JokerTypeData { DBName = "BRAINSTORM", Price = 10, Rarity = JokerRarity.RARE } },
            {"SATELLITE", new JokerTypeData { DBName = "SATELLITE", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"SHOOT THE MOON", new JokerTypeData { DBName = "SHOOT THE MOON", Price = 5 } },
            {"DRIVER'S LICENSE", new JokerTypeData { DBName = "DRIVER'S LICENSE", Price = 7, Rarity = JokerRarity.RARE } },
            {"CARTOMANCER", new JokerTypeData { DBName = "CARTOMANCER", Price = 6, Rarity = JokerRarity.UNCOMMON } },
            {"ASTRONOMER", new JokerTypeData { DBName = "ASTRONOMER", Price = 8, Rarity = JokerRarity.UNCOMMON } },
            {"BURNT JOKER", new JokerTypeData { DBName = "BURNT JOKER", Price = 8, Rarity = JokerRarity.RARE } },
            {"BOOTSTRAPS", new JokerTypeData { DBName = "BOOTSTRAPS", Price = 7, Rarity = JokerRarity.UNCOMMON } },
            {"CANIO", new JokerTypeData { DBName = "CANIO", Price = 20, Rarity = JokerRarity.LEGENDARY } },
            {"TRIBOULET", new JokerTypeData { DBName = "TRIBOULET", Price = 20, Rarity = JokerRarity.LEGENDARY } },
            {"YORICK", new JokerTypeData { DBName = "YORICK", Price = 20, Rarity = JokerRarity.LEGENDARY } },
            {"CHICOT", new JokerTypeData { DBName = "CHICOT", Price = 20, Rarity = JokerRarity.LEGENDARY } },
            {"PERKEO", new JokerTypeData { DBName = "PERKEO", Price = 20, Rarity = JokerRarity.LEGENDARY } },
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
                ret.DescriptionBuilder = _ => "+ " + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult if Played Hand contains " + ret.DataDict["INTAMOUNT"].IntData + " or fewer cards.";
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

                ret.OnCopyModifications = newDb =>
                {
                    newDb.Listeners.Clear();
                    newDb.Listeners.Add(BuildMultAddListener(newDb.MyCard, ret));
                };

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
                        var newCard = CardFactory.PlayingCardFromRankSuit((Rank)Globals.randomNext(1, 14), (Suit)Globals.randomNext(4));
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
                            var tarotType = ConsumableManager.TarotNames[Globals.randomNext(ConsumableManager.TarotNames.Count)];
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
                            ret.DataDict["MULTAMOUNT"].DoubleData = Globals.randomNext(24);
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
                        ret.DataDict["DISCARDS_START"].IntData = Globals.MaxDiscardsPerRound;
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
                        {
                            ZoneManager.DestroyCard(c, ZoneManager.JokerZone);
                            Globals.AddFlag("GROS_MICHEL_SELF_DESTROY");
                        }
                    },
                });

                ret.OnCopyModifications = newDB =>
                {
                    newDB.Listeners.RemoveAt(1);//remove the hidden destroy potential.
                };

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
                            Globals.EmitMultAdd(ScoreHandler.HandNumTimesPlayed[triggerArgs.HandCurrentlyBeingPlayed] + 1, c);
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
                                //MarketOptionsManager.DrawMarketItem(BuyItemType.SPECTRAL_CARD, ZoneManager.ConsumableZone);
                                MarketPullManager.DrawMarketItem(BuyItemType.SPECTRAL_CARD, ZoneManager.ConsumableZone, source: Pools.GenerationSource.GenericJoker);
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
                            //MarketOptionsManager.DrawMarketItem(BuyItemType.TAROT_CARD, ZoneManager.ConsumableZone);
                            MarketPullManager.DrawMarketItem(BuyItemType.TAROT_CARD, ZoneManager.ConsumableZone, source: GenerationSource.GenericJoker);
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
                    var handTypes = Enum.GetValues<PlayedHandType>().Where(x => x != oldHand && (ScoreHandler.HandNumTimesPlayed[x] > 0 || !PoolManager.SpecialHandTypes.Contains(x))).ToList();//Makes sure we never roll to the same hand.
                    return handTypes[Globals.randomNext(handTypes.Count)];
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
                ret.OnCopyModifications = newDB =>
                {
                    newDB.Listeners.RemoveAt(1);//remove the hidden destroy potential.
                };
                return ret;
            } },
            { "CARD SHARP", c =>
            {
                var ret = BasicDataBlock("Card Sharp");
                ret.DescriptionBuilder = _ => "X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult if played poker hand has already been played this round";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ScoreHandler.NumHandTypePlayedThisRound[t.HandCurrentlyBeingPlayed] >= 1) //geq because if 1, this is the second, not counted till hand play is done.
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    } });
                return ret;
            } },
            { "RED CARD", c =>
            {
                var ret = BasicDataBlock("Red Card");
                ret.DescriptionBuilder = _ => "This Joker gains +3 Mult when any Booster Pack is skipped (Currently +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult)";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 0, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTGAIN", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(BuildMultAddListener(c, ret));
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.PackSkip, 
                    MyAction = _ => ret.DataDict["MULTAMOUNT"].DoubleData += ret.DataDict["MULTGAIN"].DoubleData 
                });
                return ret;
            } },
            { "MADNESS", c =>
            {
                var ret = BasicDataBlock("Madness");
                ret.DescriptionBuilder = _ => "When Small Blind or Big Blind is selected, gain X0.5 Mult and destroy another random Joker (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.0") + " Mult)";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 0.5, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.StartPlayRoundSetupOver, 
                    MyAction = _ => { 
                        if (FlowHandler.CurrentSelectedBlind == BlindType.SMALL || FlowHandler.CurrentSelectedBlind == BlindType.BIG) 
                        { 
                            ret.DataDict["MULTMULTAMOUNT"].DoubleData += ret.DataDict["MULTMULTGAIN"].DoubleData; 
                            var valid = ZoneManager.JokerZone.Cards.Where(x => x != c && x.IsDestructible).ToList(); 
                            if (valid.Any())
                                ZoneManager.DestroyCard(valid[Globals.randomNext(valid.Count)], ZoneManager.JokerZone); 
                        } 
                    } 
                });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                        } 
                });

                ret.OnCopyModifications = newDb =>
                {
                    newDb.Listeners.Clear();
                    newDb.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.CardTrigger,
                        MyAction = args => {
                            if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == newDb.MyCard && t.isScoringTrigger)
                                Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, newDb.MyCard);
                            }
                    });
                };

                return ret;
            } },
            { "SQUARE JOKER", c =>
            {
                var ret = BasicDataBlock("Square Joker");
                ret.DescriptionBuilder = _ => "This Joker gains +" + ret.DataDict["CHIPGAIN"].IntData + " Chips if played hand has exactly 4 cards (Currently +" + ret.DataDict["CHIPAMOUNT"].IntData + " Chips)";
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 0, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("CHIPGAIN", new JokerData() { IntData = 4, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.HandPlayedCalculated, 
                    MyAction = args => 
                    { 
                        if (args is EngineHandPlayArgs h && h.CardsSelected.Count == 4) 
                            ret.DataDict["CHIPAMOUNT"].IntData += ret.DataDict["CHIPGAIN"].IntData; 
                    } 
                });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ret.DataDict["CHIPAMOUNT"].IntData > 0) 
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c); 
                    } 
                });
                return ret;
            } },
            { "SEANCE", c =>
            {
                var ret = BasicDataBlock("Seance", "If poker hand is a Straight Flush, create a random Spectral card (Must have room)");
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.HandPlayDone, 
                    MyAction = args => 
                    { 
                        if (args is EngineHandPlayDoneArgs h && h.HandTypeThatWasPlayed == PlayedHandType.STRAIGHTFLUSH && ZoneManager.ConsumableZone.HasRoom) 
                            MarketPullManager.DrawMarketItem(BuyItemType.SPECTRAL_CARD, ZoneManager.ConsumableZone, source: GenerationSource.GenericJoker); 
                            //MarketOptionsManager.DrawMarketItem(BuyItemType.SPECTRAL_CARD, ZoneManager.ConsumableZone); 
                    } 
                });
                return ret;
            } },
            { "RIFF-RAFF", c =>
            {
                var ret = BasicDataBlock("Riff-Raff", "When Blind is selected, create 2 Common Jokers (Must have room)");
                ret.DataDict.Add("NUMAMOUNT", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.StartPlayRoundSetupOver, 
                    MyAction = _ => 
                    {
                        ContentRollBatchContext context = new();
                        for (var i = 0; i < ret.DataDict["NUMAMOUNT"].IntData; i++) 
                        {
                            var commonJoker = MarketPullManager.PickMarketCard(BuyItemType.JOKER, source: GenerationSource.RiffRaffJoker, batchContext: context);
                            if (commonJoker == null || !ZoneManager.JokerZone.HasRoom)
                                break;
                            ZoneManager.JokerZone.AddCard(commonJoker, invisibleAdd: false);
                        } 
                    } 
                });
                return ret;
            } },

            { "VAMPIRE", c =>
            {
                var ret = BasicDataBlock("Vampire");
                ret.DescriptionBuilder = _ => "This Joker gains X0.1 Mult per scoring Enhanced card played, and permanently removes their Enhancements (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.0") + " Mult)";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 0.1, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardPreTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardPreTriggerArgs t && ZoneManager.CurrentlyBeingPlayedZone.Cards.Contains(t.CardAboutToTrigger) && t.CardAboutToTrigger.Enhancement != Enhancement.NONE) 
                        { 
                            ret.DataDict["MULTMULTAMOUNT"].DoubleData += ret.DataDict["MULTMULTGAIN"].DoubleData; 
                            t.CardAboutToTrigger.SetEnhancementOfficial(Enhancement.NONE); 
                        } 
                    } 
                });
                ret.Listeners.Add(new EngineEventListener() 
                {
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    } 
                });

                ret.OnCopyModifications = newDb =>
                {
                    newDb.Listeners.Clear();
                    newDb.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.CardTrigger,
                        MyAction = args =>
                        {
                            if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == newDb.MyCard && t.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1)
                                Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, newDb.MyCard);
                        }
                    });
                };
                return ret;
            } },
            { "SHORTCUT", c =>
            {
                var ret = BasicDataBlock("Shortcut", "Allows Straights to be made with gaps of 1 rank (ex: 10 8 6 5 3)");
                ret.DataDict.Add("SKIPSTRENGTH", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.OnJokerGainEffs.Add(() => EngineUtils.SkipStrength += ret.DataDict["SKIPSTRENGTH"].IntData);
                ret.OnJokerRemovalEffs.Add(() => EngineUtils.SkipStrength -= ret.DataDict["SKIPSTRENGTH"].IntData);
                return ret;
            } },
            { "HOLOGRAM", c =>
            {
                var ret = BasicDataBlock("Hologram");
                ret.DescriptionBuilder = _ => "This Joker gains X0.25 Mult every time a playing card is permanently added to your deck (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.00") + " Mult)";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 0.25, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardDrawnToZone, 
                    MyAction = args => 
                    {
                        if (args is EngineCardDrawnToZoneArgs d && (d.ZoneDrawnTo == ZoneManager.DeckZone || d.ZoneDrawnTo == ZoneManager.HandZone) && d.CardBeingDrawn.isPlayingCard && (d.ZoneDrawnFrom == null || d.ZoneDrawnFrom == ZoneManager.PackOptionZone || d.ZoneDrawnFrom == ZoneManager.MainMarketZone)) 
                            ret.DataDict["MULTMULTAMOUNT"].DoubleData += ret.DataDict["MULTMULTGAIN"].DoubleData; 
                    } 
                    //TODO: Above is a very bad approach. Ideally we should centralize new card creation (for cards meant to permanently add to the deck) then those can flag and raise an event.

                });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    } });
                return ret;
            }},
            { "VAGABOND", c =>
            {
                var ret = BasicDataBlock("Vagabond");
                ret.DescriptionBuilder = _ => "Create a Tarot card if hand is played with $" + ret.DataDict["MONEYREQ"].IntData + " or less (Must have room)";
                ret.DataDict.Add("MONEYREQ", new JokerData() { IntData = 4, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayDone,
                    MyAction = _ =>
                    {
                        if (Globals.Money <= ret.DataDict["MONEYREQ"].IntData && ZoneManager.ConsumableZone.HasRoom)
                            MarketPullManager.DrawMarketItem(BuyItemType.TAROT_CARD, ZoneManager.ConsumableZone, source : GenerationSource.GenericJoker);
                    },
                });
                return ret;
            } },
            { "BARON", c =>
            {
                var ret = BasicDataBlock("Baron");
                ret.DescriptionBuilder = _ => "Each King held in hand gives X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.0") + " Mult";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1.5, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs t && ZoneManager.HandZone.Cards.Contains(t.CardThatIsTriggering) && t.CardThatIsTriggering.Rank == Rank.KING && t.isInHandPostScoringTrigger)
                        {
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, t.CardThatIsTriggering);
                        }
                    },
                });
                return ret;
            } },
            { "CLOUD 9", c =>
            {
                var ret = BasicDataBlock("Cloud 9");
                Func<int> nineCount = () => ZoneManager.GetFullDeckCards().Count(x => x.Rank == Rank.NINE);
                ret.DescriptionBuilder = _ => "Earn $1 for each 9 in your full deck at end of round (Currently $" + nineCount() + ")";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.GatherPostRoundMoney,
                    MyAction = args =>
                    {
                        if (args is EngineGatherPostRoundMoneyArgs g)
                            g.JokersContributed.Add((ret, nineCount()));
                    },
                });
                return ret;
            } },
            { "ROCKET", c =>
            {
                var ret = BasicDataBlock("Rocket");
                ret.DescriptionBuilder = _ => "Earn $" + ret.DataDict["MONEYAMOUNT"].IntData + " at end of round. Payout increases by $" + ret.DataDict["MONEYGAIN"].IntData + " when Boss Blind is defeated";
                ret.DataDict.Add("MONEYAMOUNT", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("MONEYGAIN", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.GatherPostRoundMoney, 
                    MyAction = args => 
                    { 
                        if (args is EngineGatherPostRoundMoneyArgs g) 
                            g.JokersContributed.Add((ret, ret.DataDict["MONEYAMOUNT"].IntData)); 
                    } 
                });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.BlindChange, 
                    MyAction = args => 
                    { 
                        if (args is EngineBlindChangeEventArgs b && b.OldBlindType == BlindType.BOSS) 
                            ret.DataDict["MONEYAMOUNT"].IntData += ret.DataDict["MONEYGAIN"].IntData; 
                    } 
                });
                return ret;
            } },
            { "OBELISK", c =>
            {
                var ret = BasicDataBlock("Obelisk");
                ret.DescriptionBuilder = _ => "This Joker gains X" + ret.DataDict["MULTMULTGAIN"].DoubleData + " Mult per consecutive hand played without playing your most played poker hand (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.0") + " Mult)";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 0.2, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.HandPlayedCalculated, 
                    MyAction = args => 
                    { 
                        if (args is EngineHandPlayArgs h) 
                        { 
                            var max = ScoreHandler.HandNumTimesPlayed.Values.Max(); 
                            if (ScoreHandler.HandNumTimesPlayed[h.HandBeingPlayed] == max) 
                                ret.DataDict["MULTMULTAMOUNT"].DoubleData = 1; 
                            else 
                                ret.DataDict["MULTMULTAMOUNT"].DoubleData += ret.DataDict["MULTMULTGAIN"].DoubleData; 
                        } 
                    } 
                });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    } 
                });
                return ret;
            } },
            { "MIDAS MASK", c =>
            {
                var ret = BasicDataBlock("Midas Mask", "All played face cards become Gold cards when scored");
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.HandPlayedCalculated, 
                    MyAction = args => 
                    { 
                        if (args is EngineHandPlayArgs t && t.CardsSelected.Any(x => EngineUtils.isFace(x)))
                        {
                            foreach (var card in t.CardsSelected.Where(x => EngineUtils.isFace(x) && x.Enhancement != Enhancement.GOLD))
                            {
                                card.SetEnhancementOfficial(Enhancement.GOLD);
                            }
                        }
                    } 
                });
                return ret;
            } },
            { "LUCHADOR", c =>
            {
                var ret = BasicDataBlock("Luchador", "Sell this card to disable the current Boss Blind");
                ret.Listeners.Add(new EngineEventListener() 
                { MyContextType = EventContextType.CardSell, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardSoldArgs s && s.CardBeingSold == c && FlowHandler.CurrentSelectedBlind == BlindType.BOSS && Globals.CurrentGameState == GameState.PlayRound) 
                            ZoneManager.HiddenBlindAttributeZone.ClearCards();
                    } 
                });
                return ret;
            } },
            { "PHOTOGRAPH", c =>
            {
                var ret = BasicDataBlock("Photograph");
                ret.DescriptionBuilder = _ => "First played face card gives X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult when scored";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 2, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("TARGETCARDHASH", new JokerData() { IntData = -1, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.AllScoringCardsDecided, 
                    MyAction = args => 
                    { 
                        if (args is EngineHandPlayArgs h) 
                        { 
                            var firstFace = h.CardsInScoringHand.FirstOrDefault(x => EngineUtils.isFace(x)); 
                            ret.DataDict["TARGETCARDHASH"].IntData = firstFace?.GetHashCode() ?? -1; 
                        } 
                    } 
                });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.isScoringTrigger && t.CardThatIsTriggering.GetHashCode() == ret.DataDict["TARGETCARDHASH"].IntData) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, t.CardThatIsTriggering); 
                    } 
                });
                return ret;
            } },
            { "GIFT CARD", c =>
            {
                var ret = BasicDataBlock("Gift Card");
                ret.DescriptionBuilder = _ => "Add $" + ret.DataDict["SELLGAIN"].IntData + " of sell value to every Joker and Consumable card at end of round";
                ret.DataDict.Add("SELLGAIN", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.EndPlayRound, 
                    MyAction = _ => 
                    { 
                        foreach (var card in ZoneManager.JokerZone.Cards) 
                            card.BonusSellValue += ret.DataDict["SELLGAIN"].IntData; 
                        foreach (var card in ZoneManager.ConsumableZone.Cards) 
                            card.BonusSellValue += ret.DataDict["SELLGAIN"].IntData; 
                    } 
                });
                return ret;
            } },
            { "TURTLE BEAN", c =>
            {
                var ret = BasicDataBlock("Turtle Bean");
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["HANDSIZEAMOUNT"].IntData + " hand size, reduces by " + ret.DataDict["LOSSAMOUNT"].IntData + " at end of round";
                ret.DataDict.Add("HANDSIZEAMOUNT", new JokerData() { IntData = 5, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("LOSSAMOUNT", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.OnJokerGainEffs.Add(() => Globals.HandSize += ret.DataDict["HANDSIZEAMOUNT"].IntData);
                ret.OnJokerRemovalEffs.Add(() => Globals.HandSize -= ret.DataDict["HANDSIZEAMOUNT"].IntData);
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.EndPlayRound, 
                    MyAction = _ => 
                    { 
                        
                        ret.DataDict["HANDSIZEAMOUNT"].IntData = Math.Max(0, ret.DataDict["HANDSIZEAMOUNT"].IntData - ret.DataDict["LOSSAMOUNT"].IntData); 
                        Globals.HandSize -= ret.DataDict["LOSSAMOUNT"].IntData;//subtract from handsize and stored bonus
                        if (ret.DataDict["HANDSIZEAMOUNT"].IntData == 0 && ZoneManager.JokerZone.Cards.Contains(c)) 
                            ZoneManager.DestroyCard(c, ZoneManager.JokerZone); 
                    } 
                });
                return ret;
            } },
            { "EROSION", c =>
            {
                var ret = BasicDataBlock("Erosion");
                ret.DataDict.Add("STARTDECKSIZE", new JokerData() { IntData = 52, MyDataType = JokerDataType.INT });//TODO: Set up starting deck size somewhere else.
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 4, MyDataType = JokerDataType.DOUBLE });
                Func<double> multAmt = () => Math.Max(0, (ret.DataDict["STARTDECKSIZE"].IntData - ZoneManager.GetFullDeckCards().Count) * ret.DataDict["MULTAMOUNT"].DoubleData);
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult for each card below " + ret.DataDict["STARTDECKSIZE"].IntData + " in your full deck (Currently +" + multAmt() + " Mult)";
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if(args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger)
                        {
                            var deckSize = ZoneManager.GetFullDeckCards().Count;
                            var mult = multAmt();
                            Globals.EmitMultAdd(mult, c);
                        }
                    }
                });
                return ret;
            } },
            { "RESERVED PARKING", c =>
            {
                var ret = BasicDataBlock("Reserved Parking");
                ret.DataDict.Add("NUMERATOR", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("DENOMINATOR", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("MONEYAMOUNT", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "Each face card held in hand has a " + ret.DataDict["NUMERATOR"].IntData + " in " + ret.DataDict["DENOMINATOR"].IntData + " chance to give $" + ret.DataDict["MONEYAMOUNT"].IntData + " on played hand";
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.HandPlayDone, 
                    MyAction = args => 
                    { 
                        if(args is EngineHandPlayDoneArgs h)
                        { 
                            foreach(var card in h.CardsHeldInHand.Where(EngineUtils.isFace))
                            { 
                                if(Globals.RollRandom(ret.DataDict["NUMERATOR"].IntData, ret.DataDict["DENOMINATOR"].IntData, c)) 
                                    Globals.EmitMoneyGain(ret.DataDict["MONEYAMOUNT"].IntData, c); 
                            } 
                        } 
                    }
                });
                return ret;
            } },
            { "MAIL-IN REBATE", c =>
            {
                var ret = BasicDataBlock("Mail-In Rebate");
                ret.DataDict.Add("MONEYAMOUNT", new JokerData() { IntData = 5, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("TARGETRANK", new JokerData() { SpecificCardRank = Rank.ACE, MyDataType = JokerDataType.RANK });
                ret.DescriptionBuilder = _ => "Earn $" + ret.DataDict["MONEYAMOUNT"].IntData + " for each discarded " + ret.DataDict["TARGETRANK"].SpecificCardRank;
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.HandDiscardDone, 
                    MyAction = args => 
                    { 
                        if(args is EngineDiscardDoneArgs d) 
                            Globals.EmitMoneyGain(d.BeingDiscarded.Count(x => x.Rank == ret.DataDict["TARGETRANK"].SpecificCardRank) * ret.DataDict["MONEYAMOUNT"].IntData, c); 
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.EndPlayRound, 
                    MyAction = _ => 
                    { 
                        var vals = Enum.GetValues<Rank>().Where(x => x != Rank.NONE).ToList(); 
                        ret.DataDict["TARGETRANK"].SpecificCardRank = vals[Globals.randomNext(vals.Count)]; }});
                return ret;
            } },
            { "TO THE MOON", c =>
            {
                var ret = BasicDataBlock("To the Moon", "Earn an extra $1 of interest for every $5 you have at end of round");
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.GatherPostRoundMoney, 
                    MyAction = args => 
                    { 
                        if(args is EngineGatherPostRoundMoneyArgs g && Globals.Money >= 5) 
                            g.JokersContributed.Add((ret, Globals.Money / 5)); 
                    }
                });
                return ret;
            } },
            { "HALLUCINATION", c =>
            {
                var ret = BasicDataBlock("Hallucination");
                ret.DataDict.Add("NUMERATOR", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("DENOMINATOR", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => ret.DataDict["NUMERATOR"].IntData.ToString() + " in " + ret.DataDict["DENOMINATOR"].IntData + " chance to create a Tarot card when any Booster Pack is opened (Must have room)";
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.PackOddsEstablished, 
                    MyAction = args => 
                    { 
                        if(ZoneManager.ConsumableZone.HasRoom && Globals.RollRandom(ret.DataDict["NUMERATOR"].IntData, ret.DataDict["DENOMINATOR"].IntData, c)) 
                            MarketPullManager.DrawMarketItem(BuyItemType.TAROT_CARD, ZoneManager.ConsumableZone, source: GenerationSource.GenericJoker); 
                    }
                });
                            //MarketOptionsManager.DrawMarketItem(BuyItemType.TAROT_CARD, ZoneManager.ConsumableZone); }});
                return ret;
            } },
            { "FORTUNE TELLER", c =>
            {
                var ret = BasicDataBlock("Fortune Teller");
                Func<int> tarotUsed = () => EngineEventHandler.SavedEvents.Count(x => x.MyContext.Context == EventContextType.ConsumableUsed && x is EngineConsumableUseArgs u && u.TypeUsed == ConsumableType.TAROT);
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult per Tarot card used this run (Currently +" + tarotUsed() + ")";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if(args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger) 
                            Globals.EmitMultAdd(tarotUsed() * ret.DataDict["MULTAMOUNT"].DoubleData, c); }});
                return ret;
            } },
            { "JUGGLER", c => { 
                var ret = BasicDataBlock("Juggler"); 
                ret.DataDict.Add("INTAMOUNT", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "Gain +" + ret.DataDict["INTAMOUNT"].IntData + " hand size";
                ret.OnJokerGainEffs.Add(() => Globals.HandSize += ret.DataDict["INTAMOUNT"].IntData); 
                ret.OnJokerRemovalEffs.Add(() => Globals.HandSize -= ret.DataDict["INTAMOUNT"].IntData); 
                return ret; 
            } },
            { "DRUNKARD", c => { 
                var ret = BasicDataBlock("Drunkard"); 
                ret.DataDict.Add("INTAMOUNT", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "Gain +" + ret.DataDict["INTAMOUNT"].IntData + " discard each round";
                ret.OnJokerGainEffs.Add(() => Globals.MaxDiscardsPerRound += ret.DataDict["INTAMOUNT"].IntData); 
                ret.OnJokerRemovalEffs.Add(() => Globals.MaxDiscardsPerRound -= ret.DataDict["INTAMOUNT"].IntData); 
                return ret; 
            } },
            { "STONE JOKER", c =>
            {
                var ret = BasicDataBlock("Stone Joker");
                Func<int> chipAmt = () => ZoneManager.GetFullDeckPlayingCards().Count(x => x.Enhancement == Enhancement.STONE) * ret.DataDict["CHIPAMOUNT"].IntData;
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 25, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "Gives +" + ret.DataDict["CHIPAMOUNT"].IntData + " Chips for each Stone Card in your full deck (Currently +" + chipAmt() + " Chips)";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if(args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger)
                            Globals.EmitChipsAdd(chipAmt(), c);
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
            { "LUCKY CAT", c =>
            {
                var ret = BasicDataBlock("Lucky Cat");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 0.25, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "This Joker gains X" + ret.DataDict["MULTMULTGAIN"].DoubleData + " Mult every time a Lucky card successfully triggers (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.00") + " Mult)";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.LuckyCardSuccessfulTrigger,
                    MyAction = args =>
                    {
                        ret.DataDict["MULTMULTAMOUNT"].DoubleData += ret.DataDict["MULTMULTGAIN"].DoubleData;
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1)
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c);
                    }
                });
                return ret;
            } },
            { "BASEBALL CARD", c =>
            {
                var ret = BasicDataBlock("Baseball Card");
                ret.DescriptionBuilder = _ => "Uncommon Jokers each give X"+ ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1.5, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger && triggerArgs.CardThatIsTriggering.isJoker &&
                            triggerArgs.CardThatIsTriggering != c && triggerArgs.CardThatIsTriggering.JokerData?.Rarity == JokerRarity.UNCOMMON)
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c);
                    }
                });
                return ret;
            } },
            { "BULL", c =>
            {
                var ret = BasicDataBlock("Bull");
                ret.DataDict.Add("CHIPSAMT", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["CHIPSAMT"].IntData + " Chips per $1 you have (Currently +" + (Globals.Money * ret.DataDict["CHIPSAMT"].IntData) + " Chips)";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && Globals.Money > 0)
                            Globals.EmitChipsAdd(Globals.Money * ret.DataDict["CHIPSAMT"].IntData, c);
                    }
                });
                return ret;
            } },
            { "DIET COLA", c =>
            {
                var ret = BasicDataBlock("Diet Cola", "Sell this card to create a free Double Tag");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardSell,
                    MyAction = args =>
                    {
                        if (args is EngineCardSoldArgs soldArgs && soldArgs.CardBeingSold == c)
                            TagDb.AddTagOfType(TagType.DOUBLE_TAG);
                    }
                });
                return ret;
            } },
            { "TRADING CARD", c =>
            {
                var ret = BasicDataBlock("Trading Card", "If first discard of round has only 1 card, destroy it and gain $3");
                ret.DataDict.Add("MONEYAMOUNT", new JokerData() { IntData = 3, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandDiscardDone,
                    MyAction = args =>
                    {
                        if (args is EngineDiscardDoneArgs discardArgs && Globals.CurDiscardsRemaining == Globals.MaxDiscardsPerRound - 1 && discardArgs.BeingDiscarded.Count == 1)
                        {
                            ZoneManager.DestroyCard(discardArgs.BeingDiscarded[0]);
                            Globals.EmitMoneyGain(ret.DataDict["MONEYAMOUNT"].IntData, c);
                        }
                    }
                });
                return ret;
            } },
            { "FLASH CARD", c =>
            {
                var ret = BasicDataBlock("Flash Card");
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 0, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTGAIN", new JokerData() { DoubleData = 2, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "This Joker gains +" + ret.DataDict["MULTGAIN"].DoubleData + " Mult per reroll in the shop (Currently +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult)";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.MoneyGainEmit,
                    MyAction = args =>
                    {
                        if (args is EngineGoldGainEmitArgs moneyArgs && moneyArgs.SourceOfEmit == Globals.RerollButtonCard)
                            ret.DataDict["MULTAMOUNT"].DoubleData += ret.DataDict["MULTGAIN"].DoubleData;
                    }
                });
                ret.Listeners.Add(BuildMultAddListener(c, ret));
                return ret;
            } },
            { "POPCORN", c =>
            {
                var ret = BasicDataBlock("Popcorn");
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 20, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTLOSS", new JokerData() { DoubleData = 4, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult, -" + ret.DataDict["MULTLOSS"].DoubleData + " Mult per round played";
                ret.Listeners.Add(BuildMultAddListener(c, ret));
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.EndPlayRound,
                    MyAction = _ =>
                    {
                        ret.DataDict["MULTAMOUNT"].DoubleData -= ret.DataDict["MULTLOSS"].DoubleData;
                        if (ret.DataDict["MULTAMOUNT"].DoubleData <= 0)
                            ZoneManager.DestroyCard(c, c.MyZone);
                    }
                });
                return ret;
            } },
            { "SPARE TROUSERS", c => //TODO: CHECK WHETHER THIS SHOULD INCREASE BEFORE SCORING FOR THIS HAND OR AFTER. i.e. if you play two pair with this joker, do you get the new +2 bonus for this hand or do you have to wait until next hand?
            //
            {
                var ret = BasicDataBlock("Spare Trousers");
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 0, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTGAIN", new JokerData() { DoubleData = 2, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "This Joker gains +" + ret.DataDict["MULTGAIN"].DoubleData + " Mult if played hand contains a Two Pair (Currently +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult)";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayedCalculated,
                    MyAction = args =>
                    {
                        if (args is EngineHandPlayArgs handArgs && EngineUtils.HandContainsOtherHand(handArgs.HandBeingPlayed, PlayedHandType.TWOPAIR))
                            ret.DataDict["MULTAMOUNT"].DoubleData += ret.DataDict["MULTGAIN"].DoubleData;
                    }
                });
                ret.Listeners.Add(BuildMultAddListener(c, ret));
                return ret;
            } },
            { "ANCIENT JOKER", c =>
            {
                var ret = BasicDataBlock("Ancient Joker");
                var getRandomSuit = new Func<Suit>(() =>
                {
                    var vals = Enum.GetValues<Suit>().Where(x => x != Suit.NONE && x != ret.DataDict["SUIT"].SpecificCardSuit).ToList();
                    return vals[Globals.randomNext(vals.Count)];
                });
                ret.DataDict.Add("SUIT", new JokerData() { SpecificCardSuit = Suit.NONE, MyDataType = JokerDataType.SUIT });
                ret.DataDict["SUIT"].SpecificCardSuit = getRandomSuit();
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1.5, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "Each played card with " + ret.DataDict["SUIT"].SpecificCardSuit + " suit gives X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult when scored, suit changes at end of round";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger && triggerArgs.CardThatIsTriggering.IsSuit(ret.DataDict["SUIT"].SpecificCardSuit))
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c);
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.EndPlayRound,
                    MyAction = _ =>
                    {
                        ret.DataDict["SUIT"].SpecificCardSuit = getRandomSuit();
                    }
                });
                return ret;
            } },
            { "RAMEN", c =>
            {
                var ret = BasicDataBlock("Ramen");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 2, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTLOSS", new JokerData() { DoubleData = 0.01, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.00") + " Mult, loses X" + ret.DataDict["MULTMULTLOSS"].DoubleData.ToString("0.00") + " Mult per card discarded";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandDiscardDone,
                    MyAction = args =>
                    {
                        if (args is EngineDiscardDoneArgs discardArgs)
                        {
                            //TODO: Trigger on each card instead of all at once at end of discard.
                            ret.DataDict["MULTMULTAMOUNT"].DoubleData -= discardArgs.BeingDiscarded.Count * ret.DataDict["MULTMULTLOSS"].DoubleData;
                            if (ret.DataDict["MULTMULTAMOUNT"].DoubleData <= 1)
                                ZoneManager.DestroyCard(c, c.MyZone);
                        }
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs triggerArgs && triggerArgs.CardThatIsTriggering == c && triggerArgs.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1)
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c);
                    }
                });
                return ret;
            } },
            { "WALKIE TALKIE", c =>
            {
                var ret = BasicDataBlock("Walkie Talkie");
                ret.DescriptionBuilder = _ => "Gives +" + ret.DataDict["CHIPAMOUNT"].IntData + " Chips and +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when you play a Ten or Four";
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 10, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 4, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs t && t.isScoringTrigger && ZoneManager.CurrentlyBeingPlayedZone.Cards.Contains(t.CardThatIsTriggering) && (t.CardThatIsTriggering.Rank == Rank.TEN || t.CardThatIsTriggering.Rank == Rank.FOUR))
                        {
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c);
                        }
                    }
                });
                return ret;
            } },
            { "SELTZER", c =>
            {
                var ret = BasicDataBlock("Seltzer");
                ret.DataDict.Add("HANDSLEFT", new JokerData() { IntData = 10, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "Retrigger all cards played for the next " + ret.DataDict["HANDSLEFT"].IntData + " hands";
                ret.Listeners.Add(new EngineEventListener()
                { MyContextType = EventContextType.CardPreTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardPreTriggerArgs t && ZoneManager.CurrentlyBeingPlayedZone.Cards.Contains(t.CardAboutToTrigger)) 
                            t.numTriggersToDo += 1; 
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.HandPlayDone, 
                    MyAction = _ => 
                    { 
                        ret.DataDict["HANDSLEFT"].IntData -= 1; 
                        if (ret.DataDict["HANDSLEFT"].IntData <= 0 && c.MyZone == ZoneManager.JokerZone) 
                            ZoneManager.DestroyCard(c, ZoneManager.JokerZone); 
                    }
                });
                return ret;
            } },
            { "CASTLE", c =>
            {
                var ret = BasicDataBlock("Castle");
                var getRandomSuit = new Func<Suit>(() =>
                {
                    var vals = Enum.GetValues<Suit>().Where(x => x != Suit.NONE && x != ret.DataDict["SUIT"].SpecificCardSuit).ToList();
                    return vals[Globals.randomNext(vals.Count)];
                });
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 0, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("CHIPGAIN", new JokerData() { IntData = 3, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("SUIT", new JokerData() { SpecificCardSuit = Suit.NONE, MyDataType = JokerDataType.SUIT });
                ret.DataDict["SUIT"].SpecificCardSuit = getRandomSuit();
                ret.DescriptionBuilder = _ => "This Joker gains +3 Chips per discarded " + ret.DataDict["SUIT"].SpecificCardSuit + " card, suit changes every round (Currently +" + ret.DataDict["CHIPAMOUNT"].IntData + " chips)";
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.HandDiscardDone, 
                    MyAction = args => 
                    { 
                        if (args is EngineDiscardDoneArgs d && d.BeingDiscarded.Any(x => x.IsSuit(ret.DataDict["SUIT"].SpecificCardSuit))) //TODO: Trigger on each discarded instead of all at once.
                            ret.DataDict["CHIPAMOUNT"].IntData += d.BeingDiscarded.Count(x => x.IsSuit(ret.DataDict["SUIT"].SpecificCardSuit)) * ret.DataDict["CHIPGAIN"].IntData; 
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.EndPlayRound, 
                    MyAction = _ => 
                    {
                        ret.DataDict["SUIT"].SpecificCardSuit = getRandomSuit();
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ret.DataDict["CHIPAMOUNT"].IntData > 0) 
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c); 
                    }
                });
                return ret;
            } },
            { "SMILEY FACE", c =>
            {
                var ret = BasicDataBlock("Smiley Face");
                ret.DescriptionBuilder = _ => "Gain +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult for each face card played";
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 5, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if(args is EngineCardTriggerArgs t && t.isScoringTrigger && EngineUtils.isFace(t.CardThatIsTriggering)) 
                            Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, c); 
                    }
                });
                return ret;
            } },
            { "CAMPFIRE", c =>
            {
                var ret = BasicDataBlock("Campfire");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 0.25, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "This Joker gains X0.25 Mult for each card sold, resets when Boss Blind is defeated (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.00") + " Mult)";
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardSell, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardSoldArgs s && s.CardBeingSold != c) 
                            ret.DataDict["MULTMULTAMOUNT"].DoubleData += ret.DataDict["MULTMULTGAIN"].DoubleData; 
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.BlindChange, 
                    MyAction = args => 
                    { 
                        if (args is EngineBlindChangeEventArgs b && b.OldBlindType == BlindType.BOSS) 
                            ret.DataDict["MULTMULTAMOUNT"].DoubleData = 1; 
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1)
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    }
                });
                return ret;
            } },
            { "GOLDEN TICKET", c =>
            {
                var ret = BasicDataBlock("Golden Ticket");
                ret.DescriptionBuilder = _ => "Gain $" + ret.DataDict["INTAMOUNT"].IntData + " for each scoring trigger from a GOLD enhanced card";
                ret.DataDict.Add("INTAMOUNT", new JokerData() { IntData = 4, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if(args is EngineCardTriggerArgs t && t.isScoringTrigger && t.CardThatIsTriggering.Enhancement == Enhancement.GOLD) 
                            Globals.EmitMoneyGain(ret.DataDict["INTAMOUNT"].IntData, c); 
                    }
                });
                return ret;
            } },
            { "MR. BONES", c =>
            {
                var ret = BasicDataBlock("Mr. Bones", "Prevents Death if chips scored are at least 25% of required chips, then self-destructs");
                ret.Listeners.Add(new EngineEventListener(){ 
                    MyContextType = EventContextType.HandPlayDone, 
                    MyAction = args => 
                    {
                        if (args is EngineHandPlayDoneArgs h && Globals.CurHandsRemaining == 0 && h.CurrentTotalChips < h.RequiredChipsForBlind && h.RequiredChipsForBlind > 0 && h.CurrentTotalChips >= (h.RequiredChipsForBlind * 0.25))
                        {
                            h.PreventGameOverAndWinBlind = true;
                            if (c.MyZone == ZoneManager.JokerZone) 
                                ZoneManager.DestroyCard(c, ZoneManager.JokerZone);
                        }
                    }
                });
                return ret;
            } },
            { "ACROBAT", c =>
            {
                var ret = BasicDataBlock("Acrobat");
                ret.DescriptionBuilder = _ => "Gains X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult if it's the last card to trigger for the hand and it's a scoring trigger";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if(args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && Globals.CurHandsRemaining == 1) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    }
                });
                return ret;
            } },
            { "SOCK AND BUSKIN", c =>
            {
                var ret = BasicDataBlock("Sock and Buskin", "Retrigger all played face cards an additional time");
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardPreTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardPreTriggerArgs t && ZoneManager.CurrentlyBeingPlayedZone.Cards.Contains(t.CardAboutToTrigger) && EngineUtils.isFace(t.CardAboutToTrigger)) 
                            t.numTriggersToDo += 1; 
                    }
                });
                return ret;
            } },
            { "SWASHBUCKLER", c =>
            {
                var ret = BasicDataBlock("Swashbuckler");
                Func<int> amt = () => ZoneManager.JokerZone.Cards.Where(x => x != c).Sum(x => x.SellCost);
                ret.DescriptionBuilder = _ => "Adds the sell value of all other owned Jokers to Mult (Currently +" + amt() + " Mult)";
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && amt() > 0) 
                            Globals.EmitMultAdd(amt(), c); 
                    }
                });
                return ret;
            } },
            { "TROUBADOUR", c => {
                var ret = BasicDataBlock("Troubadour");
                ret.DataDict.Add("HANDSAMOUNT", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("HANDSIZEAMOUNT", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["HANDSIZEAMOUNT"].IntData + " Hand size, -" + ret.DataDict["HANDSAMOUNT"].IntData + " hand per round";
                ret.OnJokerGainEffs.Add(() =>
                {
                    Globals.HandSize += ret.DataDict["HANDSIZEAMOUNT"].IntData;
                    Globals.MaxHandsPerRound -= ret.DataDict["HANDSAMOUNT"].IntData;
                });
                ret.OnJokerRemovalEffs.Add(() =>
                {
                    Globals.HandSize -= ret.DataDict["HANDSIZEAMOUNT"].IntData;
                    Globals.MaxHandsPerRound += ret.DataDict["HANDSAMOUNT"].IntData;
                });
                return ret;
            } },
            { "CERTIFICATE", c => {
                var ret = BasicDataBlock("Certificate", "When round begins, add a random playing card with a random seal to your hand");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.StartPlayRoundSetupOver,
                    MyAction = _ =>
                    {
                        var created = EngineUtils.GenerateRandomPlayingCard();
                        var possibleSeals = Enum.GetValues<Seal>().Where(x => x != Seal.NONE).ToList();
                        created.Seal = possibleSeals[Globals.randomNext(possibleSeals.Count)];
                        ZoneManager.HandZone.AddCard(created, overrideSpace: true);
                    }
                });
                return ret;
            } },
            { "SMEARED JOKER", c => {
                var ret = BasicDataBlock("Smeared Joker", "Hearts and Diamonds count as the same suit. Spades and Clubs count as the same suit.");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardSuitPull,
                    MyAction = args =>
                    {
                        if (args is EngineCardSuitPullArgs suitArgs && suitArgs.CardBeingPulled != null)
                        {
                            if (suitArgs.SuitsBeingReturned.Contains(Suit.HEARTS) ^ suitArgs.SuitsBeingReturned.Contains(Suit.DIAMONDS))
                            {
                                suitArgs.SuitsBeingReturned.Add(suitArgs.SuitsBeingReturned.Contains(Suit.HEARTS) ? Suit.DIAMONDS : Suit.HEARTS);
                            }
                            if (suitArgs.SuitsBeingReturned.Contains(Suit.SPADES) ^ suitArgs.SuitsBeingReturned.Contains(Suit.CLUBS))
                            {
                                suitArgs.SuitsBeingReturned.Add(suitArgs.SuitsBeingReturned.Contains(Suit.SPADES) ? Suit.CLUBS : Suit.SPADES);
                            }
                        }
                    }
                });
                return ret;
            } },
            { "THROWBACK", c => {
                var ret = BasicDataBlock("Throwback");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 0.25, MyDataType = JokerDataType.DOUBLE });
                Func<double> curMultMult = () => ret.DataDict["MULTMULTAMOUNT"].DoubleData + (EngineEventHandler.CountOfSaved(EventContextType.BlindSkip) * ret.DataDict["MULTMULTGAIN"].DoubleData);
                ret.DescriptionBuilder = _ => "X" + ret.DataDict["MULTMULTGAIN"].DoubleData + " Mult for each Blind skipped this run (Currently X" + curMultMult().ToString("0.00") + " Mult)";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger)
                            Globals.EmitMultMult(curMultMult(), c);
                    }
                });
                return ret;
            } },
            { "HANGING CHAD", c => {
                var ret = BasicDataBlock("Hanging Chad", "Retrigger first played card used in scoring 2 additional times");
                ret.DataDict.Add("EXTRATRIGGERS", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("READYFORFIRSTTRIGGER", new JokerData() { IntData = 0, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.HandPlayedCalculated,
                    MyAction = _ =>
                    {
                        ret.DataDict["READYFORFIRSTTRIGGER"].IntData = 1;
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardPreTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardPreTriggerArgs p && p.isScoringPreTrigger && ZoneManager.CurrentlyBeingPlayedZone.Cards.Contains(p.CardAboutToTrigger) && ret.DataDict["READYFORFIRSTTRIGGER"].IntData == 1)
                        {
                            p.numTriggersToDo += ret.DataDict["EXTRATRIGGERS"].IntData;
                            ret.DataDict["READYFORFIRSTTRIGGER"].IntData = 0;
                        }
                    }
                });
                return ret;
            } },
            { "ROUGH GEM", c => BuildSuitScoringBonusJoker("Rough Gem", Suit.DIAMONDS, moneyAmount: 1) },
            { "BLOODSTONE", c =>
            {
                var ret = BuildSuitScoringBonusJoker("Bloodstone", Suit.HEARTS, multMultAmount: 1.5, randomRollNumerator: 1, randomRollDenominator: 2);
                ret.DescriptionBuilder = _ => ret.DataDict["NUMERATOR"].IntData + " in " + ret.DataDict["DENOMINATOR"].IntData + " chance for played cards with " + ret.DataDict["SUIT"].SpecificCardSuit + " suit to give X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult when scored";
                return ret;
            } },
            { "ARROWHEAD", c => BuildSuitScoringBonusJoker("Arrowhead", Suit.SPADES, chipAmount: 50) },
            { "ONYX AGATE", c => BuildSuitScoringBonusJoker("Onyx Agate", Suit.CLUBS, multAmount: 7) },
            { "GLASS JOKER", c =>
            {
                var ret = BasicDataBlock("Glass Joker");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 0.75, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "This Joker gains X" + ret.DataDict["MULTMULTGAIN"].DoubleData + " Mult for every Glass card that is destroyed (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.00") + " Mult)";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardDestroyed,
                    MyAction = args =>
                    {
                        if (args is EngineCardDestroyedArgs d && d.CardDestroyed.Enhancement == Enhancement.GLASS)
                            ret.DataDict["MULTMULTAMOUNT"].DoubleData += ret.DataDict["MULTMULTGAIN"].DoubleData;
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1)
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c);
                    }
                });
                return ret;
            } },
            { "SHOWMAN", c => BasicDataBlock("Showman", "Copies of Jokers, Tarots, Planets, and Spectrals can appear.") },
            { "FLOWER POT", c =>
            {
                var ret = BasicDataBlock("Flower Pot");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("DOTRIGGER", new JokerData() { IntData = 0, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult if poker hand contains all 4 suits";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ret.DataDict["DOTRIGGER"].IntData == 1)
                        {
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c);
                            ret.DataDict["DOTRIGGER"].IntData = 0;
                        }
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.AllScoringCardsDecided,
                    MyAction = args =>
                    {
                        if (args is EngineHandPlayArgs t && t.CardsInScoringHand.Any(x => x.IsSuit(Suit.CLUBS)) && t.CardsInScoringHand.Any(x => x.IsSuit(Suit.SPADES)) && t.CardsInScoringHand.Any(x => x.IsSuit(Suit.HEARTS)) && t.CardsInScoringHand.Any(x => x.IsSuit(Suit.DIAMONDS)))
                        {
                            ret.DataDict["DOTRIGGER"].IntData = 1;
                        }
                    }
                });
                return ret;
            } },
            { "BLUEPRINT", c => BuildCopyJoker("Blueprint", c, GetJokerRightOfCard, "Joker to the right") },
            { "WEE JOKER", c =>
            {
                var ret = BasicDataBlock("Wee Joker");
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 0, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("CHIPGAIN", new JokerData() { IntData = 8, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "This Joker gains +" + ret.DataDict["CHIPGAIN"].IntData + " Chips when each played 2 is scored (Currently +" + ret.DataDict["CHIPAMOUNT"].IntData + " Chips)";
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if(args is EngineCardTriggerArgs t && t.isScoringTrigger && t.CardThatIsTriggering.Rank == Rank.TWO) 
                            ret.DataDict["CHIPAMOUNT"].IntData += ret.DataDict["CHIPGAIN"].IntData; 
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if(args is EngineCardTriggerArgs t && t.isScoringTrigger && t.CardThatIsTriggering == c && ret.DataDict["CHIPAMOUNT"].IntData > 0) 
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c); 
                    }
                });
                return ret;
            } },
            { "MERRY ANDY", c => {
                var ret = BasicDataBlock("Merry Andy");
                ret.DescriptionBuilder = _ => "+"+ ret.DataDict["DISCARDAMOUNT"].IntData +" discards each round, "+ ret.DataDict["HANDSIZEAMOUNT"].IntData +" hand size";
                ret.DataDict.Add("DISCARDAMOUNT", new JokerData() { IntData = 3, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("HANDSIZEAMOUNT", new JokerData() { IntData = -1, MyDataType = JokerDataType.INT });
                ret.OnJokerGainEffs.Add(() => 
                { 
                    Globals.MaxDiscardsPerRound += ret.DataDict["DISCARDAMOUNT"].IntData; 
                    Globals.HandSize += ret.DataDict["HANDSIZEAMOUNT"].IntData; 
                });
                ret.OnJokerRemovalEffs.Add(() => 
                { 
                    Globals.MaxDiscardsPerRound -= ret.DataDict["DISCARDAMOUNT"].IntData; 
                    Globals.HandSize -= ret.DataDict["HANDSIZEAMOUNT"].IntData; 
                });
                return ret;
            } },
            { "OOPS! ALL 6S", c => {
                var ret = BasicDataBlock("Oops! All 6s", "Doubles all listed probabilities");
                ret.Listeners.Add(new EngineEventListener()
                { //TO-DO: SHOULD MODIFY DISPLAYED ODDS IN JOKER DESC AS WELL.
                    //MAYBE ONLY MODIFY JOKER VALUES, NOT ADD ROLL LISTENER?
                    //ONLY PROBLEM THERE IS LUCKY CARDS: MAKE THAT BASICALLY A JOKER LISTENER?
                    MyContextType = EventContextType.RandomRollHappening, 
                    MyAction = args => 
                    { 
                        if(args is EngineRandomRollArgs p)
                        { 
                            p.Numerator *= 2;
                        } 
                    }
                });
                return ret;
            } },
            { "THE IDOL", c => {
                var ret = BasicDataBlock("The Idol");
                ret.DataDict.Add("TARGETRANK", new JokerData() { SpecificCardRank = Rank.NONE, MyDataType = JokerDataType.RANK });
                ret.DataDict.Add("TARGETSUIT", new JokerData() { SpecificCardSuit = Suit.NONE, MyDataType = JokerDataType.SUIT });
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 2, MyDataType = JokerDataType.DOUBLE });
                Action reroll = () => 
                { 
                    var options = ZoneManager.GetFullDeckPlayingCards().GroupBy(x => (x.Rank, x.Suit)).Select(x => x.Key).ToList(); 
                    if(options.Count == 0) 
                        return; 
                    var pick = options[Globals.randomNext(options.Count)]; 
                    ret.DataDict["TARGETRANK"].SpecificCardRank = pick.Rank; 
                    ret.DataDict["TARGETSUIT"].SpecificCardSuit = pick.Suit; 
                };
                reroll();
                ret.DescriptionBuilder = _ => "Each played " + ret.DataDict["TARGETRANK"].SpecificCardRank + " of " + ret.DataDict["TARGETSUIT"].SpecificCardSuit + " gives X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult when scored, card changes every round";
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.EndPlayRound, 
                    MyAction = _ => reroll() 
                });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if(args is EngineCardTriggerArgs t && t.isScoringTrigger && t.CardThatIsTriggering.Rank == ret.DataDict["TARGETRANK"].SpecificCardRank && t.CardThatIsTriggering.IsSuit(ret.DataDict["TARGETSUIT"].SpecificCardSuit)) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    }
                });
                return ret;
            } },
            { "SEEING DOUBLE", c => {
                var ret = BasicDataBlock("Seeing Double");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 2, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult if played hand has a scoring Club card and a scoring card of any other suit";
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if(args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger)
                        { 
                            var cards=ZoneManager.CurrentlyBeingPlayedZone.Cards; 
                            if(cards.Any(x=>x.IsSuit(Suit.CLUBS)) && cards.Any(x=>!x.IsSuit(Suit.CLUBS))) 
                                Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                        } 
                    }
                });
                return ret;
            } },
            { "MATADOR", c => {
                var ret = BasicDataBlock("Matador");
                ret.DataDict.Add("MONEYAMOUNT", new JokerData() { IntData = 8, MyDataType = JokerDataType.INT }); 
                ret.DescriptionBuilder = _ => "Earn $" + ret.DataDict["MONEYAMOUNT"].IntData + " if played hand triggers the Boss Blind ability";
                ret.Listeners.Add(new EngineEventListener()
                {
                    //TODO: Make darn well sure you implement this event triggering in the relevant boss blinds.
                    MyContextType = EventContextType.BossAbilityTriggeredByHand, 
                    MyAction = args => 
                    {
                        Globals.EmitMoneyGain(ret.DataDict["MONEYAMOUNT"].IntData, c);
                    }
                });
                return ret;
                } },
            { "HIT THE ROAD", c => {
                var ret = BasicDataBlock("Hit the Road");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 0.5, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "This Joker gains X" + ret.DataDict["MULTMULTGAIN"].DoubleData + " Mult for every Jack discarded this round (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData.ToString("0.00") + " Mult)";
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.StartPlayRound, 
                    MyAction = _ => ret.DataDict["MULTMULTAMOUNT"].DoubleData = 1 
                });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.HandDiscardDone, 
                    MyAction = args =>
                    { //TODO: Trigger on each discarded instead of all at once, also only for discards from hand in play round.
                        if(args is EngineDiscardDoneArgs d) 
                            ret.DataDict["MULTMULTAMOUNT"].DoubleData += d.BeingDiscarded.Count(x => x.Rank == Rank.JACK) * ret.DataDict["MULTMULTGAIN"].DoubleData; 
                    }
                });
                ret.Listeners.Add(new EngineEventListener()
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if(args is EngineCardTriggerArgs t && t.isScoringTrigger && t.CardThatIsTriggering == c && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    }
                });
                return ret;
            } },
            { "THE DUO", c => BuildHandTypeMultMultJoker("The Duo", PlayedHandType.PAIR, 2, c) },
            { "THE TRIO", c => BuildHandTypeMultMultJoker("The Trio", PlayedHandType.THREEOFAKIND, 3, c) },
            { "THE FAMILY", c => BuildHandTypeMultMultJoker("The Family", PlayedHandType.FOUROFAKIND, 4, c) },
            { "THE ORDER", c => BuildHandTypeMultMultJoker("The Order", PlayedHandType.STRAIGHT, 3, c) },
            { "THE TRIBE", c => BuildHandTypeMultMultJoker("The Tribe", PlayedHandType.FLUSH, 2, c) },
            { "STUNTMAN", c =>
            {
                var ret = BasicDataBlock("Stuntman");
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = 250, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("HANDSIZEAMOUNT", new JokerData() { IntData = -2, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["CHIPAMOUNT"].IntData + " Chips, " + ret.DataDict["HANDSIZEAMOUNT"].IntData + " hand size";
                ret.OnJokerGainEffs.Add(() => Globals.HandSize += ret.DataDict["HANDSIZEAMOUNT"].IntData);
                ret.OnJokerRemovalEffs.Add(() => Globals.HandSize -= ret.DataDict["HANDSIZEAMOUNT"].IntData);
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger)
                            Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, c);
                    }
                });
                return ret;
            } },
            { "INVISIBLE JOKER", c =>
            {
                var ret = BasicDataBlock("Invisible Joker");
                ret.DataDict.Add("ROUNDSREMAINING", new JokerData() { IntData = 2, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "After " + ret.DataDict["ROUNDSREMAINING"].IntData + " rounds, sell this card to duplicate a random joker";
                ret.Listeners.Add(new EngineEventListener() { MyContextType = EventContextType.EndPlayRound, MyAction = _ => ret.DataDict["ROUNDSREMAINING"].IntData = Math.Max(0, ret.DataDict["ROUNDSREMAINING"].IntData - 1)});
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardSell,
                    MyAction = args =>
                    {
                        if (args is EngineCardSoldArgs s && s.CardBeingSold == c && ret.DataDict["ROUNDSREMAINING"].IntData <= 0)
                        {
                            var choices = ZoneManager.JokerZone.Cards.Where(x => x != c).ToList();
                            if (!choices.Any())
                                return;
                            var toCopy = choices[Globals.randomNext(choices.Count)];
                            var copied = toCopy.MakeCopy();
                            if (copied.Edition == Edition.NEGATIVE)
                                copied.SetEditionOfficial(Edition.BASE);
                            ZoneManager.JokerZone.AddCard(copied, overrideSpace: true);
                        }
                    }
                });
                return ret;
            } },
            { "BRAINSTORM", c => BuildCopyJoker("Brainstorm", c, GetLeftmostJoker, "leftmost Joker") },
            { "SATELLITE", c =>
            {
                var ret = BasicDataBlock("Satellite");
                Func<int> amt = () => EngineEventHandler.SavedEvents
                    .Where(x => x.MyContext.Context == EventContextType.ConsumableUsed)
                    .OfType<EngineConsumableUseArgs>()
                    .Where(x => x.TypeUsed == ConsumableType.PLANET)
                    .Select(x => x.ConsumableDBName)
                    .Distinct()
                    .Count();
                ret.DescriptionBuilder = _ => "Earn $1 at end of round per unique Planet card used this run (" + amt() + " total)";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.GatherPostRoundMoney,
                    MyAction = args => { if (args is EngineGatherPostRoundMoneyArgs g) g.JokersContributed.Add((ret, amt())); }
                });
                return ret;
            } },
            { "SHOOT THE MOON", c =>
            {
                var ret = BasicDataBlock("Shoot the Moon");
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = 13, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "Each Queen held in hand gives +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult";
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardTrigger,
                    MyAction = args =>
                    {//TODO: Trigger individually per card instead of all at once en-mass here. 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ZoneManager.HandZone.Cards.Any(x => !x.isSelected && x.Rank == Rank.QUEEN))
                            Globals.EmitMultAdd(ZoneManager.HandZone.Cards.Count(x => !x.isSelected && x.Rank == Rank.QUEEN) * ret.DataDict["MULTAMOUNT"].DoubleData, c);
                    }
                });
                return ret;
            } },
            { "DRIVER'S LICENSE", c =>
            {
                var ret = BasicDataBlock("Driver's License");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 3, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("INTAMOUNT", new JokerData() { IntData = 16, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult if you have at least " + ret.DataDict["INTAMOUNT"].IntData + " Enhanced cards in your full deck (Currently " + ZoneManager.GetFullDeckPlayingCards().Count(x => x.Enhancement != Enhancement.NONE) + ")";
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ZoneManager.GetFullDeckPlayingCards().Count(x => x.Enhancement != Enhancement.NONE) >= ret.DataDict["INTAMOUNT"].IntData) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    } 
                });
                return ret;
            } },
            { "CARTOMANCER", c =>
            {
                var ret = BasicDataBlock("Cartomancer", "Create a Tarot card when Blind is selected (Must have room)");
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.StartSelectedBlind, 
                    MyAction = _ => 
                    {
                        if (ZoneManager.ConsumableZone.HasRoom)
                            MarketPullManager.DrawMarketItem(BuyItemType.TAROT_CARD, ZoneManager.ConsumableZone, source: GenerationSource.GenericJoker); 
                            //MarketOptionsManager.DrawMarketItem(BuyItemType.TAROT_CARD, ZoneManager.ConsumableZone); 
                    } 
                });
                return ret;
            } },
            { "ASTRONOMER", c =>
            {
                var ret = BasicDataBlock("Astronomer", "All Planet cards and Celestial Packs in the shop are free");
                //TODO: I think this implementation is ok???
                //Prob be better to trigger an event when a card is added to shop, then modify cost, but idk.
                Action applyCosts = () =>
                {
                    foreach (var card in ZoneManager.MainMarketZone.Cards.Where(x => x.isConsumable && x.ConsumableData.Type == ConsumableType.PLANET))
                        card.BuyCostOverride = 0;
                    foreach (var card in ZoneManager.PackMarketZone.Cards.Where(x => x.MyPackType == PackType.BASIC_PLANET || x.MyPackType == PackType.JUMBO_PLANET || x.MyPackType == PackType.MEGA_PLANET))
                        card.BuyCostOverride = 0;
                };
                ret.Listeners.Add(new EngineEventListener() { MyContextType = EventContextType.StartMarket, MyAction = _ => applyCosts() });
                ret.Listeners.Add(new EngineEventListener() { MyContextType = EventContextType.Reroll, MyAction = _ => applyCosts() });
                ret.OnJokerRemovalEffs.Add(() =>
                {
                    foreach (var card in ZoneManager.MainMarketZone.Cards.Where(x => x.isConsumable && x.ConsumableData.Type == ConsumableType.PLANET))
                        card.BuyCostOverride = null;
                    foreach (var card in ZoneManager.PackMarketZone.Cards.Where(x => x.MyPackType == PackType.BASIC_PLANET || x.MyPackType == PackType.JUMBO_PLANET || x.MyPackType == PackType.MEGA_PLANET))
                        card.BuyCostOverride = null;
                });
                return ret;
            } },
            { "BURNT JOKER", c =>
            {
                var ret = BasicDataBlock("Burnt Joker", "Upgrade the level of the first discarded poker hand each round");
                ret.DataDict.Add("READY", new JokerData() { IntData = 1, MyDataType = JokerDataType.INT });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.StartPlayRound, 
                    MyAction = _ => ret.DataDict["READY"].IntData = 1 
                });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.HandDiscardDone, 
                    MyAction = args => 
                    { 
                        if (args is EngineDiscardDoneArgs d && ret.DataDict["READY"].IntData == 1 && d.BeingDiscarded.Any()) 
                        { 
                            ScoreHandler.LevelUpHand(EngineUtils.BestHandFromCards(d.BeingDiscarded).Item1); 
                            ret.DataDict["READY"].IntData = 0; 
                        } 
                    } 
                });
                return ret;
            } },
            { "BOOTSTRAPS", c =>
            {
                var ret = BasicDataBlock("Bootstraps");
                ret.DataDict.Add("MULTAMOUNTPER", new JokerData() { DoubleData = 2, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MONEYSTEP", new JokerData() { IntData = 5, MyDataType = JokerDataType.INT });
                var getAmt = new Func<double>(() => (int)(Globals.Money / ret.DataDict["MONEYSTEP"].IntData) * ret.DataDict["MULTAMOUNTPER"].DoubleData);
                ret.DescriptionBuilder = _ => "+" + ret.DataDict["MULTAMOUNTPER"].DoubleData + " Mult for every $" + ret.DataDict["MONEYSTEP"].IntData + " you have (Currently +" + getAmt() + " Mult)";
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger) 
                            Globals.EmitMultAdd(getAmt(), c); 
                    } 
                });
                return ret;
            } },
            { "CANIO", c =>
            {
                var ret = BasicDataBlock("Canio");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DescriptionBuilder = _ => "This Joker gains X" + ret.DataDict["MULTMULTGAIN"].DoubleData + " Mult when a face card is destroyed (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult)";
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardDestroyed, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardDestroyedArgs d && EngineUtils.isFace(d.CardDestroyed)) 
                            ret.DataDict["MULTMULTAMOUNT"].DoubleData += ret.DataDict["MULTMULTGAIN"].DoubleData; 
                    } 
                });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    } 
                });
                return ret;
            } },
            { "TRIBOULET", c =>
            {
                var ret = BasicDataBlock("Triboulet");
                ret.DescriptionBuilder = _ => "Played Kings and Queens each give X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult when scored";
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 2, MyDataType = JokerDataType.DOUBLE });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.isScoringTrigger && (t.CardThatIsTriggering.Rank == Rank.KING || t.CardThatIsTriggering.Rank == Rank.QUEEN)) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    } 
                });
                return ret;
            } },
            { "YORICK", c =>
            {
                var ret = BasicDataBlock("Yorick");
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("MULTMULTGAIN", new JokerData() { DoubleData = 1, MyDataType = JokerDataType.DOUBLE });
                ret.DataDict.Add("REMAINING", new JokerData() { IntData = 23, MyDataType = JokerDataType.INT });
                ret.DescriptionBuilder = _ => "This Joker gains X" + ret.DataDict["MULTMULTGAIN"].DoubleData + " Mult every 23 [remaining: " + ret.DataDict["REMAINING"].IntData + "] cards discarded (Currently X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult)";
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardDiscardedFromHand, 
                    MyAction = _ => 
                    { 
                        ret.DataDict["REMAINING"].IntData--; 
                        if (ret.DataDict["REMAINING"].IntData <= 0) 
                        { 
                            ret.DataDict["MULTMULTAMOUNT"].DoubleData += ret.DataDict["MULTMULTGAIN"].DoubleData;
                            ret.DataDict["REMAINING"].IntData = 23; 
                        } 
                    } 
                });
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.CardTrigger, 
                    MyAction = args => 
                    { 
                        if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && ret.DataDict["MULTMULTAMOUNT"].DoubleData > 1) 
                            Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c); 
                    } 
                });
                return ret;
            } },
            { "CHICOT", c =>
            {
                var ret = BasicDataBlock("Chicot", "Disables effect of every Boss Blind");
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.StartPlayRoundSetupOver, 
                    MyAction = _ => 
                    { 
                        if (FlowHandler.CurrentSelectedBlind == BlindType.BOSS) 
                            ZoneManager.HiddenBlindAttributeZone.ClearCards(); //SEE REMOVE STUFF FOR TROUBADOR ABOVE
                    } 
                });
                return ret;
            } },
            { "PERKEO", c =>
            {
                var ret = BasicDataBlock("Perkeo", "Creates a Negative copy of 1 random consumable card in your possession at the end of the shop");
                ret.Listeners.Add(new EngineEventListener() 
                { 
                    MyContextType = EventContextType.EndMarket, 
                    MyAction = _ => 
                    { 
                        if (!ZoneManager.ConsumableZone.HasRoom || !ZoneManager.ConsumableZone.Cards.Any()) 
                            return; 
                        var chosen = ZoneManager.ConsumableZone.Cards[Globals.randomNext(ZoneManager.ConsumableZone.Cards.Count)]; 
                        var copy = chosen.MakeCopy(); 
                        copy.SetEditionOfficial(Edition.NEGATIVE); 
                        ZoneManager.ConsumableZone.AddCard(copy, overrideSpace: true); 
                    } 
                });
                return ret;
            } },

        };

        public static string GetRandomJokerOfRarity(JokerRarity rarity)
        {
            var jokersOfRarity = JokerMetadata.Where(x => x.Value.Rarity == rarity).ToList();
            if (!jokersOfRarity.Any())
                return null;
            var randomIndex = Globals.randomNext(jokersOfRarity.Count);
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

        public static JokerCardDataBlock BasicDataBlock(string name)
        {
            var ret = new JokerCardDataBlock();
            ret.JokerName = name;
            ret.DBName = name.ToUpper();
            return ret;
        }

        public static JokerCardDataBlock BasicDataBlock(string name, string desc)
        {
            var ret = BasicDataBlock(name);
            ret.DescriptionBuilder = _ => desc;
            return ret;
        }

        public static JokerCardDataBlock BasicDataBlock(string name, Func<EventContext, string> descBuilder)
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

        private static JokerCardDataBlock BuildHandTypeMultMultJoker(string name, PlayedHandType handType, double multMultAmount, Card c)
        {
            var ret = BasicDataBlock(name);
            ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = multMultAmount, MyDataType = JokerDataType.DOUBLE });
            ret.DataDict.Add("PLAYEDHAND", new JokerData() { HandTypeData = handType, MyDataType = JokerDataType.HANDTYPE });
            ret.DescriptionBuilder = _ => "X" + ret.DataDict["MULTMULTAMOUNT"].DoubleData + " Mult if played hand contains a " + ret.DataDict["PLAYEDHAND"].HandTypeData.ToString();
            ret.Listeners.Add(new EngineEventListener()
            {
                MyContextType = EventContextType.CardTrigger,
                MyAction = args =>
                {
                    if (args is EngineCardTriggerArgs t && t.CardThatIsTriggering == c && t.isScoringTrigger && EngineUtils.HandContainsOtherHand(t.HandCurrentlyBeingPlayed, ret.DataDict["PLAYEDHAND"].HandTypeData))
                        Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, c);
                }
            });
            return ret;
        }


        private static JokerCardDataBlock BuildSuitScoringBonusJoker(string name, Suit suit, int moneyAmount = 0, int chipAmount = 0, double multAmount = 0, double multMultAmount = 0, int randomRollNumerator = 0, int randomRollDenominator = 0)
        {
            var ret = BasicDataBlock(name);
            ret.DataDict.Add("SUIT", new JokerData() { SpecificCardSuit = suit, MyDataType = JokerDataType.SUIT });
            if (moneyAmount > 0)
                ret.DataDict.Add("MONEYAMOUNT", new JokerData() { IntData = moneyAmount, MyDataType = JokerDataType.INT });
            if (chipAmount > 0)
                ret.DataDict.Add("CHIPAMOUNT", new JokerData() { IntData = chipAmount, MyDataType = JokerDataType.INT });
            if (multAmount > 0)
                ret.DataDict.Add("MULTAMOUNT", new JokerData() { DoubleData = multAmount, MyDataType = JokerDataType.DOUBLE });
            if (multMultAmount > 0)
                ret.DataDict.Add("MULTMULTAMOUNT", new JokerData() { DoubleData = multMultAmount, MyDataType = JokerDataType.DOUBLE });
            if (randomRollDenominator > 0)
            {
                ret.DataDict.Add("NUMERATOR", new JokerData() { IntData = randomRollNumerator, MyDataType = JokerDataType.INT });
                ret.DataDict.Add("DENOMINATOR", new JokerData() { IntData = randomRollDenominator, MyDataType = JokerDataType.INT });
            }

            if (moneyAmount > 0)
                ret.DescriptionBuilder = _ => "Played cards with " + ret.DataDict["SUIT"].SpecificCardSuit + " suit earn $" + ret.DataDict["MONEYAMOUNT"].IntData + " when scored";
            else if (chipAmount > 0)
                ret.DescriptionBuilder = _ => "Played cards with " + ret.DataDict["SUIT"].SpecificCardSuit + " suit give +" + ret.DataDict["CHIPAMOUNT"].IntData + " chips when scored";
            else if (multAmount > 0)
                ret.DescriptionBuilder = _ => "Played cards with " + ret.DataDict["SUIT"].SpecificCardSuit + " suit give +" + ret.DataDict["MULTAMOUNT"].DoubleData + " Mult when scored";

            ret.Listeners.Add(new EngineEventListener()
            {
                MyContextType = EventContextType.CardTrigger,
                MyAction = args =>
                {
                    if (args is not EngineCardTriggerArgs t || !t.isScoringTrigger || !t.CardThatIsTriggering.IsSuit(ret.DataDict["SUIT"].SpecificCardSuit))
                        return;
                    if (randomRollDenominator > 0 && !Globals.RollRandom(ret.DataDict["NUMERATOR"].IntData, ret.DataDict["DENOMINATOR"].IntData, ret.MyCard))
                        return;
                    if (moneyAmount > 0)
                        Globals.EmitMoneyGain(ret.DataDict["MONEYAMOUNT"].IntData, ret.MyCard);
                    if (chipAmount > 0)
                        Globals.EmitChipsAdd(ret.DataDict["CHIPAMOUNT"].IntData, ret.MyCard);
                    if (multAmount > 0)
                        Globals.EmitMultAdd(ret.DataDict["MULTAMOUNT"].DoubleData, ret.MyCard);
                    if (multMultAmount > 0)
                        Globals.EmitMultMult(ret.DataDict["MULTMULTAMOUNT"].DoubleData, ret.MyCard);
                }
            });
            return ret;
        }

        private static JokerCardDataBlock BuildCopyJoker(string name, Card self, Func<Card, Card?> copyTargetGetter, string copyTargetString)
        {
            var ret = BasicDataBlock(name);
            //This also serves as a flag for copy jokers
            ret.DataDict.Add("TARGETCOPYCARD", new JokerData() { MyDataType = JokerDataType.CARD });
            ret.DataDict.Add("COPIEDLISTENERS", new JokerData() { MyDataType = JokerDataType.COPIEDLISTENERS });
            //...but also a flag just in case
            ret.isCopyJoker = true;
            ret.GetCopyTargetFunc = copyTargetGetter;

            Func<Card?> curTarget = () => copyTargetGetter(self);
            Func<bool> validTarget = () => curTarget() != null && !JokersNotCopyable.Contains(curTarget()!.JokerData.DBName);

            ret.DescriptionBuilder = _ =>
            {
                var target = curTarget();
                if (target == null)
                    return $"Copies ability of {copyTargetString} (Current target: none)";
                var validTxt = validTarget() ? "VALID" : "INVALID";
                return $"Copies ability of {copyTargetString} (Current target: {target.JokerData.JokerName}, {validTxt})";
            };

            Action refreshCopy = () =>
            {
                var target = curTarget();
                var origTarget = ret.DataDict["TARGETCOPYCARD"].CardData;
                var origData = ret.HiddenCopiedData;
                if (target == origTarget && (origTarget != null && origTarget.JokerData != null && !origTarget.JokerData.isCopyJoker))
                    return;
                if(origData != null)
                {
                    foreach (var list in origData.Listeners)
                    {
                        EngineEventHandler.StopListening(list);
                    }
                    foreach (var onRem in origData.OnJokerRemovalEffs)
                    {
                        onRem();
                    }
                }

                ret.DataDict["TARGETCOPYCARD"].CardData = target;
                var newData = target != null && validTarget() ? target.JokerData : null;

                InstallCopyOf(self, ret, newData);
                newData = ret.HiddenCopiedData;

                if (newData == null)
                    return;
                foreach (var list in newData.Listeners)
                {
                    EngineEventHandler.StartListening(list);
                }
                foreach (var onAdd in newData.OnJokerGainEffs)
                {
                    onAdd();
                }
            };

            ret.Listeners.Add(new EngineEventListener()
            {
                MyContextType = EventContextType.CardPositionsSwapDone,
                MyAction = args =>
                {
                    if (args is EngineCardPositionsSwappingArgs p && p.ZoneOfSwap == ZoneManager.JokerZone)
                        refreshCopy();
                }
            });
            ret.Listeners.Add(new EngineEventListener()
            {
                MyContextType = EventContextType.CardDrawnToZone,
                MyAction = args =>
                {
                    if (args is EngineCardDrawnToZoneArgs d && d.ZoneDrawnTo == ZoneManager.JokerZone)
                        refreshCopy();
                }
            });
            ret.Listeners.Add(new EngineEventListener()
            {
                MyContextType = EventContextType.CardDiscarded,
                MyAction = args =>
                {
                    if (args is EngineCardDiscardedFromZoneArgs d && d.ZoneCardIsLeaving == ZoneManager.JokerZone)
                        refreshCopy();
                }
            });
            ret.OnJokerGainEffs.Add(refreshCopy);
            ret.OnJokerRemovalEffs.Add(() => 
            {
                foreach (var list in ret.HiddenCopiedData.Listeners)
                {
                    EngineEventHandler.StopListening(list);
                }
                foreach (var onRem in ret.HiddenCopiedData.OnJokerRemovalEffs)
                {
                    onRem();
                }
                InstallCopyOf(self, ret, null);
            });

            return ret;
        }

        private static Card? GetJokerRightOfCard(Card c)
        {
            if (c.MyZone != ZoneManager.JokerZone || !ZoneManager.JokerZone.Cards.Contains(c))
                return null;
            var i = ZoneManager.JokerZone.Cards.IndexOf(c);
            return i >= 0 && i + 1 < ZoneManager.JokerZone.Cards.Count ? ZoneManager.JokerZone.Cards[i + 1] : null;
        }

        private static Card? GetLeftmostJoker(Card _)
        {
            return ZoneManager.JokerZone.Cards.FirstOrDefault();
        }

        private static bool CheckForLoopFloodFill(Card copierCard, JokerCardDataBlock copierDataBlock, JokerCardDataBlock targetDataBlock)
        {
            var traversed = new List<JokerCardDataBlock>();
            traversed.Add(copierDataBlock);
            traversed.Add(targetDataBlock);
            while(targetDataBlock != null && !traversed.Contains(targetDataBlock.GetCopyTargetFunc(targetDataBlock.MyCard).JokerData))
            {
                if (!targetDataBlock.GetCopyTargetFunc(targetDataBlock.MyCard).JokerData.isCopyJoker)
                {
                    return false;
                }
                else
                {
                    targetDataBlock = targetDataBlock.GetCopyTargetFunc(targetDataBlock.MyCard).JokerData;
                    traversed.Add(targetDataBlock);
                }
            }
            traversed.Clear();
            return targetDataBlock != null && targetDataBlock.GetCopyTargetFunc(targetDataBlock.MyCard).JokerData != null;
        }

        private static void InstallCopyOf(Card copierCard, JokerCardDataBlock copierDataBlock, JokerCardDataBlock? targetDataBlock, int depth = 1)
        {
            if(targetDataBlock != null && targetDataBlock.isCopyJoker)
            {
                if((!CheckForLoopFloodFill(copierCard, copierDataBlock, targetDataBlock)) && targetDataBlock.HiddenCopiedData != null)
                {
                    //If the target is a copy, propagate the copying through.
                    //hacky way to prevent infinite loops lol
                    if (depth < 1000)
                        InstallCopyOf(copierCard, copierDataBlock, targetDataBlock.HiddenCopiedData, depth + 1);
                    else
                        InstallCopyOf(copierCard, copierDataBlock, null);
                    return;
                }
                else
                {
                    copierDataBlock.HiddenCopiedData = null;
                    return;
                }
                
            }

            if(targetDataBlock == null)
            {
                copierDataBlock.HiddenCopiedData = null;
                return;
            }

            var dbTarget = targetDataBlock.DBName;
            var newBlock = JokerData[dbTarget](copierCard);
            newBlock.MyCard = copierCard;

            targetDataBlock.CopyDataDictTo(newBlock);
            targetDataBlock.OnCopyModifications?.Invoke(newBlock);

            copierDataBlock.HiddenCopiedData = newBlock;
        }
    }
}