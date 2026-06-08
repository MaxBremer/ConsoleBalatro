using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleBalatro.Engine
{
    public static class UnlockManager
    {
        private static readonly object SaveLock = new();
        private static readonly Dictionary<string, UnlockAchievementDefinition> AchievementDefinitions = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, EngineEventListener> AchievementListeners = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> UnlockedDecks = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> AchievedAchievements = new(StringComparer.OrdinalIgnoreCase);

        public const string TenHandsPlayedAchievementId = "TEN_HANDS_PLAYED";
        public const string ScoreTenThousandAchievementId = "SCORE_10000_HAND";
        public const string DiscardRoyalFlushAchievementId = "DISCARD_ROYAL_FLUSH";

        private static readonly Dictionary<string, AchievementDisplayData> AchievementDisplayDataById = new(StringComparer.OrdinalIgnoreCase)
        {
            { TenHandsPlayedAchievementId, new AchievementDisplayData("Practiced Hand", "Played 10 hands.") },
            { ScoreTenThousandAchievementId, new AchievementDisplayData("Big Score", "Played a hand that scored 10,000 or more total chips.") },
            { DiscardRoyalFlushAchievementId, new AchievementDisplayData("Royal Mistake", "Discarded a royal flush instead of playing it.") },
        };

        private static readonly JsonSerializerOptions SaveJsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static string SaveFilePath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ConsoleBalatro",
            "unlocks.json");

        public static event Action<AchievementUnlockedEventArgs>? AchievementUnlocked;

        public static IReadOnlyCollection<string> UnlockedDeckNames => UnlockedDecks.OrderBy(x => x).ToList();
        public static IReadOnlyCollection<string> AchievedAchievementIds => AchievedAchievements.OrderBy(x => x).ToList();
        public static IReadOnlyCollection<string> RegisteredAchievementIds => AchievementDefinitions.Keys.OrderBy(x => x).ToList();

        static UnlockManager()
        {
            RegisterBuiltInAchievements();
            ResetProgressToDefaults();
        }

        public static void ResetProgressToDefaults(bool clearAchievementDefinitions = false)
        {
            foreach (var listener in AchievementListeners.Values)
            {
                EngineEventHandler.StopListening(listener);
            }
            AchievementListeners.Clear();

            UnlockedDecks = new HashSet<string>(DeckDb.DefaultUnlockedDeckNames, StringComparer.OrdinalIgnoreCase);
            AchievedAchievements = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (clearAchievementDefinitions)
            {
                AchievementDefinitions.Clear();
            }
            else
            {
                RegisterBuiltInAchievements();
                StartUnachievedAchievementListeners();
            }
        }


        public static void RegisterBuiltInAchievements()
        {
            RegisterAchievementListener(
                TenHandsPlayedAchievementId,
                EventContextType.HandPlayDone,
                _ => EngineEventHandler.CountOfSaved(EventContextType.HandPlayDone) >= 10);

            RegisterAchievementListener(
                ScoreTenThousandAchievementId,
                EventContextType.HandPlayDone,
                args => args is EngineHandPlayDoneArgs playArgs && playArgs.CurrentTotalChips >= 10000);

            RegisterAchievementListener(
                DiscardRoyalFlushAchievementId,
                EventContextType.HandDiscardDone,
                args => args is EngineDiscardDoneArgs discardArgs && IsRoyalFlush(discardArgs.BeingDiscarded));
        }

        public static bool IsDeckUnlocked(string deckDbName) => UnlockedDecks.Contains(deckDbName);

        public static bool UnlockDeck(string deckDbName, bool saveImmediately = true)
        {
            if (!DeckDb.DeckData.ContainsKey(deckDbName))
            {
                return false;
            }

            if (!UnlockedDecks.Add(deckDbName))
            {
                return false;
            }

            if (saveImmediately)
            {
                SaveProgress();
            }
            return true;
        }

        public static bool IsAchievementAchieved(string achievementId) => AchievedAchievements.Contains(achievementId);

        public static bool RegisterAchievement(string achievementId)
        {
            return RegisterAchievementListener(achievementId, EventContextType.NONE, _ => true, startListening: false);
        }

        public static bool RegisterAchievementListener(string achievementId, EventContextType contextType, Func<EngineEventArgs, bool> condition)
        {
            return RegisterAchievementListener(achievementId, contextType, condition, startListening: true);
        }

        public static bool MarkAchievementAchieved(string achievementId, bool saveImmediately = true)
        {
            if (string.IsNullOrWhiteSpace(achievementId))
            {
                return false;
            }

            if (!AchievedAchievements.Add(achievementId))
            {
                return false;
            }

            StopAchievementListener(achievementId);
            AchievementUnlocked?.Invoke(BuildAchievementUnlockedEventArgs(achievementId));

            if (saveImmediately)
            {
                SaveProgress();
            }
            return true;
        }

        public static void SaveProgress()
        {
            lock (SaveLock)
            {
                var saveData = UnlockSaveData.FromCurrentState(UnlockedDecks, AchievedAchievements);
                var saveDirectory = Path.GetDirectoryName(SaveFilePath);
                if (!string.IsNullOrWhiteSpace(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                var json = JsonSerializer.Serialize(saveData, SaveJsonOptions);
                var tempPath = SaveFilePath + ".tmp";
                File.WriteAllText(tempPath, json);
                File.Move(tempPath, SaveFilePath, true);
            }
        }

        public static bool LoadProgress()
        {
            lock (SaveLock)
            {
                if (!File.Exists(SaveFilePath))
                {
                    ApplySaveData(null);
                    return false;
                }

                UnlockSaveData? saveData;
                try
                {
                    saveData = JsonSerializer.Deserialize<UnlockSaveData>(File.ReadAllText(SaveFilePath), SaveJsonOptions);
                }
                catch (JsonException)
                {
                    ApplySaveData(null);
                    return false;
                }

                ApplySaveData(saveData);
                return true;
            }
        }


        private static AchievementUnlockedEventArgs BuildAchievementUnlockedEventArgs(string achievementId)
        {
            var displayData = AchievementDisplayDataById.GetValueOrDefault(achievementId)
                ?? new AchievementDisplayData(achievementId, "Unlocked an achievement.");

            return new AchievementUnlockedEventArgs(achievementId, displayData.Name, displayData.Details);
        }

        private static bool RegisterAchievementListener(string achievementId, EventContextType contextType, Func<EngineEventArgs, bool> condition, bool startListening)
        {
            if (string.IsNullOrWhiteSpace(achievementId) || condition == null)
            {
                return false;
            }

            StopAchievementListener(achievementId);
            AchievementDefinitions[achievementId] = new UnlockAchievementDefinition(achievementId, contextType, condition, startListening);

            if (startListening && !IsAchievementAchieved(achievementId))
            {
                StartAchievementListener(AchievementDefinitions[achievementId]);
            }

            return true;
        }

        private static void ApplySaveData(UnlockSaveData? saveData)
        {
            UnlockedDecks = new HashSet<string>(DeckDb.DefaultUnlockedDeckNames, StringComparer.OrdinalIgnoreCase);
            AchievedAchievements = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (saveData != null)
            {
                foreach (var deckName in (saveData.Decks?.Unlocked ?? new List<string>()).Where(DeckDb.DeckData.ContainsKey))
                {
                    UnlockedDecks.Add(deckName);
                }

                foreach (var achievementId in (saveData.Achievements?.Achieved ?? new List<string>()).Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    AchievedAchievements.Add(achievementId);
                }
            }

            StartUnachievedAchievementListeners();
        }

        private static void StartUnachievedAchievementListeners()
        {
            foreach (var listener in AchievementListeners.Values)
            {
                EngineEventHandler.StopListening(listener);
            }
            AchievementListeners.Clear();

            foreach (var definition in AchievementDefinitions.Values.Where(x => x.StartListening && !IsAchievementAchieved(x.Id)))
            {
                StartAchievementListener(definition);
            }
        }

        private static void StartAchievementListener(UnlockAchievementDefinition definition)
        {
            var listener = new EngineEventListener
            {
                MyContextType = definition.ContextType,
            };
            listener.MyAction = args =>
            {
                if (definition.Condition(args) && MarkAchievementAchieved(definition.Id))
                {
                    listener.RemoveAfterTriggering = true;
                }
            };

            AchievementListeners[definition.Id] = listener;
            EngineEventHandler.StartListening(listener);
        }

        private static void StopAchievementListener(string achievementId)
        {
            if (AchievementListeners.TryGetValue(achievementId, out var listener))
            {
                EngineEventHandler.StopListening(listener);
                AchievementListeners.Remove(achievementId);
            }
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

        private sealed record AchievementDisplayData(string Name, string Details);

        private sealed record UnlockAchievementDefinition(string Id, EventContextType ContextType, Func<EngineEventArgs, bool> Condition, bool StartListening);

        private sealed class UnlockSaveData
        {
            public int Version { get; set; } = 1;
            public DeckUnlockSaveData? Decks { get; set; } = new();
            public AchievementUnlockSaveData? Achievements { get; set; } = new();

            public static UnlockSaveData FromCurrentState(IEnumerable<string> unlockedDecks, IEnumerable<string> achievedAchievements)
            {
                return new UnlockSaveData
                {
                    Decks = new DeckUnlockSaveData
                    {
                        Unlocked = unlockedDecks.OrderBy(x => x).ToList(),
                    },
                    Achievements = new AchievementUnlockSaveData
                    {
                        Achieved = achievedAchievements.OrderBy(x => x).ToList(),
                    },
                };
            }
        }

        private sealed class DeckUnlockSaveData
        {
            public List<string> Unlocked { get; set; } = new();
        }

        private sealed class AchievementUnlockSaveData
        {
            public List<string> Achieved { get; set; } = new();
        }
    }

    public sealed class AchievementUnlockedEventArgs : EventArgs
    {
        public AchievementUnlockedEventArgs(string achievementId, string achievementName, string achievementDetails)
        {
            AchievementId = achievementId;
            AchievementName = achievementName;
            AchievementDetails = achievementDetails;
        }

        public string AchievementId { get; }
        public string AchievementName { get; }
        public string AchievementDetails { get; }
    }
}
