using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Vouchers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using ConsoleBalatro.Engine.Pools;
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
        public const bool USE_DEFAULT_JOKER_IF_POOL_EMPTY = true;
        public const bool MIRROR_ILLUSION_SEAL_GLITCH = false;
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
        public static bool ChaosClownFreeRerollAvailable = false;
        public static Card RerollButtonCard;

        public static int HandSize { get => ZoneManager.HandSize; set => ZoneManager.HandSize = value; }

        public static int SelectionMax = 5;

        public static int CurNumCardsSelected => ZoneManager.CardsSelectedInHand.Count();

        public static int BaseHandSize = 8;

        public const int BaseMainMarketCount = 2;
        public const int BasePackMarketCount = 2;
        public const int BaseVoucherMarketCount = 1;

        public static int MaxHandsPerRound = 4;
        public static int MaxDiscardsPerRound = 3;

        public static int CurMaxInterest = 5;

        public static int BaseBossBlindRerollsAllowed = 0;
        public static int CurBossBlindRerollsAllowed = 0;
        public static bool CanRerollBossBlind => CurBossBlindRerollsAllowed != 0;

        public static bool ShopPlayingCardsGetModifiers = false;//NOTE: TEMP. 

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
            ZoneManager.InitializeMainGameZones();

            ScoreHandler.InitializeHandStatTracker();

            GlobalEventListeners.SetupGlobalListeners();

            //IMPORTANT TO DO THIS BEFORE INITIALIZING MARKET POOLS
            VoucherDb.ResetDependants();

            MarketOptionsManager.InitializeMarketPools();
            MarketOptionsManager.ShufflePools();

            FlowHandler.InitializeFlowListeners();

            PackActions.InitializePackData();

            PoolManager.Initialize();

            RerollButtonCard = new();
        }

        public static void ResetGlobalValues()
        {
            TotalCurrentChips = 0;
            Money = 0;
            MinimumMoneyAllowed = 0;
            BaseRerollCost = 5;
            CurrentRerollCost = BaseRerollCost;
            ChaosClownFreeRerollAvailable = false;

            SelectionMax = 5;
            BaseHandSize = 8;

            MaxHandsPerRound = 4;
            MaxDiscardsPerRound = 3;

            CurMaxInterest = 5;

            CurBossBlindRerollsAllowed = 0;
            BaseBossBlindRerollsAllowed = 0;

            DiscountMultiplier = 1.0;

            ShopPlayingCardsGetModifiers = false;

            EngineUtils.ResetUtilValues();
        }

        public static void PlayCurrentlySelectedHand()
        {
            if(CurrentGameState != GameState.PlayRound || CurNumCardsSelected == 0)
            {
                return;
            }

            var selCards = ZoneManager.CardsSelectedInHand;

            var evArgs = new EngineHandPlayArgs()
            {
                CardsSelected = selCards,
                PreHandTypeCalculation = true,
                MyContext = new EventContext() { Context = EventContextType.CardsSelectedForPlay },
            };
            EngineEventHandler.TriggerEvent(evArgs);

            ZoneManager.CurrentlyBeingPlayedZone.DrawTargetsFrom(ZoneManager.HandZone, selCards);
            var handSelInfo = EngineUtils.BestHandFromCards(selCards);

            var bestHandArgs = new EngineHandPlayArgs()
            {
                CardsSelected = selCards,
                MyContext = new EventContext() { Context = EventContextType.HandPlayedCalculated },
                HandBeingPlayed = handSelInfo.Item1,
                CardsInScoringHand = handSelInfo.Item2,
            };
            EngineEventHandler.TriggerEvent(bestHandArgs);

            ScoreHandler.SetBaseHandScore(bestHandArgs.HandBeingPlayed);

            var handTypePlayed = bestHandArgs.HandBeingPlayed;
            var cardsInActualHandPlayed = bestHandArgs.CardsInScoringHand;

            //Find the cards we're going to use in the scoring calc.
            var cardsForScoringCalc = new List<Card>();
            foreach (var cardSelected in selCards)
            {
                bool addToScoringCalc = cardsInActualHandPlayed.Contains(cardSelected);
                var considerationArgs = new EngineCardChosenForPlayedHandArgs()
                {
                    CardBeingConsidered = cardSelected,
                    WillBeIncludedInCalc = addToScoringCalc,
                    MyContext = new EventContext() { Context = EventContextType.SelectedCardBeingConsideredForCalc },
                };
                EngineEventHandler.TriggerEvent(considerationArgs);

                if (considerationArgs.WillBeIncludedInCalc)
                {
                    cardsForScoringCalc.Add(cardSelected);
                }
            }

            var sContext = new ScoringContext() { HandBeingPlayed = handTypePlayed, };
            EngineEventHandler.TriggerEvent(new EngineHandPlayArgs()
            {
                MyContext = new EventContext() { Context = EventContextType.AllScoringCardsDecided },
                CardsInScoringHand = cardsForScoringCalc,
                HandBeingPlayed = handTypePlayed,
            });
            sContext.PlayingCardsBeingScored.AddRange(cardsInActualHandPlayed);//Uhhh shouldn't this be cardsForScoringCalc? TODO
            //like honestly wtf did I write here?
            sContext.AllPlayingCardsSubmittedForHand.AddRange(selCards);//See this one makes sense
            foreach (var cardScored in cardsForScoringCalc)//see this makes sense
            {
                cardScored.TriggerScoring(sContext);
            }

            foreach (var cardInHand in ZoneManager.HandZone.Cards)
            {
                cardInHand.TriggerInHandDuringScoring(sContext);
            }

            foreach (var jokerCard in ZoneManager.JokerZone.Cards)
            {
                jokerCard.TriggerScoring(sContext);
            }

            foreach (var voucherCard in ZoneManager.ActiveVoucherZone.Cards)
            {
                voucherCard.TriggerScoring(sContext);//TODO: Put all always-score cards (like jokers, vouchers, boss blind jokers) in one always-score list, then score that.
            }

            ScoreHandler.FinalPlayChipsCalc();
            EngineEventHandler.TriggerEvent(new EngineEventArgs()
            {
                MyContext = new EventContext() { Context = EventContextType.HandPlayScoringDone },
            });
            ZoneManager.HiddenPlayZone.DrawUntilCapacityFrom(ZoneManager.CurrentlyBeingPlayedZone);
            CurHandsRemaining -= 1;

            var handPlayDoneArgs = new EngineHandPlayDoneArgs()
            {
                MyContext = new EventContext() { Context = EventContextType.HandPlayDone },
                HandTypeThatWasPlayed = handTypePlayed,
                CardsInPlayedHand = cardsForScoringCalc.ToList(),
                CardsHeldInHand = ZoneManager.HandZone.Cards.ToList(),
                CurrentTotalChips = TotalCurrentChips,
                RequiredChipsForBlind = RequiredChipsForCurrentBlind,
            };

            EngineEventHandler.TriggerEvent(handPlayDoneArgs);

            if(TotalCurrentChips >= RequiredChipsForCurrentBlind)
            {
                FlowHandler.ClosePlayRound();
            }else if(CurHandsRemaining == 0)
            {
                if (handPlayDoneArgs.PreventGameOverAndWinBlind)
                    FlowHandler.ClosePlayRound();
                else
                    FlowHandler.GameOver();

            }
            else
            {
                ZoneManager.DrawHandful();
            }
        }

        public static void DiscardSelectedFromHand(bool doRedraw = true)
        {
            if (CurDiscardsRemaining == 0)
                return;
            var selList = ZoneManager.CardsSelectedInHand.ToList();
            ZoneManager.DiscardSelectedFromHand();
            if (doRedraw)
                ZoneManager.DrawHandful();
            CurDiscardsRemaining -= 1;
            EngineEventHandler.TriggerEvent(new EngineDiscardDoneArgs()
            {
                BeingDiscarded = selList,
                MyContext = new EventContext() { Context = EventContextType.HandDiscardDone }
            });
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
            var args = new EngineCardSoldArgs()
            {
                CardBeingSold = beingSold,
                ValueBeingSoldFor = beingSold.SellCost,
                ZoneCardIsLeaving = zoneCardIsLeaving,
                MyContext = new EventContext() { Context = EventContextType.CardSell},
            };
            EngineEventHandler.TriggerEvent(args);

            EmitMoneyGain(args.ValueBeingSoldFor, args.CardBeingSold);

            //Right now, only jokers/consumables get returned to market pools.
            //Later maybe vouchers? I mean they shouldn't be sell-able but do have a pool.
            if(args.CardBeingSold.isJoker || args.CardBeingSold.isConsumable)
            {
                if(!MarketOptionsManager.ReturnMarketItemFromZone(args.CardBeingSold, args.ZoneCardIsLeaving))
                {
                    ZoneManager.DestroyCard(args.CardBeingSold, args.ZoneCardIsLeaving);
                }
            }
            else
            {
                //TODO: Others get destroyed? idk. Only weird cases where non-jokers/consumables getting sold, for modded stuff.
            }
        }

        public static void PerformPurchaseByType(Card beingPurchased)
        {
            var zoneFrom = beingPurchased.MyZone;
            CardZone zoneTo = null;
            //Order matters here; a card can in theory be multiple of these things.
            if (beingPurchased.isVoucher)
            {
                zoneTo = ZoneManager.ActiveVoucherZone;
            }else if (beingPurchased.isJoker)
            {
                zoneTo = ZoneManager.JokerZone;
            }else if (beingPurchased.isConsumable)
            {
                zoneTo = ZoneManager.ConsumableZone;
            }

            PerformPurchase(beingPurchased, zoneTo, zoneFrom);
        }

        public static void PerformPurchase(Card beingPurchased, CardZone zoneGoingTo = null, CardZone zoneFrom = null)
        {
            if (zoneFrom == null)
                zoneFrom = beingPurchased.MyZone;
            //noZone purchases can occur, such as a buyAndUse, which will handle its own discard.
            if (zoneGoingTo != null && zoneFrom != null)
                zoneGoingTo.DrawTargetFrom(zoneFrom, beingPurchased);
            EmitMoneyLoss(beingPurchased.BuyCost, beingPurchased, true);
            //TODO: Should this really be done here? Doesn't feel right.
            //Maybe add extra conditional here? In case a pack consumable can be bought without open.
            if (beingPurchased.isPack)
            {
                PackActions.OpenPack(beingPurchased);
                ZoneManager.DestroyCard(beingPurchased, beingPurchased.MyZone);
            }else if(beingPurchased.BuyCostOverride != null)
            {
                beingPurchased.BuyCostOverride = null;//Currently after purchase, any buy cost override is reset.
            }
        }

        public static void PerformBuyAndUse(Card beingPurchased)
        {
            var zoneFrom = beingPurchased.MyZone;

            //TODO: Once again, consumable usage params.
            if(beingPurchased.isConsumable && beingPurchased.ConsumableData.IsActivatable(null))
            {
                PerformPurchase(beingPurchased);
                ConsumableManager.UseConsumable(beingPurchased, zoneFrom);
            }
        }

        public static bool CanBuyAndUse(Card beingPurchased)
        {
            //TODO: Consumable usage params??
            return CanAfford(beingPurchased) && beingPurchased.isConsumable && beingPurchased.ConsumableData.IsActivatable(null);
        }

        public static void SetStartOfRoundStats()
        {
            CurHandsRemaining = MaxHandsPerRound;
            CurDiscardsRemaining = MaxDiscardsPerRound;
        }

        public static GameStateObj PopCurrGameState()
        {
            if (GameStateStack == null || GameStateStack.Count == 0)
                return null;
            //NOTE: For now, just this. Maybe some other stuff later idk.

            var args = new EngineGameStateChangeArgs()
            {
                MyContext = new() { Context = EventContextType.GameStatePop },
                OldStateToBePopped = GameStateStack.Peek(),
                NewStateRevealedByPop = GameStateStack.Count > 1 ? GameStateStack.ToArray()[1] : null,
            };
            EngineEventHandler.TriggerEvent(args);
            var ret = GameStateStack.Pop();

            args.isAfterStateChange = true;
            args.MyContext = new() { Context = EventContextType.PostGameStatePop };//Now in theory, could use same context obj and change context type. But don't really know eventual full scope of EventContext, maybe contextual info changes.
            EngineEventHandler.TriggerEvent(args);

            return ret;
        }

        public static void PushGameState(GameStateObj obj)
        {
            //Same as above; maybe more later.
            var args = new EngineGameStateChangeArgs()
            {
                MyContext = new() { Context = EventContextType.GameStatePush},
                NewStateBeingPushed = obj,
                OldStatePushedOver = GameStateStack != null && GameStateStack.Count > 0 ? CurrentGameStateObj : null,
            };
            EngineEventHandler.TriggerEvent(args);

            GameStateStack.Push(obj);

            args.isAfterStateChange = true;
            args.MyContext = new() { Context = EventContextType.PostGameStatePush };
            EngineEventHandler.TriggerEvent(args);
        }

        public static void ClearGameStateStack()
        {
            GameStateStack.Clear();
        }

        public static bool RollRandom(int numerator, int denominator, Card cardCalling)
        {
            var args = new EngineRandomRollArgs() { MyContext = new() { Context = EventContextType.RandomRollHappening }, CardThatIsRolling = cardCalling, Numerator = numerator, Denominator = denominator };
            EngineEventHandler.TriggerEvent(args);
            return args.OverrideResult ?? ChooseRandomInclusive(1, args.Denominator) <= args.Numerator;
        }

        public static int ChooseRandomInclusive(int min, int max)
        {
            return Random.Shared.Next(min, max + 1);
        }

        public static int randomNext(int val) => ChooseRandomInclusive(0, val - 1);

        public static int randomNext(int min, int max) => ChooseRandomInclusive(min, max - 1);
    }
}
