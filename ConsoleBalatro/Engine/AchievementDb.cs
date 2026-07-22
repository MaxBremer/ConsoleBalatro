using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;

namespace ConsoleBalatro.Engine
{
    public static class AchievementDb
    {
        private static readonly Dictionary<string, AchievementDefinition> AchievementDefinitionsById = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, AchievementDisplayData> AchievementDisplayDataById = new(StringComparer.OrdinalIgnoreCase);

        private const EventContextType DeckCheckEventType = EventContextType.EndPlayRound;

        public const string TenHandsPlayedAchievementId = "TEN_HANDS_PLAYED";
        public const string ScoreTenThousandAchievementId = "SCORE_10000_HAND";

        public const string Brainstorm_UnlockId = "BRAINSTORM_UNLOCK";

        public const string Satellite_UnlockId = "SATELLITE_UNLOCK";
        public const string HitTheRoad_UnlockId = "HITTHEROAD_UNLOCK";

        public const string OopsAll6s_UnlockId = "OOPSALL6S_UNLOCK";
        public const string TheIdol_UnlockId = "THEIDOL_UNLOCK";
        public const string Stuntman_UnlockId = "STUNTMAN_UNLOCK";

        public const string RoughGem_UnlockId = "ROUGHGEM_UNLOCK";
        public const string BloodStone_UnlockId = "BLOODSTONE_UNLOCK";
        public const string Arrowhead_UnlockId = "ARROWHEAD_UNLOCK";
        public const string OnyxAgate_UnlockId = "ONYXAGATE_UNLOCK";
        public const string GlassJoker_UnlockId = "GLASSJOKER_UNLOCK";
        public const string SmearedJoker_UnlockId = "SMEAREDJOKER_UNLOCK";
        public const string Certificate_UnlockId = "CERTIFICATE_UNLOCK";

        public const string Blueprint_UnlockId = "BLUEPRINT_UNLOCK";
        public const string Showman_UnlockId = "SHOWMAN_UNLOCK";
        public const string Flowerpot_UnlockId = "FLOWERPOT_UNLOCK";

        public const string MrBones_UnlockId = "MRBONES_UNLOCK";
        public const string Acrobat_UnlockId = "ACROBAT_UNLOCK";
        public const string SockAndBuskin_UnlockId = "SOCKANDBUSKIN_UNLOCK";
        public const string Swashbuckler_UnlockId = "SWASHBUCKLER_UNLOCK";
        public const string BurntJoker_UnlockId = "BURNTJOKER_UNLOCK";

        public const string GoldenTicket_UnlockId = "GOLDENTICKET_UNLOCK";
        public const string SeeingDouble_UnlockId = "SEEINGDOUBLE_UNLOCK";
        public const string DriversLicense_UnlockId = "DRIVERSLICENSE_UNLOCK";
        public const string Cartomancer_UnlockId = "CARTOMANCER_UNLOCK";
        public const string Astronomer_UnlockId = "ASTRONOMER_UNLOCK";

        public const string Duo_UnlockId = "DUO_UNLOCK";
        public const string Trio_UnlockId = "TRIO_UNLOCK";
        public const string Family_UnlockId = "FAMILY_UNLOCK";
        public const string Order_UnlockId = "ORDER_UNLOCK";
        public const string Tribe_UnlockId = "TRIBE_UNLOCK";

        public const string WeeJoker_UnlockId = "WEEJOKER_UNLOCK";
        public const string MerryAndy_UnlockId = "MERRYANDY_UNLOCK";
        public const string Bootstraps_UnlockId = "BOOTSTRAPS_UNLOCK";

        static AchievementDb()
        {
            RegisterDefaultAchievements();
        }

        public static IReadOnlyDictionary<string, AchievementDefinition> AchievementDefinitions => AchievementDefinitionsById;
        public static IReadOnlyCollection<string> RegisteredAchievementIds => AchievementDefinitionsById.Keys.OrderBy(x => x).ToList();

