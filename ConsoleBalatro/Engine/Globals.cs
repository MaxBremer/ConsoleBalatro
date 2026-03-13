using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public enum BlindType
    {
        SMALL,
        BIG,
        BOSS,
    }
    public static class Globals
    {
        //SETTINGS
        public const bool GUARANTEE_UNIQUE_TAGS = false;
        private static int _reqChipsBlind = -1;

        public static Dictionary<Rank, int> RankChipVals = new()
        {
            {Rank.TWO, 2},
            {Rank.THREE, 3},
            {Rank.FOUR, 4},
            {Rank.FIVE, 5},
            {Rank.SIX, 6},
            {Rank.SEVEN, 7},
            {Rank.EIGHT, 8},
            {Rank.NINE, 9},
            {Rank.TEN, 10},
            {Rank.JACK, 10},
            {Rank.QUEEN, 10},
            {Rank.KING, 10},
            {Rank.ACE, 11},
        };

        public static int CurrentChips = 0;
        public static double CurrentMult = 0;

        public static double DiscountMultiplier = 1.0;//For price discounting effects

        public static int RequiredChipsForCurrentBlind
        {
            get => _reqChipsBlind;
            set
            {
                var args = new EngineRequirementSetArgs() { RequirementBeingSet = value, MyContext = new Events.EventContext() { Context = Events.EventContextType.RequiredChipsSet } };
                EngineEventHandler.TriggerEvent(args);
                _reqChipsBlind = args.RequirementBeingSet;
            }
        }

        public static Stack<GameStateObj> GameStateStack = new();
        public static GameStateObj CurrentGameStateObj => GameStateStack.Peek();
        public static GameState CurrentGameState => CurrentGameStateObj.GameState;

        public static bool QUIT = false;

        public static int TotalCurrentChips = 0;

        public static int Money = 0;
        public static int MinimumMoneyAllowed = 0;
        public static int BaseRerollCost = 5;
        public static int CurrentRerollCost = BaseRerollCost;
        public static Card RerollButtonCard;

        public static int HandSize { get => ZoneManager.HandSize; set => ZoneManager.HandSize = value; }

        public static int StartingHandSize = 8;

        public static int MainMarketCount = 2;
        public static int PackMarketCount = 2;
        public static int VoucherMarketCount = 1;

        public static int MaxHandsPerRound = 4;
        public static int MaxDiscardsPerRoudn = 3;

        public static int CurMaxInterest = 5;

        private static int _curDisc;
        private static int _curHands;

        public static int CurHandsRemaining
        {
            get => _curHands;
            set
            {
                var args = new EngineHandDiscChangeArgs();
                args.oldVal = _curHands;
                args.newVal = value;
                args.isHand = true;
                args.MyContext = new EventContext() { Context = EventContextType.HandsChange };
                EngineEventHandler.TriggerEvent(args);
                _curHands = value;
            }
        }

        public static int CurDiscardsRemaining
        {
            get => _curDisc;
            set
            {
                var args = new EngineHandDiscChangeArgs();
                args.oldVal = _curDisc;
                args.newVal = value;
                args.isHand = false;
                args.MyContext = new EventContext() { Context = EventContextType.DiscardsChange };
                EngineEventHandler.TriggerEvent(args);
                _curDisc = value;
            }
        }

        public static bool CanDiscard => CurDiscardsRemaining > 0;

        public static void InitializeMain()
        {
            //DO
        }

        public static void PlayCurrentlySelectedHand()
        {
            //DO
        }

        public static void DiscardSelectedFromHand(bool doRedraw = true)
        {
            //DO
        }

        public static void EmitChipsAdd(int chipsNum, Card src)
        {
            var emitArgs = new EngineChipsMultGainEmitArgs() { ChipsGainEmitted = chipsNum, MyContext = new EventContext() { Context = EventContextType.GainEmit }, SourceOfEmit = src };
            EngineEventHandler.TriggerEvent(emitArgs);

            CurrentChips += emitArgs.ChipsGainEmitted;
        }

        public static void EmitMultAdd(double multGain, Card src)
        {
            var emitArgs = new EngineChipsMultGainEmitArgs() { MultGainEmitted = multGain, MyContext = new EventContext() { Context = EventContextType.GainEmit }, SourceOfEmit = src };
            EngineEventHandler.TriggerEvent(emitArgs);

            CurrentMult += emitArgs.MultGainEmitted;
        }

        public static void EmitMultMult(double multMultGain, Card src)
        {
            var emitArgs = new EngineChipsMultGainEmitArgs() { MultMultEmitted = multMultGain, MyContext = new EventContext() { Context = EventContextType.GainEmit }, SourceOfEmit = src };
            EngineEventHandler.TriggerEvent(emitArgs);

            CurrentMult *= emitArgs.MultMultEmitted;
        }

        public static void EmitMoneyGain(int moneyAmt, Card src)
        {
            var emitArgs = new EngineGoldGainEmitArgs() { AmountGained = moneyAmt, MyContext = new EventContext() { Context = EventContextType.MoneyGainEmit }, SourceOfEmit = src };
            EngineEventHandler.TriggerEvent(emitArgs);

            Money += emitArgs.AmountGained;
        }

        //Differentiated from above for some edge cases.
        public static void EmitMoneyLoss(int moneyAmt, Card src, bool isPurchase)
        {
            //TODO: Might differentiate event as well later idk.
            var emitArgs = new EngineGoldGainEmitArgs() { AmountGained = -1 * moneyAmt, MyContext = new EventContext() { Context = EventContextType.MoneyGainEmit }, SourceOfEmit = src };
            EngineEventHandler.TriggerEvent(emitArgs);
            Money += emitArgs.AmountGained;
        }

        public static bool CanAfford(int costToCheck)
        {
            return Money - costToCheck >= MinimumMoneyAllowed;
        }

        public static bool CanAfford(Card c)
        {
            return CanAfford(c.BuyCost);
        }

        public static bool CanBePurchased(Card c)
        {
            if (c.isJoker)
            {
                return CanAfford(c) && ZoneManager.JokerZone.HasRoom;
            }
            if (c.isConsumable)
            {
                return CanAfford(c) && ZoneManager.ConsumableZone.HasRoom;
            }
            return CanAfford(c);
        }

        public static void PerformSell(Card beingSold, CardZone zoneCardIsLeaving)
        {
            //DO
        }

        public static void PerformPurchaseByType(Card beingPurchased)
        {
            //DO
        }

        public static void PerformPurchase(Card beingPurchased, CardZone zoneGoingTo = null, CardZone zoneFrom = null)
        {
            //DO
        }

        public static void PerformBuyAndUse(Card beingPurchased)
        {
            //DO
        }

        public static bool CanBuyAndUse(Card beingPurchased)
        {
            //TODO: Consumable usage params??
            return CanAfford(beingPurchased) && beingPurchased.isConsumable && beingPurchased.ConsumableData.IsActivatable(null);
        }

        public static void SetStartOfRoundStats()
        {
            CurHandsRemaining = MaxHandsPerRound;
            CurDiscardsRemaining = MaxDiscardsPerRoudn;
        }

        public static GameStateObj PopCurrGameState()
        {
            //DO
            return null;
        }

        public static void PushGameState(GameStateObj obj)
        {
            //DO
        }

        public static void ClearGameStateStack()
        {
            GameStateStack.Clear();
        }

        public static bool RollRandom(int numerator, int denominator)
        {
            return ChooseRandomInclusive(1, denominator) <= numerator;
        }

        public static int ChooseRandomInclusive(int min, int max)
        {
            return Random.Shared.Next(min, max + 1);
        }
    }
}
