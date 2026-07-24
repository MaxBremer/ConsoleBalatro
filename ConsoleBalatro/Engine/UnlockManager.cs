using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Stakes;
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
        private static readonly List<EngineEventListener> PersistentProgressListeners = new();
        private static HashSet<string> UnlockedDecks = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> AchievedAchievements = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> CollectedJokers = new(StringComparer.OrdinalIgnoreCase);
        private static HashSet<string> CollectedConsumables = new(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, int> DeckHighestBeatenStakeIndexes = new(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, int> JokerHighestBeatenStakeIndexes = new(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, int> PersistentProgressCounts = new(StringComparer.OrdinalIgnoreCase);

        public const string LostRunsProgressKey = "LOST_RUNS";
        public const string HandsPlayedProgressKey = "HANDS_PLAYED";
        public const string FaceCardsPlayedProgressKey = "FACE_CARDS_PLAYED";
        public const string JokersSoldProgressKey = "JOKERS_SOLD";
        public const string CardsSoldProgressKey = "CARDS_SOLD";
        public const string ShopDollarsProgressKey = "DOLLARS_SPENT_AT_SHOP";
        public const string ShopRerollsProgressKey = "REROLLS";
        public const string TarotsUsedFromPacksProgressKey = "PACK_TAROTS";
        public const string TarotsBoughtFromShopProgressKey = "SHOP_TAROTS";
        public const string PlanetsUsedFromPacksProgressKey = "PACK_PLANETS";
        public const string PlanetsBoughtFromShopProgressKey = "SHOP_PLANETS";
        public const string PlayingCardsBoughtFromShopProgressKey = "SHOP_PLAYING_CARDS";
        public const string CardsPlayedProgressKey = "CARDS_PLAYED";
        public const string CardsDiscardedProgressKey = "CARDS_DISCARDED";
        public const string BlanksRedeemedProgressKey = "BLANKS";

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
            StopPersistentProgressListeners();

            UnlockedDecks = new HashSet<string>(DeckDb.DefaultUnlockedDeckNames, StringComparer.OrdinalIgnoreCase);
            AchievedAchievements = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            CollectedJokers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            CollectedConsumables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            DeckHighestBeatenStakeIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            JokerHighestBeatenStakeIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            PersistentProgressCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (clearAchievementDefinitions)
            {
                AchievementDb.ClearAchievementDefinitions();
            }
            else
            {
                AchievementDb.RegisterDefaultAchievements();
                StartPersistentProgressListeners();
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


        public static int GetStakesBeatenCountForDeck(string deckDbName)
        {
            return Math.Max(0, GetHighestBeatenStakeIndexForDeck(deckDbName) + 1);
        }

        public static StakeType? GetHighestBeatenStakeForDeck(string deckDbName)
        {
            var index = GetHighestBeatenStakeIndexForDeck(deckDbName);
            return index >= 0 ? StakeManager.OfficialStakeOrder[index] : null;
        }

        public static bool HasDeckStakeSticker(string deckDbName, StakeType stakeType)
        {
            var stakeIndex = StakeManager.OfficialStakeOrder.IndexOf(stakeType);
            return stakeIndex >= 0 && stakeIndex <= GetHighestBeatenStakeIndexForDeck(deckDbName);
        }

        public static bool IsStakeUnlockedForDeck(string deckDbName, StakeType stakeType)
        {
            if (!IsDeckUnlocked(deckDbName))
            {
                return false;
            }

            var stakeIndex = StakeManager.OfficialStakeOrder.IndexOf(stakeType);
            if (stakeIndex < 0)
            {
                return false;
            }

            var maxUnlockedIndex = Math.Min(GetHighestBeatenStakeIndexForDeck(deckDbName) + 1, StakeManager.OfficialStakeOrder.Count - 1);
            return stakeIndex <= maxUnlockedIndex;
        }

        public static bool MarkDeckStakeBeaten(string deckDbName, StakeType stakeType, bool saveImmediately = true)
        {
            if (!DeckDb.DeckData.ContainsKey(deckDbName))
            {
                return false;
            }

            var stakeIndex = StakeManager.OfficialStakeOrder.IndexOf(stakeType);
            if (stakeIndex < 0 || stakeIndex <= GetHighestBeatenStakeIndexForDeck(deckDbName))
            {
                return false;
            }

            DeckHighestBeatenStakeIndexes[deckDbName] = stakeIndex;
            if (saveImmediately)
            {
                SaveProgress();
            }
            return true;
        }

        private static int GetHighestBeatenStakeIndexForDeck(string deckDbName)
        {
            return DeckHighestBeatenStakeIndexes.TryGetValue(deckDbName, out var index) ? Math.Clamp(index, -1, StakeManager.OfficialStakeOrder.Count - 1) : -1;
        }


        public static bool IsJokerCollected(string jokerDbName) => CollectedJokers.Contains(jokerDbName);

        public static StakeType? GetHighestBeatenStakeForJoker(string jokerDbName)
        {
            var index = GetHighestBeatenStakeIndexForJoker(jokerDbName);
            return index >= 0 ? StakeManager.OfficialStakeOrder[index] : null;
        }

        public static bool HasJokerStakeSticker(string jokerDbName, StakeType stakeType)
        {
            var stakeIndex = StakeManager.OfficialStakeOrder.IndexOf(stakeType);
            return stakeIndex >= 0 && stakeIndex <= GetHighestBeatenStakeIndexForJoker(jokerDbName);
        }

        public static bool MarkJokerStakeBeaten(string jokerDbName, StakeType stakeType, bool saveImmediately = true)
        {
            if (string.IsNullOrWhiteSpace(jokerDbName) || !JokerDb.JokerData.ContainsKey(jokerDbName))
            {
                return false;
            }

            var stakeIndex = StakeManager.OfficialStakeOrder.IndexOf(stakeType);
            if (stakeIndex < 0 || stakeIndex <= GetHighestBeatenStakeIndexForJoker(jokerDbName))
            {
                return false;
            }

            JokerHighestBeatenStakeIndexes[jokerDbName] = stakeIndex;
            if (saveImmediately)
            {
                SaveProgress();
            }
            return true;
        }

        private static int GetHighestBeatenStakeIndexForJoker(string jokerDbName)
        {
            return JokerHighestBeatenStakeIndexes.TryGetValue(jokerDbName, out var index) ? Math.Clamp(index, -1, StakeManager.OfficialStakeOrder.Count - 1) : -1;
        }

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

        public static int GetPersistentProgressCount(string progressKey)
        {
            return !string.IsNullOrWhiteSpace(progressKey) && PersistentProgressCounts.TryGetValue(progressKey, out var count)
                ? Math.Max(0, count)
                : 0;
        }

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
                var saveData = UnlockSaveData.FromCurrentState(UnlockedDecks, DeckHighestBeatenStakeIndexes, JokerHighestBeatenStakeIndexes, AchievedAchievements, PersistentProgressCounts, CollectedJokers, CollectedConsumables);
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
            DeckHighestBeatenStakeIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            JokerHighestBeatenStakeIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            PersistentProgressCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (saveData != null)
            {
                foreach (var deckName in (saveData.Decks?.Unlocked ?? new List<string>()).Where(DeckDb.DeckData.ContainsKey))
                {
                    UnlockedDecks.Add(deckName);
                }

                foreach (var beaten in saveData.Decks?.HighestBeatenStakeIndexes ?? new Dictionary<string, int>())
                {
                    if (DeckDb.DeckData.ContainsKey(beaten.Key))
                    {
                        DeckHighestBeatenStakeIndexes[beaten.Key] = Math.Clamp(beaten.Value, -1, StakeManager.OfficialStakeOrder.Count - 1);
                    }
                }

                foreach (var achievementId in (saveData.Achievements?.Achieved ?? new List<string>()).Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    AchievedAchievements.Add(achievementId);
                }

                foreach (var progress in saveData.Achievements?.ProgressCounts ?? new Dictionary<string, int>())
                {
                    if (!string.IsNullOrWhiteSpace(progress.Key))
                    {
                        PersistentProgressCounts[progress.Key] = Math.Max(0, progress.Value);
                    }
                }

                foreach (var jokerDbName in (saveData.Collection?.Jokers ?? new List<string>()).Where(JokerDb.JokerData.ContainsKey))
                {
                    CollectedJokers.Add(jokerDbName);
                }

                foreach (var beaten in saveData.Collection?.JokerHighestBeatenStakeIndexes ?? new Dictionary<string, int>())
                {
                    if (JokerDb.JokerData.ContainsKey(beaten.Key))
                    {
                        JokerHighestBeatenStakeIndexes[beaten.Key] = Math.Clamp(beaten.Value, -1, StakeManager.OfficialStakeOrder.Count - 1);
                    }
                }

                var allConsumableDbNames = GetAllConsumableDbNames();
                foreach (var consumableDbName in (saveData.Collection?.Consumables ?? new List<string>()).Where(allConsumableDbNames.Contains))
                {
                    CollectedConsumables.Add(consumableDbName);
                }
            }

            StartPersistentProgressListeners();
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
                        && drawArgs.CardBeingDrawn.IsJoker
                        && !drawArgs.CardBeingDrawn.IsVoucher
                        && !drawArgs.CardBeingDrawn.IsTag
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

        private static void StartPersistentProgressListeners()
        {
            StopPersistentProgressListeners();

            PersistentProgressListeners.Add(new EngineEventListener
            {
                MyContextType = EventContextType.RunLost,
                MyAction = _ => IncrementPersistentProgressCount(LostRunsProgressKey),
            });
            PersistentProgressListeners.Add(new EngineEventListener
            {
                MyContextType = EventContextType.HandPlayDone,
                MyAction = _ => IncrementPersistentProgressCount(HandsPlayedProgressKey),
            });
            PersistentProgressListeners.Add(new EngineEventListener
            {
                MyContextType = EventContextType.CardsSelectedForPlay,
                MyAction = args =>
                {
                    if (args is EngineHandPlayArgs playArgs)
                        IncrementPersistentProgressCount(FaceCardsPlayedProgressKey, playArgs.CardsSelected.Count(EngineUtils.isFace));
                },
            });
            PersistentProgressListeners.Add(new EngineEventListener
            {
                MyContextType = EventContextType.CardSell,
                MyAction = args =>
                {
                    if (args is EngineCardSoldArgs soldArgs)
                    {
                        IncrementPersistentProgressCount(CardsSoldProgressKey);
                        if (soldArgs.CardBeingSold.IsJoker)
                            IncrementPersistentProgressCount(JokersSoldProgressKey);
                    }
                },
            });
            PersistentProgressListeners.Add(new EngineEventListener
            {
                MyContextType = EventContextType.CardPurchased,
                MyAction = args =>
                {
                    if(args is EngineCardPurchasedArgs buyArgs)
                    {
                        IncrementPersistentProgressCount(ShopDollarsProgressKey, buyArgs.AmountPaid);
                        if (buyArgs.BeingPurchased.isConsumable)
                        {
                            if(buyArgs.BeingPurchased.ConsumableData.Type == ConsumableType.TAROT)
                                IncrementPersistentProgressCount(TarotsBoughtFromShopProgressKey);
                            else if(buyArgs.BeingPurchased.ConsumableData.Type == ConsumableType.PLANET)
                                IncrementPersistentProgressCount(PlanetsBoughtFromShopProgressKey);
                        }
                        else if (buyArgs.BeingPurchased.isPlayingCard)
                            IncrementPersistentProgressCount(PlayingCardsBoughtFromShopProgressKey);
                    }
                }
            });
            PersistentProgressListeners.Add(new EngineEventListener
            {
                MyContextType = EventContextType.MarketRerolled,
                MyAction = _ =>
                {
                    IncrementPersistentProgressCount(ShopRerollsProgressKey);
                }
            });
            PersistentProgressListeners.Add(new EngineEventListener
            {
                MyContextType = EventContextType.ConsumableUsed,
                MyAction = args =>
                {
                    if(args is EngineConsumableUseArgs conArgs)
                    {
                        if(conArgs.ZoneUsedFrom == ZoneManager.PackOptionZone)
                        {
                            if(conArgs.TypeUsed == ConsumableType.TAROT)
                                IncrementPersistentProgressCount(TarotsUsedFromPacksProgressKey);
                            else if(conArgs.TypeUsed == ConsumableType.PLANET)
                                IncrementPersistentProgressCount(PlanetsUsedFromPacksProgressKey);
                        }
                    }
                }
            });
            PersistentProgressListeners.Add(new EngineEventListener
            {
                MyContextType = EventContextType.HandPlayedCalculated,
                MyAction = args =>
                {
                    if(args is EngineHandPlayArgs handArgs)
                        IncrementPersistentProgressCount(CardsPlayedProgressKey, handArgs.CardsSelected.Count);
                }
            });
            PersistentProgressListeners.Add(new EngineEventListener
            {
                MyContextType = EventContextType.HandDiscardDone,
                MyAction = args =>
                {
                    if(args is EngineDiscardDoneArgs discArgs)
                        IncrementPersistentProgressCount(CardsDiscardedProgressKey, discArgs.BeingDiscarded.Count);
                }
            });
            PersistentProgressListeners.Add(new EngineEventListener
            {
                MyContextType = EventContextType.VoucherRedeemed,
                MyAction = args =>
                {
                    if (args is EngineVoucherRedeemedArgs voucherArgs && voucherArgs.BeingRedeemed.JokerData != null && voucherArgs.BeingRedeemed.JokerData.DBName == "BLANK")
                        IncrementPersistentProgressCount(BlanksRedeemedProgressKey);
                }
            });

            foreach (var listener in PersistentProgressListeners)
                EngineEventHandler.StartListening(listener);
        }

        private static void StopPersistentProgressListeners()
        {
            foreach (var listener in PersistentProgressListeners)
            {
                EngineEventHandler.StopListening(listener);
            }
            PersistentProgressListeners.Clear();
        }

        private static void IncrementPersistentProgressCount(string progressKey, int amount = 1)
        {
            if (string.IsNullOrWhiteSpace(progressKey) || amount <= 0)
            {
                return;
            }

            PersistentProgressCounts[progressKey] = GetPersistentProgressCount(progressKey) + amount;
            EngineEventHandler.TriggerEvent(new EngineEventArgs { MyContext = new EventContext { Context = EventContextType.AchievementProgressChanged } });
            SaveProgress();
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

            public static UnlockSaveData FromCurrentState(IEnumerable<string> unlockedDecks, IReadOnlyDictionary<string, int> highestBeatenStakeIndexes, IReadOnlyDictionary<string, int> jokerHighestBeatenStakeIndexes, IEnumerable<string> achievedAchievements, IReadOnlyDictionary<string, int> progressCounts, IEnumerable<string> collectedJokers, IEnumerable<string> collectedConsumables)
            {
                return new UnlockSaveData
                {
                    Decks = new DeckUnlockSaveData
                    {
                        Unlocked = unlockedDecks.OrderBy(x => x).ToList(),
                        HighestBeatenStakeIndexes = highestBeatenStakeIndexes.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value),
                    },
                    Achievements = new AchievementUnlockSaveData
                    {
                        Achieved = achievedAchievements.OrderBy(x => x).ToList(),
                        ProgressCounts = progressCounts.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value),
                    },
                    Collection = new CollectionSaveData
                    {
                        Jokers = collectedJokers.OrderBy(x => x).ToList(),
                        JokerHighestBeatenStakeIndexes = jokerHighestBeatenStakeIndexes.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value),
                        Consumables = collectedConsumables.OrderBy(x => x).ToList(),
                    },
                };
            }
        }

        private sealed class DeckUnlockSaveData
        {
            public List<string> Unlocked { get; set; } = new();
            public Dictionary<string, int> HighestBeatenStakeIndexes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        }

        private sealed class AchievementUnlockSaveData
        {
            public List<string> Achieved { get; set; } = new();
            public Dictionary<string, int> ProgressCounts { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        }

        private sealed class CollectionSaveData
        {
            public List<string> Jokers { get; set; } = new();
            public Dictionary<string, int> JokerHighestBeatenStakeIndexes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
            public List<string> Consumables { get; set; } = new();
        }
    }
}