        public static void RegisterDefaultAchievements()
        {
            RegisterAllAchievementData(
                TenHandsPlayedAchievementId, 
                "Practiced Hand", 
                "Played 10 hands.", 
                EventContextType.HandPlayDone, 
                _ => EngineEventHandler.CountOfSaved(EventContextType.HandPlayDone) >= 10);
            RegisterAllAchievementData(
                ScoreTenThousandAchievementId, 
                "Lil Big Score", 
                "Played a hand that scored 100 or more total chips (TEST ACHIEVEMENT).", 
                EventContextType.HandPlayDone, 
                args => args is EngineHandPlayDoneArgs playArgs && playArgs.CurrentTotalChips >= 100);
            RegisterAllAchievementData(
                Brainstorm_UnlockId, 
                "Brainstorm Unlocked", 
                "Discarded a royal flush.", 
                EventContextType.HandDiscardDone, 
                args => args is EngineDiscardDoneArgs discardArgs && IsRoyalFlush(discardArgs.BeingDiscarded));

            RegisterPersistentCountAchievement(
                MrBones_UnlockId,
                "Mr. Bones Unlocked",
                "Lose 5 runs.",
                UnlockManager.LostRunsProgressKey,
                5);
            RegisterPersistentCountAchievement(
                Acrobat_UnlockId,
                "Acrobat Unlocked",
                "Play 200 hands.",
                UnlockManager.HandsPlayedProgressKey,
                200);
            RegisterPersistentCountAchievement(
                SockAndBuskin_UnlockId,
                "Sock and Buskin Unlocked",
                "Play a total of 300 face cards.",
                UnlockManager.FaceCardsPlayedProgressKey,
                300);
            RegisterPersistentCountAchievement(
                Swashbuckler_UnlockId,
                "Swashbuckler Unlocked",
                "Sell a total of 20 Joker cards.",
                UnlockManager.JokersSoldProgressKey,
                20);
            RegisterPersistentCountAchievement(
                BurntJoker_UnlockId,
                "Burnt Joker Unlocked",
                "Sell 50 cards.",
                UnlockManager.CardsSoldProgressKey,
                50);

            RegisterAllAchievementData(
                GoldenTicket_UnlockId,
                "Golden Ticket Unlocked",
                "Played a hand with 5 Golden cards.",
                EventContextType.CardsSelectedForPlay,
                args => args is EngineHandPlayArgs playArgs && playArgs.CardsSelected.Count(card => card.Enhancement == Enhancement.GOLD) >= 5);
            RegisterAllAchievementData(
                SeeingDouble_UnlockId,
                "Seeing Double Unlocked",
                "Played a hand containing 4 7s of Clubs.",
                EventContextType.CardsSelectedForPlay,
                args => args is EngineHandPlayArgs playArgs && playArgs.CardsSelected.Count(card => card.Rank == Rank.SEVEN && card.Suit == Suit.CLUBS) >= 4);
            RegisterAllAchievementData(
                HitTheRoad_UnlockId,
                "Hit the Road Unlocked",
                "Discarded 5 Jacks at the same time.",
                EventContextType.HandDiscardDone,
                args => args is EngineDiscardDoneArgs discardArgs && discardArgs.BeingDiscarded.Count(x => x.Rank == Rank.JACK) == 5);
            RegisterAllAchievementData(
                Satellite_UnlockId,
                "Satellite Unlocked",
                "Have at least 400$.",
                EventContextType.MoneyGainEmit,
                args => args is EngineGoldGainEmitArgs moneyArgs && Globals.Money + moneyArgs.AmountGained >= 400);
            RegisterAllAchievementData(
                Blueprint_UnlockId,
                "Blueprint Unlocked",
                "Win a run.",
                EventContextType.RunWon,
                args => true);
            RegisterAllAchievementData(
                Showman_UnlockId,
                "Showman Unlocked",
                "Reach Ante level 4.",
                EventContextType.AnteChange,
                args => args is EngineNewAnteArgs anteArgs && anteArgs.NewAnteVal == 4);
            RegisterAllAchievementData(
                Flowerpot_UnlockId,
                "Flowerpot Unlocked",
                "Reach Ante level 8.",
                EventContextType.AnteChange,
                args => args is EngineNewAnteArgs anteArgs && anteArgs.NewAnteVal == 8);
            RegisterAllAchievementData(
                Cartomancer_UnlockId,
                "Cartomancer Unlocked",
                "Discover every Tarot card.",
                EventContextType.CollectionItemAdded,
                args => AllTarotsCollected());
            RegisterAllAchievementData(
                Astronomer_UnlockId,
                "Astronomer Unlocked",
                "Discover every Planet card.",
                EventContextType.CollectionItemAdded,
                args => AllPlanetsCollected());

            RegisterChipNumAchievement(
                OopsAll6s_UnlockId,
                "Oops! All 6s Unlocked",
                "Gain at least 10,000 chips in a single hand.",
                10000);
            RegisterChipNumAchievement(
                TheIdol_UnlockId,
                "The Idol Unlocked",
                "Gain at least 1,000,000 chips in a single hand.",
                1000000);
            RegisterChipNumAchievement(
                Stuntman_UnlockId,
                "Stuntman Unlocked",
                "Gain at least 100,000,000 chips in a single hand.",
                100000000);

            RegisterWinWithoutHandAchievement(
                Duo_UnlockId,
                "The Duo Unlocked",
                "Win a run without playing a Pair.",
                PlayedHandType.PAIR);
            RegisterWinWithoutHandAchievement(
                Trio_UnlockId,
                "The Trio Unlocked",
                "Win a run without playing a Three of a Kind.",
                PlayedHandType.THREEOFAKIND);
            RegisterWinWithoutHandAchievement(
                Family_UnlockId,
                "The Family Unlocked",
                "Win a run without playing a Four of a Kind.",
                PlayedHandType.FOUROFAKIND);
            RegisterWinWithoutHandAchievement(
                Order_UnlockId,
                "The Order Unlocked",
                "Win a run without playing a Straight.",
                PlayedHandType.STRAIGHT);
            RegisterWinWithoutHandAchievement(
                Tribe_UnlockId,
                "The Tribe Unlocked",
                "Win a run without playing a Flush.",
                PlayedHandType.FLUSH);

            RegisterAllAchievementData(
                WeeJoker_UnlockId,
                "Wee Joker Unlocked",
                "Win a run in 18 or fewer play rounds.",
                EventContextType.RunWon,
                args => ScoreHandler.NumRoundsPlayedSoFar <= 18);
            RegisterAllAchievementData(
                MerryAndy_UnlockId,
                "Merry Andy Unlocked",
                "Win a run in 12 or fewer play rounds.",
                EventContextType.RunWon,
                args => ScoreHandler.NumRoundsPlayedSoFar <= 12);

            RegisterDeckCheckAchievement(
                RoughGem_UnlockId,
                "Rough Gem Unlocked",
                "Have at least 30 cards with Diamond suit in your deck.",
                cards => cards != null && cards.Count(card => card.IsSuit(Suit.DIAMONDS)) >= 30);
            RegisterDeckCheckAchievement(
                BloodStone_UnlockId,
                "Blood Stone Unlocked",
                "Have at least 30 cards with Heart suit in your deck.",
                cards => cards != null && cards.Count(card => card.IsSuit(Suit.HEARTS)) >= 30);
            RegisterDeckCheckAchievement(
                Arrowhead_UnlockId,
                "Arrowhead Unlocked",
                "Have at least 30 cards with Spade suit in your deck.",
                cards => cards != null && cards.Count(card => card.IsSuit(Suit.SPADES)) >= 30);
            RegisterDeckCheckAchievement(
                OnyxAgate_UnlockId,
                "Onyx Agate Unlocked",
                "Have at least 30 cards with Club suit in your deck.",
                cards => cards != null && cards.Count(card => card.IsSuit(Suit.CLUBS)) >= 30);

            RegisterDeckCheckAchievement(
                GlassJoker_UnlockId,
                "Glass Joker Unlocked",
                "Have at least5 Glass cards in your deck.",
                cards => cards != null && cards.Count(card => card.Enhancement == Enhancement.GLASS) >= 5);
            RegisterDeckCheckAchievement(
                SmearedJoker_UnlockId,
                "Smeared Joker Unlocked",
                "Have at least 3 Wild cards in your deck.",
                cards => cards != null && cards.Count(card => card.Enhancement == Enhancement.WILD) >= 3);
            RegisterDeckCheckAchievement(
                Certificate_UnlockId,
                "Certificate Unlocked",
                "Have a Gold playing card with a Gold seal in your deck.",
                cards => cards != null && cards.Any(card => card.Enhancement == Enhancement.GOLD && card.Seal == Seal.GOLD));
            RegisterDeckCheckAchievement(
                DriversLicense_UnlockId,
                "Driver's License Unlocked",
                "Enhance 16 cards in your deck.",
                cards => cards != null && cards.Count(card => card.Enhancement != Enhancement.NONE) >= 16);
            RegisterAllAchievementData(
                Bootstraps_UnlockId,
                "Bootstraps Unlocked",
                "Have at least 2 Polychrome Jokers at the same time.",
                EventContextType.CardDrawnToZone,
                args => ZoneManager.JokerZone != null && ZoneManager.JokerZone.Cards.Count(c => c.IsJoker && c.Edition == Edition.POLYCHROME) >= 2);
        }

