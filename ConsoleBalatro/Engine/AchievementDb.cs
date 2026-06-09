using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;

namespace ConsoleBalatro.Engine
{
    public static class AchievementDb
    {
        private static readonly Dictionary<string, AchievementDefinition> AchievementDefinitionsById = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, AchievementDisplayData> AchievementDisplayDataById = new(StringComparer.OrdinalIgnoreCase);

        public const string TenHandsPlayedAchievementId = "TEN_HANDS_PLAYED";
        public const string ScoreTenThousandAchievementId = "SCORE_10000_HAND";
        public const string DiscardRoyalFlushAchievementId = "DISCARD_ROYAL_FLUSH";

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
                "Big Score", 
                "Played a hand that scored 10,000 or more total chips.", 
                EventContextType.HandPlayDone, 
                args => args is EngineHandPlayDoneArgs playArgs && playArgs.CurrentTotalChips >= 100);
            RegisterAllAchievementData(
                DiscardRoyalFlushAchievementId, 
                "Royal Mistake", 
                "Discarded a royal flush instead of playing it.", 
                EventContextType.HandDiscardDone, 
                args => args is EngineDiscardDoneArgs discardArgs && IsRoyalFlush(discardArgs.BeingDiscarded));
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
