using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Events;
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

        private static readonly JsonSerializerOptions SaveJsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static string SaveFilePath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ConsoleBalatro",
            "unlocks.json");

        public static IReadOnlyCollection<string> UnlockedDeckNames => UnlockedDecks.OrderBy(x => x).ToList();
        public static IReadOnlyCollection<string> AchievedAchievementIds => AchievedAchievements.OrderBy(x => x).ToList();
        public static IReadOnlyCollection<string> RegisteredAchievementIds => AchievementDefinitions.Keys.OrderBy(x => x).ToList();

        static UnlockManager()
        {
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
                StartUnachievedAchievementListeners();
            }
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
}