        private static void RegisterAllAchievementData(string id, string name, string desc, EventContextType contextType, Func<EngineEventArgs, bool> condition)
        {
            RegisterAchievementDisplayData(id, name, desc);
            RegisterAchievementListener(id, contextType, condition);
        }

        public static void ClearAchievementDefinitions()
        {
            AchievementDefinitionsById.Clear();
            AchievementDisplayDataById.Clear();
        }

        public static void RegisterChipNumAchievement(string id, string name, string desc, int chipNum)
        {
            RegisterAllAchievementData(id, name, desc, EventContextType.TotalChipsGained, args => args is EngineTotalChipsGainArgs chipArgs && chipArgs.AmountBeingGained >= chipNum);
        }

        public static void RegisterPersistentCountAchievement(string id, string name, string desc, string progressKey, int targetCount)
        {
            RegisterAllAchievementData(id, name, desc, EventContextType.AchievementProgressChanged, _ => UnlockManager.GetPersistentProgressCount(progressKey) >= targetCount);
        }

        public static void RegisterDeckCheckAchievement(string id, string name, string desc, Func<List<Card>?, bool> condition)
        {
            RegisterAllAchievementData(id, name, desc, DeckCheckEventType, _ => condition(ZoneManager.GetFullDeckCards()));
        }

