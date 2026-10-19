using System.Text.Json;
using System.Text.Json.Serialization;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Tags;
using ConsoleBalatro.Engine.Cards.Vouchers;
using ConsoleBalatro.Engine.Stakes;

namespace ConsoleBalatro.Engine.Save
{
    public static class RunSaveManager
    {
        private static readonly object SaveLock = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static string SaveFilePath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ConsoleBalatro",
            "run-save.json");

        public static bool HasSave => File.Exists(SaveFilePath);

        public static void SaveAndQuit()
        {
            var saveData = CaptureCurrentRun();
            var saveDirectory = Path.GetDirectoryName(SaveFilePath);
            if (!string.IsNullOrWhiteSpace(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            var tempPath = SaveFilePath + ".tmp";
            var json = JsonSerializer.Serialize(saveData, JsonOptions);

            lock (SaveLock)
            {
                File.WriteAllText(tempPath, json);
                File.Move(tempPath, SaveFilePath, true);
            }

            Globals.QUIT = true;
        }

        public static bool TryLoadRun()
        {
            lock (SaveLock)
            {
                if (!File.Exists(SaveFilePath))
                    return false;

                try
                {
                    var saveData = JsonSerializer.Deserialize<RunSaveData>(File.ReadAllText(SaveFilePath), JsonOptions);
                    if (saveData == null)
                        return false;

                    RestoreRun(saveData);
                    return true;
                }
                catch (JsonException)
                {
                    return false;
                }
                catch (IOException)
                {
                    return false;
                }
            }
        }

        public static void DeleteSave()
        {
            lock (SaveLock)
            {
                if (File.Exists(SaveFilePath))
                    File.Delete(SaveFilePath);

                var tempPath = SaveFilePath + ".tmp";
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }

        public static RunSaveData CaptureCurrentRun()
        {
            return new RunSaveData
            {
                SavedAtUtc = DateTime.UtcNow,
                Globals = CaptureGlobals(),
                GameStates = Globals.GameStateStack.Reverse().Select(CaptureGameState).ToList(),
                Zones = CaptureZones().ToList()
            };
        }

        public static void RestoreRun(RunSaveData saveData)
        {
            ArgumentNullException.ThrowIfNull(saveData);

            Globals.ResetFullEngine();
            DataManager.CardsByID.Clear();
            ClearAllManagedZones();

            var cardsById = new Dictionary<int, Card>();
            foreach (var zoneData in saveData.Zones)
            {
                var zone = GetZoneByName(zoneData.Name);
                if (zone == null)
                    continue;

                zone.MaxCapacity = zoneData.MaxCapacity;
                zone.ClearCards(invisibleRemove: true);
                foreach (var cardData in zoneData.Cards)
                {
                    var card = RestoreCard(cardData);
                    cardsById[card.ID] = card;
                    zone.AddCard(card, invisibleAdd: true, overrideSpace: true);
                }
            }

            RestoreGlobals(saveData.Globals);
            Globals.ClearGameStateStack();
            foreach (var stateData in saveData.GameStates)
                Globals.GameStateStack.Push(RestoreGameState(stateData, cardsById));
        }

        private static RunGlobalSaveData CaptureGlobals() => new()
        {
            CurrentAnte = FlowHandler.CurrentAnte,
            CurrentDeckDbName = FlowHandler.CurrentDeckDbName,
            CurrentSelectedBlind = FlowHandler.CurrentSelectedBlind,
            CurSmallBlindTag = FlowHandler.CurSmallBlindTag,
            CurBigBlindTag = FlowHandler.CurBigBlindTag,
            CurrentBossBlind = FlowHandler.CurrentBossBlind,
            HasWonCurrentRun = FlowHandler.HasWonCurrentRun,
            RunWinDecisionPending = FlowHandler.RunWinDecisionPending,
            CurrentStake = StakeManager.CurrentStake,
            CurrentChips = Globals.CurrentChips,
            CurrentMult = Globals.CurrentMult,
            RequiredChipsForCurrentBlind = Globals.RequiredChipsForCurrentBlind,
            TotalCurrentChips = Globals.TotalCurrentChips,
            Money = Globals.Money,
            MinimumMoneyAllowed = Globals.MinimumMoneyAllowed,
            BaseRerollCost = Globals.BaseRerollCost,
            CurrentRerollCost = Globals.CurrentRerollCost,
            ChaosClownFreeRerollAvailable = Globals.ChaosClownFreeRerollAvailable,
            SelectionMax = Globals.SelectionMax,
            BaseHandSize = Globals.BaseHandSize,
            MaxHandsPerRound = Globals.MaxHandsPerRound,
            MaxDiscardsPerRound = Globals.MaxDiscardsPerRound,
            CurMaxInterest = Globals.CurMaxInterest,
            CurrentBossBlindRerollCost = Globals.CurrentBossBlindRerollCost,
            BaseBossBlindRerollsAllowed = Globals.BaseBossBlindRerollsAllowed,
            CurBossBlindRerollsAllowed = Globals.CurBossBlindRerollsAllowed,
            DiscountMultiplier = Globals.DiscountMultiplier,
            ShopPlayingCardsGetModifiers = Globals.ShopPlayingCardsGetModifiers,
            CurHandsRemaining = Globals.CurHandsRemaining,
            CurDiscardsRemaining = Globals.CurDiscardsRemaining,
            Flags = Globals.Flags.ToList(),
            BossBlindsAlreadyUsed = BossBlindDb.BossBlindsAlreadyUsed.ToList()
        };

        private static void RestoreGlobals(RunGlobalSaveData data)
        {
            FlowHandler.CurrentAnte = data.CurrentAnte;
            FlowHandler.CurrentDeckDbName = data.CurrentDeckDbName ?? string.Empty;
            FlowHandler.CurrentSelectedBlind = data.CurrentSelectedBlind;
            FlowHandler.CurSmallBlindTag = data.CurSmallBlindTag;
            FlowHandler.CurBigBlindTag = data.CurBigBlindTag;
            FlowHandler.CurrentBossBlind = data.CurrentBossBlind ?? string.Empty;
            FlowHandler.HasWonCurrentRun = data.HasWonCurrentRun;
            FlowHandler.RunWinDecisionPending = data.RunWinDecisionPending;
            StakeManager.CurrentStake = data.CurrentStake;
            Globals.CurrentChips = data.CurrentChips;
            Globals.CurrentMult = data.CurrentMult;
            Globals.RequiredChipsForCurrentBlind = data.RequiredChipsForCurrentBlind;
            Globals.TotalCurrentChips = data.TotalCurrentChips;
            Globals.Money = data.Money;
            Globals.MinimumMoneyAllowed = data.MinimumMoneyAllowed;
            Globals.BaseRerollCost = data.BaseRerollCost;
            Globals.CurrentRerollCost = data.CurrentRerollCost;
            Globals.ChaosClownFreeRerollAvailable = data.ChaosClownFreeRerollAvailable;
            Globals.SelectionMax = data.SelectionMax;
            Globals.BaseHandSize = data.BaseHandSize;
            Globals.MaxHandsPerRound = data.MaxHandsPerRound;
            Globals.MaxDiscardsPerRound = data.MaxDiscardsPerRound;
            Globals.CurMaxInterest = data.CurMaxInterest;
            Globals.CurrentBossBlindRerollCost = data.CurrentBossBlindRerollCost;
            Globals.BaseBossBlindRerollsAllowed = data.BaseBossBlindRerollsAllowed;
            Globals.CurBossBlindRerollsAllowed = data.CurBossBlindRerollsAllowed;
            Globals.DiscountMultiplier = data.DiscountMultiplier;
            Globals.ShopPlayingCardsGetModifiers = data.ShopPlayingCardsGetModifiers;
            Globals.CurHandsRemaining = data.CurHandsRemaining;
            Globals.CurDiscardsRemaining = data.CurDiscardsRemaining;
            Globals.Flags = data.Flags.ToHashSet();
            BossBlindDb.BossBlindsAlreadyUsed.Clear();
            foreach (var boss in data.BossBlindsAlreadyUsed)
                BossBlindDb.BossBlindsAlreadyUsed.Add(boss);
        }

        private static IEnumerable<ZoneSaveData> CaptureZones()
        {
            foreach (var (name, zone) in GetZones())
            {
                if (zone == null)
                    continue;

                yield return new ZoneSaveData
                {
                    Name = name,
                    MaxCapacity = zone.MaxCapacity,
                    Cards = zone.Cards.Select(CaptureCard).ToList()
                };
            }
        }

        private static CardSaveData CaptureCard(Card card)
        {
            var kind = GetCardKind(card);
            return new CardSaveData
            {
                Id = card.ID,
                Kind = kind,
                Rank = card.Rank,
                Suit = card.Suit,
                Enhancement = card.Enhancement,
                Edition = card.Edition,
                Seal = card.Seal,
                Stickers = card.Stickers.ToList(),
                ChipsBase = card.ChipsBase,
                MultBase = card.MultBase,
                MultMultBase = card.MultMultBase,
                ForcedSelect = card.ForcedSelect,
                IsSelected = card.isSelected,
                Debuffed = card.Debuffed,
                DebuffedByBoss = card.DebuffedByBoss,
                FaceDown = card.FaceDown,
                PerishCountdownVal = card.PerishCountdownVal,
                BaseCost = card.BaseCost,
                BuyCostOverride = card.BuyCostOverride,
                BonusSellValue = card.BonusSellValue,
                DbName = card.IsTag ? card.TagData?.MyType.ToString() : card.JokerData?.DBName ?? card.ConsumableData?.DBName,
                PackType = card.MyPackType,
                ConsumableType = card.ConsumableData?.Type ?? ConsumableType.TAROT,
                PlanetHandType = card.ConsumableData?.PlanetHandType ?? PlayedHandType.HIGHCARD,
                DataDict = CaptureDataDict(card)
            };
        }

        private static GameStateSaveData CaptureGameState(GameStateObj state) => new()
        {
            GameState = state.GameState,
            TargetPackCardId = state.TargetPack?.ID,
            NumChoicesAlreadyMade = state.NumChoicesAlreadyMade,
            TotalNumChoicesAllowed = state.TotalNumChoicesAllowed,
            PostRoundMoneyToGive = state.PostRoundMoneyToGive,
            PostRoundMoneySources = state.PostRoundMoneySources.Select(x => new PostRoundMoneySourceSaveData { Source = x.Item1, Amount = x.Item2 }).ToList(),
            SavedHandOrder = state.SavedHandOrder,
            SavedDeckOrder = state.SavedDeckOrder,
            SavedDiscardOrder = state.SavedDiscardOrder,
            SavedHiddenPlayOrder = state.SavedHiddenPlayOrder
        };

        private static GameStateObj RestoreGameState(GameStateSaveData data, IReadOnlyDictionary<int, Card> cardsById)
        {
            var state = new GameStateObj
            {
                GameState = data.GameState,
                NumChoicesAlreadyMade = data.NumChoicesAlreadyMade,
                TotalNumChoicesAllowed = data.TotalNumChoicesAllowed,
                PostRoundMoneyToGive = data.PostRoundMoneyToGive,
                SavedHandOrder = data.SavedHandOrder,
                SavedDeckOrder = data.SavedDeckOrder,
                SavedDiscardOrder = data.SavedDiscardOrder,
                SavedHiddenPlayOrder = data.SavedHiddenPlayOrder
            };

            if (data.TargetPackCardId.HasValue && cardsById.TryGetValue(data.TargetPackCardId.Value, out var packCard))
                state.TargetPack = packCard;

            state.PostRoundMoneySources.AddRange(data.PostRoundMoneySources.Select(x => (x.Source, x.Amount)));
            return state;
        }

        private static Card RestoreCard(CardSaveData data)
        {
            var card = data.Kind switch
            {
                CardKind.Joker when !string.IsNullOrWhiteSpace(data.DbName) && JokerDb.JokerData.ContainsKey(data.DbName) => JokerDb.GenerateJokerCard(data.DbName),
                CardKind.Voucher when !string.IsNullOrWhiteSpace(data.DbName) && VoucherDb.VoucherData.ContainsKey(data.DbName) => VoucherDb.MakeVoucherCard(data.DbName),
                CardKind.Tag => GenerateTagCard(data),
                CardKind.Tarot when !string.IsNullOrWhiteSpace(data.DbName) => ConsumableManager.MakeTarotCard(data.DbName) ?? new Card(),
                CardKind.Planet when !string.IsNullOrWhiteSpace(data.DbName) => ConsumableManager.MakePlanetCard(data.DbName),
                CardKind.Spectral when !string.IsNullOrWhiteSpace(data.DbName) => ConsumableManager.MakeSpectralCard(data.DbName) ?? new Card(),
                CardKind.Pack => ConsumableManager.MakePack(data.PackType),
                _ => CardFactory.PlayingCardFromRankSuit(data.Rank, data.Suit)
            };

            var generatedId = card.ID;
            if (DataManager.CardsByID.ContainsKey(generatedId))
                DataManager.CardsByID.Remove(generatedId);
            card.ID = data.Id;
            DataManager.CardsByID[data.Id] = card;

            card.Rank = data.Rank;
            card.Suit = data.Suit;
            card.Enhancement = data.Enhancement;
            card.Edition = data.Edition;
            card.Seal = data.Seal;
            card.Stickers.Clear();
            card.Stickers.AddRange(data.Stickers);
            card.ChipsBase = data.ChipsBase;
            card.MultBase = data.MultBase;
            card.MultMultBase = data.MultMultBase;
            card.ForcedSelect = data.ForcedSelect;
            card.isSelected = data.IsSelected;
            card.DebuffedByBoss = data.DebuffedByBoss;
            card.Debuffed = data.Debuffed;
            card.FaceDown = data.FaceDown;
            card.PerishCountdownVal = data.PerishCountdownVal;
            card.BaseCost = data.BaseCost;
            card.BuyCostOverride = data.BuyCostOverride;
            card.BonusSellValue = data.BonusSellValue;
            RestoreDataDict(card, data);
            return card;
        }

        private static Dictionary<string, JokerDataSaveData> CaptureDataDict(Card card)
        {
            var source = card.JokerData?.DataDict ?? card.ConsumableData?.DataDict;
            if (source == null)
                return new();

            return source.ToDictionary(kvp => kvp.Key, kvp => new JokerDataSaveData
            {
                MyDataType = kvp.Value.MyDataType,
                IntData = kvp.Value.IntData,
                DoubleData = kvp.Value.DoubleData,
                BoolData = kvp.Value.BoolData,
                HandTypeData = kvp.Value.HandTypeData,
                SpecificCardRank = kvp.Value.SpecificCardRank,
                SpecificCardSuit = kvp.Value.SpecificCardSuit,
                StringList = kvp.Value.StringList.ToList(),
                CardDataId = kvp.Value.CardData?.ID
            });
        }

        private static void RestoreDataDict(Card card, CardSaveData data)
        {
            var target = card.JokerData?.DataDict ?? card.ConsumableData?.DataDict;
            if (target == null)
                return;

            foreach (var kvp in data.DataDict)
            {
                target[kvp.Key] = new JokerData
                {
                    MyDataType = kvp.Value.MyDataType,
                    IntData = kvp.Value.IntData,
                    DoubleData = kvp.Value.DoubleData,
                    BoolData = kvp.Value.BoolData,
                    HandTypeData = kvp.Value.HandTypeData,
                    SpecificCardRank = kvp.Value.SpecificCardRank,
                    SpecificCardSuit = kvp.Value.SpecificCardSuit,
                    StringList = kvp.Value.StringList.ToList()
                };
            }
        }

        private static Card GenerateTagCard(CardSaveData data)
        {
            var card = new Card();
            if (Enum.TryParse<TagType>(data.DbName, out var tagType) && TagDb.TagBuilders.ContainsKey(tagType))
                TagDb.MakeCardTagOfType(card, tagType);
            return card;
        }

        private static CardKind GetCardKind(Card card)
        {
            if (card.IsVoucher) return CardKind.Voucher;
            if (card.IsTag) return CardKind.Tag;
            if (card.IsJoker) return CardKind.Joker;
            if (card.isPack) return CardKind.Pack;
            if (card.isConsumable)
            {
                return card.ConsumableData.Type switch
                {
                    ConsumableType.TAROT => CardKind.Tarot,
                    ConsumableType.PLANET => CardKind.Planet,
                    ConsumableType.SPECTRAL => CardKind.Spectral,
                    _ => CardKind.Unknown
                };
            }
            return card.isPlayingCard ? CardKind.PlayingCard : CardKind.Unknown;
        }

        private static IEnumerable<(string Name, CardZone Zone)> GetZones()
        {
            yield return ("Deck", ZoneManager.DeckZone);
            yield return ("Jokers", ZoneManager.JokerZone);
            yield return ("Consumables", ZoneManager.ConsumableZone);
            yield return ("Tags", ZoneManager.TagZone);
            yield return ("Hand", ZoneManager.HandZone);
            yield return ("CurrentlyBeingPlayed", ZoneManager.CurrentlyBeingPlayedZone);
            yield return ("MainMarket", ZoneManager.MainMarketZone);
            yield return ("PacksMarket", ZoneManager.PackMarketZone);
            yield return ("VouchersMarket", ZoneManager.VoucherMarketZone);
            yield return ("PackOptions", ZoneManager.PackOptionZone);
            yield return ("Discard", ZoneManager.DiscardZone);
            yield return ("PreDestroy", ZoneManager.PreDestructionZone);
            yield return ("Destruction", ZoneManager.DestructionZone);
            yield return ("Played", ZoneManager.HiddenPlayZone);
            yield return ("ActiveVouchers", ZoneManager.ActiveVoucherZone);
            yield return ("CurrentConsumable", ZoneManager.CurrentlyActivatingConsumable);
            yield return ("Blinds", ZoneManager.HiddenBlindAttributeZone);
            yield return ("PermanentEffects", ZoneManager.OtherHiddenJokerZone);
        }

        private static CardZone? GetZoneByName(string name) => GetZones().FirstOrDefault(x => x.Name == name).Zone;

        private static void ClearAllManagedZones()
        {
            foreach (var (_, zone) in GetZones())
                zone?.ClearCards(invisibleRemove: true);
        }
    }
}
