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
    //...why is this definition here?? Why?? Why not literally anywhere else??
    //Too big a pain in the ass to move it now... sadge.
    public enum BlindType
    {
        SMALL,
        BIG,
        BOSS,
    }

    public static class Globals
    {
        //CONSTANT SETTINGS
        //Should the two tags for a given ante be guaranteed to be unique or not?
        public const bool GUARANTEE_UNIQUE_TAGS = false;
        //Should the "default joker" be used if we try to generate a joker and the pool's empty?
        //If not, no joker will be generated, literally a null.
        public const bool USE_DEFAULT_JOKER_IF_POOL_EMPTY = true;
        //Ok, so: in real Balatro, the Illusion voucher (playing cards in shop can have enhancements/editions/seals) has a GLITCH:
        //though it says cards can generate with seals in market, they can't really.
        //So this is here to toggle our Illusion voucher behaves like it SHOULD based on description, or how it does in real Balatro.
        public const bool MIRROR_ILLUSION_SEAL_GLITCH = false;
        //Enables the key commands for A) debug command line, and B) debug-only rerolls like pack market reroll, voucher market reroll, etc.
        public const bool ALLOW_DEBUG_COMMANDS = true;


        private static int _reqChipsBlind = -1;

        //The pre-calculation values of current chips and mult.
        //By pre-calculation like literally before they're multiplied together to get the final chips.
        public static int CurrentChips = 0;
        public static double CurrentMult = 0;

        public static double DiscountMultiplier = 1.0;//For price discounting effects

        //The "number to beat" for the current blind.
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


        //Game state tracking fields; check GameState enum vals for potential states.
        public static Stack<GameStateObj> GameStateStack = new();
        public static GameStateObj CurrentGameStateObj => GameStateStack.Peek();
        public static GameState CurrentGameState => CurrentGameStateObj.GameState;


        //Stops the whole game loop.
        public static bool QUIT = false;

        //Total chips built up in the current round;
        //Tracked until you either clear the requirement or you lose, then reset for next round.
        public static int TotalCurrentChips = 0;

        //Money-related fields.
        public static int Money = 4;
        public static int MinimumMoneyAllowed = 0;
        public static int BaseRerollCost = 5;
        public static int CurrentRerollCost = BaseRerollCost;

        //TODO: Yeah I hate this. A whole ass global field for one (1) Joker.
        //TBH I was getting tired of implementing jokers when I made this.
        //Completely change/fix this later.
        public static bool ChaosClownFreeRerollAvailable = false;

        //Used to track the "card source" of reroll costs. Actually useful, not just a hack.
        public static Card RerollButtonCard;

        //Max capacity of hand zone.
        public static int HandSize { get => ZoneManager.HandSize; set => ZoneManager.HandSize = value; }

        /// <summary>
        /// The maximum number of cards the player can select at once.
        /// </summary>
        public static int SelectionMax = 5;

        /// <summary>
        /// Number of cards currently selected in hand.
        /// </summary>
        public static int CurNumCardsSelected => ZoneManager.CardsSelectedInHand.Count();

        /// <summary>
        /// The BASE hand max capacity, what it's set to at initialization.
        /// </summary>
        public static int BaseHandSize = 8;

        /// <summary>
        /// The base size of the primary market zone.
        /// </summary>
        public const int BaseMainMarketCount = 2;

        /// <summary>
        /// The base size of the markets pack zone.
        /// </summary>
        public const int BasePackMarketCount = 2;

        /// <summary>
        /// The base size of the markets voucher zone.
        /// </summary>
        public const int BaseVoucherMarketCount = 1;

        /// <summary>
        /// How many hands can be played per blind this run.
        /// </summary>
        public static int MaxHandsPerRound = 4;

        /// <summary>
        /// How many discards can be used per blind this run.
        /// </summary>
        public static int MaxDiscardsPerRound = 3;


        /// <summary>
        /// The maximum amount earnable through interest per played blind.
        /// </summary>
        public static int CurMaxInterest = 5;

        /// <summary>
        /// The cost of boss blind rerolls.
        /// This does NOT affect how many boss rerolls are allowed.
        /// </summary>
        public static int CurrentBossBlindRerollCost = 10;

        /// <summary>
        /// The number of Boss blind rerolls allowed per-ante in the current run. (-1 = infinite rolls)
        /// </summary>
        public static int BaseBossBlindRerollsAllowed = 0;

        /// <summary>
        /// How many boss blind rerolls are remaining for the current ante. (-1 = infinite rolls)
        /// </summary>
        public static int CurBossBlindRerollsAllowed = 0;

        /// <summary>
        /// Flags triggered so far this run.
        /// </summary>
        public static HashSet<string> Flags = new();

        /// <summary>
        /// Can the player currently reroll the boss blind?
        /// </summary>
        public static bool CanRerollBossBlind => CurBossBlindRerollsAllowed != 0 && CanAfford(CurrentBossBlindRerollCost);


        public static bool ShopPlayingCardsGetModifiers = false;//NOTE: TEMP. 

        private static int _curDisc;
        private static int _curHands;

        /// <summary>
        /// Number of hands to play remaining in the current round.
        /// </summary>
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

        /// <summary>
        /// Number of discards remaining in the current round.
        /// </summary>
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

        /// <summary>
        /// Can the player discard from hand right now?
        /// </summary>
        public static bool CanDiscard => CurDiscardsRemaining > 0;

        /// <summary>
        /// This is the big one. Initialize the whole Balatro Engine.
        /// MUST run this once before doing anything else with the engine.
        /// </summary>
        public static void InitializeMain()
        {
            UnlockManager.LoadProgress();

            ZoneManager.InitializeMainGameZones();

            ScoreHandler.InitializeHandStatTracker();

            GlobalEventListeners.SetupGlobalListeners();

            //IMPORTANT TO DO THIS BEFORE INITIALIZING MARKET POOLS
            VoucherDb.ResetDependants();

            MarketOptionsManager.InitializeMarketPools();

            FlowHandler.InitializeFlowListeners();

            PackActions.InitializePackData();

            PoolManager.InitializePoolManager();

            RerollButtonCard = new();
        }

        /// <summary>
        /// Reset the big global values to their defaults.
        /// </summary>
        public static void ResetGlobalValues()
        {
            TotalCurrentChips = 0;
            Money = 4;//starting money is base 4
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
            Flags.Clear();
        }

        /// <summary>
        /// Play the currently selected cards, score them, and update gamestate appropriately depending on the results.
        /// Official Blind hand play.
        /// </summary>
        public static void PlayCurrentlySelectedHand()
        {
            if (CurrentGameState != GameState.PlayRound || CurNumCardsSelected == 0)
            {
                return;
            }

            var selCards = ZoneManager.CardsSelectedInHand.ToList();

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
            var calcedArgs = new EngineHandPlayArgs()
            {
                MyContext = new EventContext() { Context = EventContextType.AllScoringCardsDecided },
                CardsInScoringHand = cardsForScoringCalc,
                HandBeingPlayed = handTypePlayed,
                CardsSelected = selCards,
            };
            EngineEventHandler.TriggerEvent(calcedArgs);
            //Yeah, we trigger a lot of different events during hand scoring. Yeah, some of them are probably redundant and could be merged into others.
            //But you know what? Huh? You know what? huh?
            //huh?
            
            if (!calcedArgs.CancelScoring)
            {
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
                    //So: right now technically there are things that should score but aren't; take other hidden "jokers" like boss blinds, challenges, decks etc. 
                    //This works right now cause none of them have effects that require them to trigger; that's for things like jokers that give +mult, +chips, etc.
                    //HOWEVER: I find the idea of getting like a HOLO deck somehow kinda funny. So I think these should trigger eventually.
                    //LMAO I WROTE THIS LIKE EXACT COMMENT BELOW THE CURLY BRACKET AND DIDNT NOTICE. WTF.
                }

                //TODO: Other hidden effects should score correct? Otherwise only listeners from challenges/decks/boss blinds/other hiddens will trigger, not scoring effects.
                //BUT maybe we don't want them to trigger scoring... idk man.
                //Onadds/onremoves will also trigger in either case since hidden zone is a jokerzone.

                ScoreHandler.FinalPlayChipsCalc();
                EngineEventHandler.TriggerEvent(new EngineEventArgs()
                {
                    MyContext = new EventContext() { Context = EventContextType.HandPlayScoringDone },
                });
            }
            else
            {
                //TODO: SCORING CANCELLED EVENT.
                CurrentChips = 0;
                CurrentMult = 0;

            }

            ZoneManager.ClearOutPlayZone();
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
            }
            else if(CurHandsRemaining == 0)
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

        /// <summary>
        /// Discard the currently selected cards from hand. Official gameplay discard.
        /// </summary>
        /// <param name="doRedraw">Should the hand be refilled after the discard?</param>
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

        /// <summary>
        /// Gain an amount of chips (pre-calculation) and emit for all listeners to hear.
        /// </summary>
        /// <param name="chipsNum">The number of chips to be gained.</param>
        /// <param name="src">The card that caused this chip gain.</param>
        public static void EmitChipsAdd(int chipsNum, Card src)
        {
            var emitArgs = new EngineChipsMultGainEmitArgs() { ChipsGainEmitted = chipsNum, MyContext = new EventContext() { Context = EventContextType.GainEmit }, SourceOfEmit = src };
            EngineEventHandler.TriggerEvent(emitArgs);

            CurrentChips += emitArgs.ChipsGainEmitted;
        }

        /// <summary>
        /// Gain an amount of mult (pre-calculation) and emit for all listeners to hear.
        /// </summary>
        /// <param name="multGain">Amount of mult to be gained.</param>
        /// <param name="src">The card that caused this mult gain.</param>
        public static void EmitMultAdd(double multGain, Card src)
        {
            var emitArgs = new EngineChipsMultGainEmitArgs() { MultGainEmitted = multGain, MyContext = new EventContext() { Context = EventContextType.GainEmit }, SourceOfEmit = src };
            EngineEventHandler.TriggerEvent(emitArgs);

            CurrentMult += emitArgs.MultGainEmitted;
        }

        /// <summary>
        /// Apply a multiplier to the current mult (pre-calculation) and emit for all listeners to hear.
        /// </summary>
        /// <param name="multMultGain">The multiplier for current mult.</param>
        /// <param name="src">The card that caused this mult gain.</param>
        public static void EmitMultMult(double multMultGain, Card src)
        {
            var emitArgs = new EngineChipsMultGainEmitArgs() { MultMultEmitted = multMultGain, MyContext = new EventContext() { Context = EventContextType.GainEmit }, SourceOfEmit = src };
            EngineEventHandler.TriggerEvent(emitArgs);

            CurrentMult *= emitArgs.MultMultEmitted;
        }

        /// <summary>
        /// Gain an amount of money and emit for all listeners to hear.
        /// </summary>
        /// <param name="moneyAmt">Amount of money to gain.</param>
        /// <param name="src">The card that caused this money gain.</param>
        public static void EmitMoneyGain(int moneyAmt, Card src)
        {
            var emitArgs = new EngineGoldGainEmitArgs() { AmountGained = moneyAmt, MyContext = new EventContext() { Context = EventContextType.MoneyGainEmit }, SourceOfEmit = src };
            EngineEventHandler.TriggerEvent(emitArgs);

            Money += emitArgs.AmountGained;
        }

        //Differentiated from above for some edge cases.
        /// <summary>
        /// Lose an amount of money and emit for all listeners to hear.
        /// </summary>
        /// <param name="moneyAmt">Amount of money to lose.</param>
        /// <param name="src">The card that caused this money loss.</param>
        /// <param name="isPurchase">Was this money loss caused by a purchase?</param>
        public static void EmitMoneyLoss(int moneyAmt, Card src, bool isPurchase)
        {
            //TODO: Might differentiate event as well later idk.
            var emitArgs = new EngineGoldGainEmitArgs() { AmountGained = -1 * moneyAmt, MyContext = new EventContext() { Context = EventContextType.MoneyGainEmit }, SourceOfEmit = src };
            EngineEventHandler.TriggerEvent(emitArgs);
            Money += emitArgs.AmountGained;
        }

        /// <summary>
        /// Add a flag to the current run.
        /// </summary>
        /// <param name="flag">The flag to add.</param>
        public static void AddFlag(string flag)
        {
            if (!Flags.Contains(flag))
                Flags.Add(flag);
        }

        /// <summary>
        /// Returns a value indicating whether the player can afford the passed cost.
        /// </summary>
        /// <param name="costToCheck">The price being checked for affordability.</param>
        /// <returns>A boolean indicating whether the passed cost is affordable.</returns>
        public static bool CanAfford(int costToCheck)
        {
            return Money - costToCheck >= MinimumMoneyAllowed;
        }

        /// <summary>
        /// Returns a value indicating whether the player can afford the passed card.
        /// </summary>
        /// <param name="c">The card whose affordability is being checked.</param>
        /// <returns>A boolean indicating whether the passed card is currently affordable.</returns>
        public static bool CanAfford(Card c)
        {
            return CanAfford(c.BuyCost);
        }

        /// <summary>
        /// Returns a value indicating whether the passed card can currently be purchased.
        /// </summary>
        /// <param name="c">The card being checked for purchasibility.</param>
        /// <returns>A boolean indicating whether the passed card could currently be purchased.</returns>
        public static bool CanBePurchased(Card c)
        {
            if (c.isJoker)
            {
                return CanAfford(c) && ZoneManager.JokerZone.HasRoomFor(c);
            }
            if (c.isConsumable)
            {
                return CanAfford(c) && ZoneManager.ConsumableZone.HasRoomFor(c);
            }
            return CanAfford(c);
        }

        /// <summary>
        /// Sell the passed card.
        /// </summary>
        /// <param name="beingSold">The card to be sold.</param>
        /// <param name="zoneCardIsLeaving">The zone the sold card is from.</param>
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

        /// <summary>
        /// Purchase the passed card AFTER determining which zone that card should be moved to.
        /// </summary>
        /// <param name="beingPurchased">The card to be purchased.</param>
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

        /// <summary>
        /// Purchase the specified card.
        /// </summary>
        /// <param name="beingPurchased">Card to be purchased.</param>
        /// <param name="zoneGoingTo">The zone the card will go to after the purchase.</param>
        /// <param name="zoneFrom">The zone the card is coming from before the purchase.</param>
        public static void PerformPurchase(Card beingPurchased, CardZone zoneGoingTo = null, CardZone zoneFrom = null)
        {
            if (zoneFrom == null)
                zoneFrom = beingPurchased.MyZone;
            //no-Zone purchases can occur, such as a buyAndUse, which will handle its own discard.
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

        /// <summary>
        /// Perform a "buy and use" of the passed consumable card; that is, buy it and use it immediately, without sending it to any particular zone.
        /// </summary>
        /// <param name="beingPurchased">The card to be bought and used.</param>
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

        /// <summary>
        /// Returns a value indicating whether the passed consumable can be bought and used immediately.
        /// </summary>
        /// <param name="beingPurchased">The card being checked.</param>
        /// <returns>A boolean indicating whether the passed card can be bought and used immediately.</returns>
        public static bool CanBuyAndUse(Card beingPurchased)
        {
            //TODO: Consumable usage params??
            return CanAfford(beingPurchased) && beingPurchased.isConsumable && beingPurchased.ConsumableData.IsActivatable(null);
        }

        /// <summary>
        /// Reset basic per-round stats to their start-of-round state.
        /// </summary>
        public static void SetStartOfRoundStats()
        {
            CurHandsRemaining = MaxHandsPerRound;
            CurDiscardsRemaining = MaxDiscardsPerRound;
        }

        /// <summary>
        /// Pop the current game state object off the state stack.
        /// </summary>
        /// <returns>The now-removed current game state object.</returns>
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

        /// <summary>
        /// Push a new game state object to the state stack.
        /// </summary>
        /// <param name="obj">The new game state object to be pushed to the game state stack.</param>
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

        /// <summary>
        /// Clear out the entire game state stack.
        /// </summary>
        public static void ClearGameStateStack()
        {
            GameStateStack.Clear();
        }

        /// <summary>
        /// Perform an in-game random roll, alerting any listeners for potential modifications to the roll.
        /// </summary>
        /// <param name="numerator">The numerator of the chance for the roll: "numerator" in "denominator" chance.</param>
        /// <param name="denominator">The denominator of the chance for the roll: "numerator" in "denominator" chance.</param>
        /// <param name="cardCalling">The card requesting this random roll occur.</param>
        /// <returns>The results of the random roll.</returns>
        public static bool RollRandom(int numerator, int denominator, Card cardCalling)
        {
            var args = new EngineRandomRollArgs() { MyContext = new() { Context = EventContextType.RandomRollHappening }, CardThatIsRolling = cardCalling, Numerator = numerator, Denominator = denominator };
            EngineEventHandler.TriggerEvent(args);
            return args.OverrideResult ?? ChooseRandomInclusive(1, args.Denominator) <= args.Numerator;
        }

        /// <summary>
        /// Choose a random integer in the passed range; does NOT trigger any events or alert any listeners, purely returns the value.
        /// </summary>
        /// <param name="min">Minimum possible roll; min <= result <= max</param>
        /// <param name="max">Maximum possible roll; min <= result <= max</param>
        /// <returns>The rolled integer.</returns>
        public static int ChooseRandomInclusive(int min, int max)
        {
            return Random.Shared.Next(min, max + 1);
        }

        /// <summary>
        /// Behaves like "Random.Next"; return a random value from 0 to val - 1.
        /// </summary>
        /// <param name="val">Maximum possible roll, non-inclusive; 0 <= result < val</param>
        /// <returns>A random integer less than the passed value.</returns>
        public static int randomNext(int val) => ChooseRandomInclusive(0, val - 1);

        /// <summary>
        /// Behaves like "Random.Next" when given two values; returns a random value from min to max - 1.
        /// </summary>
        /// <param name="min">Minimum possible roll, inclusive; min <= result < max</param>
        /// <param name="max">Maximum possible roll, non-inclusive; min <= result < max</param>
        /// <returns>A random integer greater than or equal to the min and less than the max.</returns>
        public static int randomNext(int min, int max) => ChooseRandomInclusive(min, max - 1);
    }
}