        public static void RegisterWinWithoutHandAchievement(string id, string name, string desc, PlayedHandType hand)
        {
            RegisterAllAchievementData(id, name, desc, EventContextType.RunWon, args => ScoreHandler.HandNumTimesPlayed[hand] == 0);
        }

        public static bool RegisterAchievement(string achievementId)
        {
            return RegisterAchievementDefinition(new AchievementDefinition(achievementId, EventContextType.NONE, _ => true, StartListening: false));
        }

        public static bool RegisterAchievementListener(string achievementId, EventContextType contextType, Func<EngineEventArgs, bool> condition)
        {
            return RegisterAchievementDefinition(new AchievementDefinition(achievementId, contextType, condition, StartListening: true));
        }

        public static bool RegisterAchievementDisplayData(string achievementId, string name, string details)
        {
            if (string.IsNullOrWhiteSpace(achievementId) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(details))
            {
                return false;
            }

            AchievementDisplayDataById[achievementId] = new AchievementDisplayData(name, details);
            return true;
        }

        public static AchievementDefinition? GetAchievementDefinition(string achievementId)
        {
            return AchievementDefinitionsById.GetValueOrDefault(achievementId);
        }

        public static AchievementDisplayData GetAchievementDisplayData(string achievementId)
        {
            return AchievementDisplayDataById.GetValueOrDefault(achievementId)
                ?? new AchievementDisplayData(achievementId, "Unlocked an achievement.");
        }

        private static bool RegisterAchievementDefinition(AchievementDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(definition.Id) || definition.Condition == null)
            {
                return false;
            }

            AchievementDefinitionsById[definition.Id] = definition;
            return true;
        }

        private static bool AllTarotsCollected()
        {
            foreach (var tName in ConsumableManager.TarotNames)
            {
                if(!UnlockManager.CollectedConsumableDbNames.Contains(tName))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AllPlanetsCollected()
        {
            foreach (var pName in ConsumableManager.PlanetCardNames.Values)
            {
                if (!UnlockManager.CollectedConsumableDbNames.Contains(pName))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsRoyalFlush(List<Card>? cards)
        {
            if (cards == null || cards.Count < 5)
            {
                return false;
            }

            var royalRanks = new HashSet<Rank> { Rank.TEN, Rank.JACK, Rank.QUEEN, Rank.KING, Rank.ACE };
            return cards
                .Where(card => royalRanks.Contains(card.Rank))
                .GroupBy(card => card.Suit)
                .Any(group => group.Key != Suit.NONE && royalRanks.All(rank => group.Any(card => card.Rank == rank)));
        }
    }

    public sealed record AchievementDisplayData(string Name, string Details);

    public sealed record AchievementDefinition(string Id, EventContextType ContextType, Func<EngineEventArgs, bool> Condition, bool StartListening);
}
