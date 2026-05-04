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
        HandPlayDone,

        SelectedCardBeingConsideredForCalc,
        CardInHandAfterScoring,

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
    }
    public class EventContext
    {
        public EventContextType Context;
        public ScoringContext ScoringContext = null;
    }
}
