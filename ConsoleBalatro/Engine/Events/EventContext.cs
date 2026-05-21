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
        CardBuy,
        CardSelect,
        CardDiscarded,
        CardDestroyed,
        Reroll,
        PackOpen,
        PackSkip,
        PackOddsEstablished,
        CardDrawnToZone,

        StartPlayRound,
        StartPlayRoundSetupOver,
        EndPlayRound,

        CardDetailsChange,

        CardSuitPull,

        CardDiscardedFromHand,

        CardsSelectedForPlay,
        HandPlayedCalculated,

        HandDiscardDone,
        HandPlayScoringDone,
        HandPlayDone,

        SelectedCardBeingConsideredForCalc,
        CardInHandAfterScoring,
        AllScoringCardsDecided,

        TotalChipsGained,
        RequiredChipsSet,

        GainEmit,
        MoneyGainEmit,

        GameStatePush,
        GameStatePop,

        PostGameStatePush,
        PostGameStatePop,

        StartPostRound,
        EndPostRound,

        StartMarket,
        EndMarket,

        StartBlindSelection,
        EndBlindSelection,

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
    }
    public class EventContext
    {
        public EventContextType Context;
        public ScoringContext ScoringContext = null;
    }
}
