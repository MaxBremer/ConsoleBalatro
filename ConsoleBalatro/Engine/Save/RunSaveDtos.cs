using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Tags;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Stakes;
using ConsoleBalatro.Engine.Cards.Jokers;

namespace ConsoleBalatro.Engine.Save
{
    public sealed class RunSaveData
    {
        public int Version { get; set; } = 1;
        public DateTime SavedAtUtc { get; set; } = DateTime.UtcNow;
        public RunGlobalSaveData Globals { get; set; } = new();
        public List<GameStateSaveData> GameStates { get; set; } = new();
        public List<ZoneSaveData> Zones { get; set; } = new();
    }

    public sealed class RunGlobalSaveData
    {
        public int CurrentAnte { get; set; }
        public string CurrentDeckDbName { get; set; } = string.Empty;
        public BlindType CurrentSelectedBlind { get; set; }
        public TagType CurSmallBlindTag { get; set; }
        public TagType CurBigBlindTag { get; set; }
        public string CurrentBossBlind { get; set; } = string.Empty;
        public bool HasWonCurrentRun { get; set; }
        public bool RunWinDecisionPending { get; set; }
        public StakeType CurrentStake { get; set; }
        public int CurrentChips { get; set; }
        public double CurrentMult { get; set; }
        public int RequiredChipsForCurrentBlind { get; set; }
        public int TotalCurrentChips { get; set; }
        public int Money { get; set; }
        public int MinimumMoneyAllowed { get; set; }
        public int BaseRerollCost { get; set; }
        public int CurrentRerollCost { get; set; }
        public bool ChaosClownFreeRerollAvailable { get; set; }
        public int SelectionMax { get; set; }
        public int BaseHandSize { get; set; }
        public int MaxHandsPerRound { get; set; }
        public int MaxDiscardsPerRound { get; set; }
        public int CurMaxInterest { get; set; }
        public int CurrentBossBlindRerollCost { get; set; }
        public int BaseBossBlindRerollsAllowed { get; set; }
        public int CurBossBlindRerollsAllowed { get; set; }
        public double DiscountMultiplier { get; set; }
        public bool ShopPlayingCardsGetModifiers { get; set; }
        public int CurHandsRemaining { get; set; }
        public int CurDiscardsRemaining { get; set; }
        public List<string> Flags { get; set; } = new();
        public List<string> BossBlindsAlreadyUsed { get; set; } = new();
    }

    public sealed class GameStateSaveData
    {
        public GameState GameState { get; set; }
        public int? TargetPackCardId { get; set; }
        public int NumChoicesAlreadyMade { get; set; }
        public int TotalNumChoicesAllowed { get; set; }
        public int PostRoundMoneyToGive { get; set; }
        public List<PostRoundMoneySourceSaveData> PostRoundMoneySources { get; set; } = new();
        public string? SavedHandOrder { get; set; }
        public string? SavedDeckOrder { get; set; }
        public string? SavedDiscardOrder { get; set; }
        public string? SavedHiddenPlayOrder { get; set; }
    }

    public sealed class PostRoundMoneySourceSaveData
    {
        public string Source { get; set; } = string.Empty;
        public int Amount { get; set; }
    }

    public sealed class ZoneSaveData
    {
        public string Name { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public List<CardSaveData> Cards { get; set; } = new();
    }

    public sealed class CardSaveData
    {
        public int Id { get; set; }
        public CardKind Kind { get; set; }
        public Rank Rank { get; set; }
        public Suit Suit { get; set; }
        public Enhancement Enhancement { get; set; }
        public Edition Edition { get; set; }
        public Seal Seal { get; set; }
        public List<Sticker> Stickers { get; set; } = new();
        public int ChipsBase { get; set; }
        public double MultBase { get; set; }
        public double MultMultBase { get; set; }
        public bool ForcedSelect { get; set; }
        public bool IsSelected { get; set; }
        public bool Debuffed { get; set; }
        public bool DebuffedByBoss { get; set; }
        public bool FaceDown { get; set; }
        public int PerishCountdownVal { get; set; }
        public int BaseCost { get; set; }
        public int? BuyCostOverride { get; set; }
        public int BonusSellValue { get; set; }
        public string? DbName { get; set; }
        public PackType PackType { get; set; }
        public ConsumableType ConsumableType { get; set; }
        public PlayedHandType PlanetHandType { get; set; }
        public Dictionary<string, JokerDataSaveData> DataDict { get; set; } = new();
    }

    public sealed class JokerDataSaveData
    {
        public JokerDataType MyDataType { get; set; }
        public int IntData { get; set; }
        public double DoubleData { get; set; }
        public bool BoolData { get; set; }
        public PlayedHandType HandTypeData { get; set; }
        public Rank SpecificCardRank { get; set; }
        public Suit SpecificCardSuit { get; set; }
        public List<string> StringList { get; set; } = new();
        public int? CardDataId { get; set; }
    }

    public enum CardKind
    {
        PlayingCard,
        Joker,
        Voucher,
        Tag,
        Tarot,
        Planet,
        Spectral,
        Pack,
        Unknown
    }
}
