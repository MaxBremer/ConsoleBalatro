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

        GatherPostRoundMoney,

        HandsChange,
        DiscardsChange,

        ConsumableUsed,

        TagAdded,
    }
    public class EventContext
    {
        public EventContextType Context;
        public ScoringContext ScoringContext = null;
    }
}
