using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleBalatro.Engine
{
    public static class UnlockManager
    {
        private static readonly object SaveLock = new();
        private static readonly Dictionary<string, EngineEventListener> AchievementListeners = new(StringComparer.OrdinalIgnoreCase);
        private static readonly List<EngineEventListener> CollectionListeners = new();
        private static HashSet<string> UnlockedDecks = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> AchievedAchievements = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> CollectedJokers = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> CollectedConsumables = new(StringComparer.OrdinalIgnoreCase);

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
        public static IReadOnlyCollection<string> RegisteredAchievementIds => AchievementDb.RegisteredAchievementIds;
        public static IReadOnlyCollection<string> CollectedJokerDbNames => CollectedJokers.OrderBy(x => x).ToList();
        public static IReadOnlyCollection<string> CollectedConsumableDbNames => CollectedConsumables.OrderBy(x => x).ToList();
        public static int CollectedJokerCount => CollectedJokers.Count;
        public static int CollectedConsumableCount => CollectedConsumables.Count;
        public static int CollectionCount => CollectedJokerCount + CollectedConsumableCount;
        // When true, unlocks, achievements, and collection progress still update in memory, but are not written to the permanent save file.
        public static bool PermanentProgressSavingDisabled { get; set; } = false;


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
            StopCollectionListeners();

            UnlockedDecks = new HashSet<string>(DeckDb.DefaultUnlockedDeckNames, StringComparer.OrdinalIgnoreCase);
            AchievedAchievements = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            CollectedJokers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            CollectedConsumables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (clearAchievementDefinitions)
            {
                AchievementDb.ClearAchievementDefinitions();
            }
            else
            {
                AchievementDb.RegisterDefaultAchievements();
                StartCollectionListeners();
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


        public static bool IsJokerCollected(string jokerDbName) => CollectedJokers.Contains(jokerDbName);

        public static bool IsConsumableCollected(string consumableDbName) => CollectedConsumables.Contains(consumableDbName);

        public static bool IsCollectionComplete() => CollectedJokers.IsSupersetOf(JokerDb.JokerDbNames)
            && CollectedConsumables.IsSupersetOf(GetAllConsumableDbNames());

        public static bool AddJokerToCollection(string jokerDbName, bool saveImmediately = true)
        {
            if (string.IsNullOrWhiteSpace(jokerDbName) || !JokerDb.JokerData.ContainsKey(jokerDbName))
            {
                return false;
            }

            return AddCollectionItem(CollectedJokers, jokerDbName, saveImmediately);
        }

        public static bool AddConsumableToCollection(string consumableDbName, bool saveImmediately = true)
        {
            if (string.IsNullOrWhiteSpace(consumableDbName) || !GetAllConsumableDbNames().Contains(consumableDbName))
            {
                return false;
            }

            return AddCollectionItem(CollectedConsumables, consumableDbName, saveImmediately);
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
            var displayData = AchievementDb.GetAchievementDisplayData(achievementId);
            var achArgs = new EngineAchievementUnlockArgs() { AchievementId = achievementId, AchievementName = displayData.Name, AchievementDesc = displayData.Details };
            EngineEventHandler.TriggerEvent(achArgs);

            if (saveImmediately)
            {
                SaveProgress();
            }
            return true;
        }

        public static void SaveProgress()
        {
            if (PermanentProgressSavingDisabled)
            {
                return;
            }

            lock (SaveLock)
            {
                var saveData = UnlockSaveData.FromCurrentState(UnlockedDecks, AchievedAchievements, CollectedJokers, CollectedConsumables);
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
            var registered = startListening
                ? AchievementDb.RegisterAchievementListener(achievementId, contextType, condition)
                : AchievementDb.RegisterAchievement(achievementId);

            var definition = AchievementDb.GetAchievementDefinition(achievementId);
            if (registered && definition != null && definition.StartListening && !IsAchievementAchieved(achievementId))
            {
                StartAchievementListener(definition);
            }

            return registered;
        }

        private static void ApplySaveData(UnlockSaveData? saveData)
        {
            UnlockedDecks = new HashSet<string>(DeckDb.DefaultUnlockedDeckNames, StringComparer.OrdinalIgnoreCase);
            AchievedAchievements = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            CollectedJokers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            CollectedConsumables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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

                foreach (var jokerDbName in (saveData.Collection?.Jokers ?? new List<string>()).Where(JokerDb.JokerData.ContainsKey))
                {
                    CollectedJokers.Add(jokerDbName);
                }

                var allConsumableDbNames = GetAllConsumableDbNames();
                foreach (var consumableDbName in (saveData.Collection?.Consumables ?? new List<string>()).Where(allConsumableDbNames.Contains))
                {
                    CollectedConsumables.Add(consumableDbName);
                }
            }

            StartCollectionListeners();
            StartUnachievedAchievementListeners();
        }

        private static void StartUnachievedAchievementListeners()
        {
            foreach (var listener in AchievementListeners.Values)
            {
                EngineEventHandler.StopListening(listener);
            }
            AchievementListeners.Clear();

            foreach (var definition in AchievementDb.AchievementDefinitions.Values.Where(x => x.StartListening && !IsAchievementAchieved(x.Id)))
            {
                StartAchievementListener(definition);
            }
        }

        private static void StartAchievementListener(AchievementDefinition definition)
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



        private static bool AddCollectionItem(HashSet<string> collection, string dbName, bool saveImmediately)
        {
            if (!collection.Add(dbName))
            {
                return false;
            }

            if (saveImmediately)
            {
                SaveProgress();
            }


            var args = new EngineCollectionItemAddArgs() { ItemDbName = dbName };
            EngineEventHandler.TriggerEvent(args);
            return true;
        }

        private static void StartCollectionListeners()
        {
            StopCollectionListeners();

            var jokerGainListener = new EngineEventListener
            {
                MyContextType = EventContextType.CardDrawnToZone,
                MyAction = args =>
                {
                    if (args is EngineCardDrawnToZoneArgs drawArgs
                        && drawArgs.ZoneDrawnTo == ZoneManager.JokerZone
                        && drawArgs.CardBeingDrawn.isJoker
                        && !drawArgs.CardBeingDrawn.isVoucher
                        && !drawArgs.CardBeingDrawn.isTag
                        && !string.IsNullOrWhiteSpace(drawArgs.CardBeingDrawn.JokerData?.DBName))
                    {
                        AddJokerToCollection(drawArgs.CardBeingDrawn.JokerData.DBName);
                    }
                },
            };

            var consumableUseListener = new EngineEventListener
            {
                MyContextType = EventContextType.ConsumableUsed,
                MyAction = args =>
                {
                    if (args is EngineConsumableUseArgs consumableArgs && !string.IsNullOrWhiteSpace(consumableArgs.ConsumableDBName))
                    {
                        AddConsumableToCollection(consumableArgs.ConsumableDBName);
                    }
                },
            };

            CollectionListeners.Add(jokerGainListener);
            CollectionListeners.Add(consumableUseListener);
            EngineEventHandler.StartListening(jokerGainListener);
            EngineEventHandler.StartListening(consumableUseListener);
        }

        private static void StopCollectionListeners()
        {
            foreach (var listener in CollectionListeners)
            {
                EngineEventHandler.StopListening(listener);
            }
            CollectionListeners.Clear();
        }

        private static HashSet<string> GetAllConsumableDbNames()
        {
            return new HashSet<string>(
                ConsumableManager.TarotNames
                    .Concat(ConsumableManager.SpectralNames)
                    .Concat(ConsumableManager.PlanetCardNames.Values.Select(x => x.ToUpper())),
                StringComparer.OrdinalIgnoreCase);
        }

        private sealed class UnlockSaveData
        {
            public int Version { get; set; } = 1;
            public DeckUnlockSaveData? Decks { get; set; } = new();
            public AchievementUnlockSaveData? Achievements { get; set; } = new();
            public CollectionSaveData? Collection { get; set; } = new();

            public static UnlockSaveData FromCurrentState(IEnumerable<string> unlockedDecks, IEnumerable<string> achievedAchievements, IEnumerable<string> collectedJokers, IEnumerable<string> collectedConsumables)
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
                    Collection = new CollectionSaveData
                    {
                        Jokers = collectedJokers.OrderBy(x => x).ToList(),
                        Consumables = collectedConsumables.OrderBy(x => x).ToList(),
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

        private sealed class CollectionSaveData
        {
            public List<string> Jokers { get; set; } = new();
            public List<string> Consumables { get; set; } = new();
        }
    }
}
