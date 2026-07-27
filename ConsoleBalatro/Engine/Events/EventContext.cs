using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events
{
    public enum EventContextType
    {
        NONE,

        CardTrigger,
        CardPreTrigger,

        CardSell,
        CardPurchased,
        CardSelect,
        CardDiscarded,
        CardDestroyed,
        CardPositionsSwapping,
        CardPositionsSwapDone,
        MarketRerolled,
        PackOpen,
        PackSkip,
        PackOddsEstablished,
        CardDrawnToZone,
        VISUALCardDrawnToZone,

        StartPlayRound,
        StartPlayRoundSetupOver,
        EndPlayRound,

        AnteChange,
        RunWon,
        RunLost,

        DrawHandfulStarted,
        DrawHandfulDone,

        CardDetailsChange,
        CardTogglingSelection,

        CardSuitPull,

        CardManuallyDiscardedFromHand,

        CardsSelectedForPlay,
        HandPlayedCalculated,

        HandDiscardDone,
        HandPlayScoringDone,
        HandPlayDone,

        SelectedCardBeingConsideredForCalc,
        CardInHandAfterScoring,
        AllScoringCardsDecided,

        GetBlindChips,
        PreFinalGainCheck,
        TotalChipsGained,
        TotalChipsReset,
        RequiredChipsSet,

        HandLevelChange,

        GainEmit,
        MoneyGainEmit,

        GameStatePush,
        GameStatePop,
        GameStateReplace,

        PostGameStatePush,
        PostGameStatePop,
        PostGameStateReplace,

        StartPostRound,
        EndPostRound,

        StartMarket,
        EndMarket,

        StartBlindSelection,
        EndBlindSelection,

        StartDeckSelection,
        EndDeckSelection,

        StartSelectedBlind,

        BlindChange,
        BlindSkip,

        GatherPostRoundMoney,

        MarketSetupDone,

        HandsChange,
        DiscardsChange,

        ConsumableUsed,

        TagAdded,
        TagActivatedViaListener,
        TagActivatedInstantly,

        RandomRollHappening,
        MarketTypeBeingChosen,

        LuckyCardSuccessfulTrigger,

        BossAbilityTriggeredByHand,

        SettingBaseChipsMult,

        ZoneShuffling,

        AchievementUnlocked,
        CollectionItemAdded,
        AchievementProgressChanged,

        RolledCardGenerated,

        VoucherRedeemed,

        HandSizeChanged,
    }
    public class EventContext
    {
        public EventContextType Context;
        public ScoringContext ScoringContext = null;
    }
}
