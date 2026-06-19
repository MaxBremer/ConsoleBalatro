using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Cards.Tags;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using ConsoleBalatro.Engine.Stakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public static class FlowHandler
    {
        /// <summary>
        /// In the basic White stake, this is the base ante chip amount.
        /// Small blind=1x amount, Big blind = 1.5x, Boss = 2x (unless in exception list)
        /// Index = ante, except lower than 0 which is just 0.
        /// </summary>
        /// TODO: Implement endless/up to ante 16.
        /// TODO: need bigger datatype than int. That's a separate, BIG can of worms.
        public static List<int> BaseAnteChipAmounts = new()
        {
            100,
            300,
            800,
            2000,
            5000,
            11000,
            20000,
            35000,
            50000,
        };

        /// <summary>
        /// Base chip amount per-ante for Green stake.
        /// </summary>
        public static List<int> GreenStakeAnteChipAmounts = new()
        {
            100,
            300,
            900,
            2600,
            8000,
            20000,
            36000,
            60000,
            100000,
        };

        /// <summary>
        /// Base chip amount per-ante for Purple stake and above.
        /// </summary>
        public static List<int> PurpleStakeAnteChipAmounts = new()
        {
            100,
            300,
            1000,
            3200,
            9000,
            25000,
            60000,
            110000,
            200000,
        };

        /// <summary>
        /// The amount of money each blind gives for free once beaten.
        /// </summary>
        public static Dictionary<BlindType, int> PostRoundFreeMoney = new()
        {
            {BlindType.SMALL, 3 },
            {BlindType.BIG, 4 },
            {BlindType.BOSS, 5 },
        };

        /// <summary>
        /// Boss blinds with exceptions to their chip amount multiplier.
        /// </summary>
        public static Dictionary<string, double> BlindSpecificChipMults = new()
        {
            ["THE WALL"] = 4,
            ["THE NEEDLE"] = 1,
            ["VIOLET VESSEL"] = 6,
        };

        /// <summary>
        /// The current ante of the run.
        /// </summary>
        public static int CurrentAnte = 0;

        /// <summary>
        /// The currently equipped deck.
        /// </summary>
        public static string CurrentDeckDbName = string.Empty;

        /// <summary>
        /// Getter for the base chip amount of the current ante.
        /// </summary>
        public static int CurrentBaseChipAmount => GetCurrentChipScalingList()[CurrentAnte];

        /// <summary>
        /// The currently selected blind within the ante (small, big, or boss)
        /// </summary>
        public static BlindType CurrentSelectedBlind = BlindType.SMALL;

        /// <summary>
        /// The tag received for skipping the current small blind.
        /// </summary>
        public static TagType CurSmallBlindTag;

        /// <summary>
        /// The tag received for skipping the current big blind.
        /// </summary>
        public static TagType CurBigBlindTag;

        /// <summary>
        /// Get the tag received for skipping the passed blind.
        /// </summary>
        /// <param name="b">The blind whose tag should be retrieved.</param>
        /// <returns>The tag of the passed blind for the current ante.</returns>
        public static TagType GetTagTypeOf(BlindType b) => b == BlindType.SMALL ? CurSmallBlindTag : (b == BlindType.BIG ? CurBigBlindTag : TagType.NONE);

        /// <summary>
        /// Returns the tag for the currently selected blind.
        /// </summary>
        public static TagType CurrentTag => GetTagTypeOf(CurrentSelectedBlind);

        /// <summary>
        /// The Boss blind DB name of the boss of the current ante.
        /// </summary>
        public static string CurrentBossBlind;

        /// <summary>
        /// Returns a value indicating whether the player can skip the currently selected blind.
        /// </summary>
        public static bool SkipAvailable => CurrentSelectedBlind != BlindType.BOSS;

        /// <summary>
        /// A horrible no-good very bad way to basically just implement juggle tag. Stores temporary modifiers to be used only at the next blind, then discarded.
        /// TODO: fix this. do it better. dummy.
        /// </summary>
        public static EnginePlayRoundSetupArgs CurrentTempChanges = null;

        /// <summary>
        /// Initialize any global event listeners for gameplay flow.
        /// </summary>
        public static void InitializeFlowListeners()
        {
            //XTO-DO: Might not be any flow listeners, idk. This is here if I need it.
            //There was one! I'm a smartie :)
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = RestoreSavedOrderings, MyContextType = EventContextType.GameStatePop });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = SavePlayRoundOrderings, MyContextType= EventContextType.GameStatePush });
        }

        private static List<int> GetCurrentChipScalingList()
        {
            //Yeah, it's the dumb way to do it, but doing an event is also pretty dumb idk.
            //Maybe make it an event/listener later? If so just have to make sure Purple triggers AFTER green (which would be a good reason to implement event priority >:( )
            if (StakeManager.StakeActive(StakeType.PURPLE))
            {
                return PurpleStakeAnteChipAmounts;
            }else if (StakeManager.StakeActive(StakeType.GREEN))
            {
                return GreenStakeAnteChipAmounts;
            }
            else
            {
                return BaseAnteChipAmounts;
            }
        }

        //TODO: I think we'll end up getting rid of this whole order saving/loading. Unless I want mid-round packs, we're not doing run saving, so what's the point. 
        private static void SavePlayRoundOrderings(EngineEventArgs args)
        {
            if(args != null && args is EngineGameStateChangeArgs gsArgs && gsArgs.isPush && gsArgs.OldStatePushedOver != null && gsArgs.OldStatePushedOver.GameState == GameState.PlayRound)
            {
                var targetState = gsArgs.OldStatePushedOver;
                targetState.SavedHandOrder = DataManager.PossiblyGetOrderFor(ZoneManager.HandZone);
                targetState.SavedDeckOrder = DataManager.PossiblyGetOrderFor(ZoneManager.DeckZone);
                targetState.SavedDiscardOrder = DataManager.PossiblyGetOrderFor(ZoneManager.DiscardZone);
                targetState.SavedHiddenPlayOrder = DataManager.PossiblyGetOrderFor(ZoneManager.HiddenPlayZone);
            }
        }

        //TODO: Probably delete, see above todo.
        private static void RestoreSavedOrderings(EngineEventArgs args)
        {
            if(args != null && args is EngineGameStateChangeArgs gsArgs && gsArgs.isPop && gsArgs.NewStateRevealedByPop != null && gsArgs.NewStateRevealedByPop.GameState == GameState.PlayRound)
            {
                var targetState = gsArgs.NewStateRevealedByPop;
                if (!string.IsNullOrEmpty(targetState.SavedHandOrder))
                    RedrawOrdered(ZoneManager.HandZone, ZoneManager.DeckZone, targetState.SavedHandOrder);
                if (!string.IsNullOrEmpty(targetState.SavedDiscardOrder))
                    RedrawOrdered(ZoneManager.DiscardZone, ZoneManager.DeckZone, targetState.SavedDiscardOrder);
                if (!string.IsNullOrEmpty(targetState.SavedHiddenPlayOrder))
                    RedrawOrdered(ZoneManager.HiddenPlayZone, ZoneManager.DeckZone, targetState.SavedHiddenPlayOrder);
                if (!string.IsNullOrEmpty(targetState.SavedDeckOrder))
                    DataManager.ReorderCards(ZoneManager.DeckZone.Cards, targetState.SavedDeckOrder);
            }
        }

        //TODO: Probably delete, see above todo.
        private static void RedrawOrdered(CardZone toDrawTo, CardZone drawFrom, string order)
        {
            var trueOrder = DataManager.OrderListFromString(order);
            foreach (var id in trueOrder)
            {
                var targetCard = DataManager.CardsByID[id];
                if(drawFrom.Cards.Contains(targetCard))
                    toDrawTo.DrawTargetFrom(drawFrom, targetCard);
            }
            DataManager.ReorderCards(toDrawTo.Cards, order);
        }

        /// <summary>
        /// Gets the number of chips required to beat the passed blind for the current ante.
        /// </summary>
        /// <param name="blind">Blind whose chip requirement we want</param>
        /// <returns>The amount of chips required to beat the passed blind.</returns>
        public static int GetChipsForBlindType(BlindType blind)
        {
            var chipsMult = 1.0;
            switch (blind)
            {
                case BlindType.BIG:
                    chipsMult = 1.5;
                    break;
                case BlindType.BOSS:
                    if(CurrentBossBlind != null && BlindSpecificChipMults.ContainsKey(CurrentBossBlind))
                        chipsMult = BlindSpecificChipMults[CurrentBossBlind];
                    else
                        chipsMult = 2.0;
                    break;
                default:
                    break;
            }
            
            var retAmt = (int)(CurrentBaseChipAmount * chipsMult);
            var args = new EngineGetBlindReqArgs() { MyContext = new EventContext() { Context = EventContextType.GetBlindChips }, ChipRequirementAmount = retAmt };
            EngineEventHandler.TriggerEvent(args);

            return args.ChipRequirementAmount;
        }

        /// <summary>
        /// Initialize a "Play Round", that is, an actual blind, in which the player must beat a score with played hands, you know the drill.
        /// </summary>
        /// <param name="blindType">The blind type within the current ante to initialize.</param>
        public static void InitializePlayRound(BlindType blindType)
        {
            if(blindType == BlindType.BOSS)
            {
                //SET UP BOSS BLIND BEFORE ANYTHING
                ZoneManager.HiddenBlindAttributeZone.AddCard(BossBlindDb.GenerateBlindCard(CurrentBossBlind), invisibleAdd: false); //NO INVISIBLE ADD. AGAIN. WHAT IS IT EVEN GOOD FOR?
            }
            var setupArgs = new EnginePlayRoundSetupArgs() { MyContext = new() { Context = EventContextType.StartPlayRound } };
            EngineEventHandler.TriggerEvent(setupArgs);
            ProcessTempRoundBuffs(setupArgs);
            Globals.PushGameState(new GameStateObj() { GameState = GameState.PlayRound });
            Globals.SetStartOfRoundStats();
            ZoneManager.ShuffleDeck();
            //Scores should already be reset, as they're reset post-round.
            //but u know. can never be too sure.

            Globals.RequiredChipsForCurrentBlind = GetChipsForBlindType(blindType);
            ZoneManager.DrawHandful();

            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.StartPlayRoundSetupOver } });
        }

        private static void ProcessTempRoundBuffs(EnginePlayRoundSetupArgs args)
        {
            if (!args.AnyBuffsApplied())
                return;

            if(args.TempHandSizeBonus != 0)
            {
                ZoneManager.HandSize += args.TempHandSizeBonus;
            }

            CurrentTempChanges = args;
        }

        /// <summary>
        /// Closes an open "Play Round", that is, an actual blind, moving immediately to the Post Round.
        /// </summary>
        public static void ClosePlayRound()
        {
            if (Globals.CurrentGameState != GameState.PlayRound)
                return;

            ZoneManager.ClosePlayRound();
            ScoreHandler.ResetScoresPostRound();
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.EndPlayRound } });
            UndoTempRoundBuffs();

            Globals.PopCurrGameState();

            //BEFORE WE INCREMENT BLIND:
            //POSSIBLY CLEAR BLIND CARDS FROM HIDDEN BLIND ZONE.
            ZoneManager.HiddenBlindAttributeZone.ClearCards();

            //BEFORE BLIND INCREMENT, CALC MONEY TO GIVE.
            var postRoundMoney = CalcPostRoundMoneyWithSources();

            IncrementBlind();

            //Post-round with the money menu

            InitializePostRound(postRoundMoney);
        }

        private static void UndoTempRoundBuffs()
        {
            if (CurrentTempChanges == null)
                return;

            if(CurrentTempChanges.TempHandSizeBonus != 0)
                ZoneManager.HandSize -= CurrentTempChanges.TempHandSizeBonus;

            CurrentTempChanges = null;
        }

        /// <summary>
        /// Initializes the "Post Round", a brief round in which the player is presented with their reward money and its sources.
        /// </summary>
        /// <param name="postRoundMoney">A list of tuples, each representing an amount of money and its source.</param>
        public static void InitializePostRound(List<(string, int)> postRoundMoney)
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.StartPostRound } });
            var gsObj = new GameStateObj() { GameState = GameState.PostRoundRewardsMenu };
            gsObj.PostRoundMoneySources.AddRange(postRoundMoney);
            gsObj.PostRoundMoneyToGive = gsObj.PostRoundMoneySources.Select(x => x.Item2).Sum();
            Globals.PushGameState(gsObj); //TODO: context??
        }

        /// <summary>
        /// Closes the "Post Round", moving immediately to the Market Round.
        /// </summary>
        public static void ClosePostRound()
        {
            Globals.EmitMoneyGain(Globals.CurrentGameStateObj.PostRoundMoneyToGive, null);
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.EndPostRound } });
            Globals.PopCurrGameState();
            InitializeMarketRound();
        }

        /// <summary>
        /// Initializes the "Market Round", in which the player can buy from the main market, pack market, and/or voucher market.
        /// </summary>
        public static void InitializeMarketRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.StartMarket } });
            Globals.PushGameState(new GameStateObj() { GameState = GameState.ShopMenu });
            
            MarketGeneralManager.FillFreshMarket();
        }

        /// <summary>
        /// Closes the Market Round, moving back to blind selection.
        /// </summary>
        public static void CloseMarketRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.EndMarket } });
            MarketGeneralManager.MarketClosing();
            Globals.PopCurrGameState();

            InitializeBlindSelectionRound();
        }

        /// <summary>
        /// Initializes the Blind Selection Round, in which the player plays blinds and/or chooses which ones to skip.
        /// </summary>
        public static void InitializeBlindSelectionRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.StartBlindSelection } });
            Globals.PushGameState(new GameStateObj() { GameState = GameState.BlindsMenu }); //TODO: context??
        }

        /// <summary>
        /// Closes the Blind Selection Round.
        /// </summary>
        public static void CloseBlindSelectionRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.EndBlindSelection } });
            Globals.PopCurrGameState();
        }

        /// <summary>
        /// Initializes the Deck Selection Round, a menu in which the player chooses which deck to use this run.
        /// </summary>
        public static void InitializeDeckSelectionRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.StartDeckSelection } });
            Globals.PushGameState(new GameStateObj() { GameState = GameState.DeckSelectMenu }); //TODO: context??
        }

        /// <summary>
        /// Closes the Deck Selection Round.
        /// </summary>
        public static void CloseDeckSelectionRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.EndDeckSelection } });
            Globals.PopCurrGameState();
        }

        /// <summary>
        /// Opens the Pack Selection Round, in which the player has opened a pack and chooses which rewards to take.
        /// </summary>
        /// <param name="beingOpened">A Card representing the pack being opened.</param>
        public static void OpenPackSelectionRound(Card beingOpened)
        {
            var gsObj = new GameStateObj();
            gsObj.GameState = GameState.SelectingPackOption;
            gsObj.TargetPack = beingOpened;
            Globals.PushGameState(gsObj);
        }

        /// <summary>
        /// Closes the Pack Selection Round.
        /// </summary>
        public static void ClosePackSelectionRound()
        {
            ZoneManager.ClosePackSelection();
            Globals.PopCurrGameState();
        }

        /// <summary>
        /// Ends the game, currently only in the event of a loss.
        /// </summary>
        public static void GameOver()
        {
            Globals.ClearGameStateStack();
            Globals.PushGameState(new GameStateObj() { GameState = GameState.GameOverMenu });
            //TODO: EVENTUALLY, REMOVE THIS.
            Globals.QUIT = true;
        }

        /// <summary>
        /// Increment the currently selected blind. If currently selected is the Boss, start a new Ante and select the Small Blind.
        /// </summary>
        public static void IncrementBlind()
        {
            var oldBlind = CurrentSelectedBlind;
            switch (CurrentSelectedBlind)
            {
                case BlindType.SMALL:
                    CurrentSelectedBlind = BlindType.BIG;
                    break;
                case BlindType.BIG:
                    CurrentSelectedBlind = BlindType.BOSS;
                    break;
                case BlindType.BOSS:
                    MarkCurrentStakeBeatenIfRunWon();
                    //TODO: SHOW WIN SCREEN IF ANTE 8
                    StartNewAnte();
                    break;
            }
            var args = new EngineBlindChangeEventArgs();
            args.OldBlindType = oldBlind;
            args.NewBlindType = CurrentSelectedBlind;
            args.MyContext = new EventContext() { Context = EventContextType.BlindChange };
            EngineEventHandler.TriggerEvent(args);
        }

        /// <summary>
        /// If the run was won on this ante increase, mark the relevant deck and joker stickers and save them.
        /// </summary>
        private static void MarkCurrentStakeBeatenIfRunWon()
        {
            if (CurrentAnte == 8 && !string.IsNullOrWhiteSpace(CurrentDeckDbName))
            {
                var progressChanged = UnlockManager.MarkDeckStakeBeaten(CurrentDeckDbName, StakeManager.CurrentStake, saveImmediately: false);

                foreach (var jokerName in ZoneManager.JokerZone.Cards
                    .Where(card => card.IsJoker && !card.IsVoucher && !card.IsTag && !string.IsNullOrWhiteSpace(card.JokerData?.DBName))
                    .Select(card => card.JokerData!.DBName)
                    .Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    progressChanged |= UnlockManager.MarkJokerStakeBeaten(jokerName, StakeManager.CurrentStake, saveImmediately: false);
                }

                if (progressChanged)
                {
                    UnlockManager.SaveProgress();
                }
            }
        }

        /// <summary>
        /// Calculate all money earned by the player in the post round.
        /// </summary>
        /// <returns>A list of string/int pairs representing money earned and its source.</returns>
        public static List<(string, int)> CalcPostRoundMoneyWithSources()
        {
            var ret = new List<(string, int)>();

            if(PostRoundFreeMoney.ContainsKey(CurrentSelectedBlind) && PostRoundFreeMoney[CurrentSelectedBlind] > 0)
            {
                ret.Add(("Blind", PostRoundFreeMoney[CurrentSelectedBlind]));
            }

            if(Globals.Money >= 5)//TODO: Should probably be a listener somewhere.
            {
                var interestAmount = Math.Min(Globals.CurMaxInterest, Globals.Money / 5);
                ret.Add(("Interest", interestAmount));
            }

            if(Globals.CurHandsRemaining > 0)//TODO: Again. Listener.
            {
                ret.Add(("Hands Remaining", Globals.CurHandsRemaining));
            }

            var gatherArgs = new EngineGatherPostRoundMoneyArgs() { ExistingSources = ret };
            gatherArgs.MyContext = new EventContext() { Context = EventContextType.GatherPostRoundMoney };
            EngineEventHandler.TriggerEvent(gatherArgs);

            foreach (var jokerPair in gatherArgs.JokersContributed)
            {
                ret.Add((jokerPair.Item1.JokerName, jokerPair.Item2));
            }

            return ret;
        }

        /// <summary>
        /// Start a new ante. Refresh the voucher in market, reroll the boss blind, etc.
        /// </summary>
        public static void StartNewAnte()
        {
            //Steps:

            //Refresh/Reroll Voucher
            MarketGeneralManager.ResetVoucher();

            //Increment the current ante
            CurrentAnte += 1;

            //Roll a new boss blind
            //TODO: final boss blinds
            RerollBossBlind();

            //Set up new skip tags
            InitNewTags(Globals.GUARANTEE_UNIQUE_TAGS);

            //and set current blind to small.
            CurrentSelectedBlind = BlindType.SMALL;

            //Reset current remaining boss rerolls to the base
            Globals.CurBossBlindRerollsAllowed = Globals.BaseBossBlindRerollsAllowed;
        }

        /// <summary>
        /// Reroll the boss blind for the current ante.
        /// </summary>
        /// <param name="isPlayerReroll">A boolean indicating whether this was a player reroll, accessible via the retcon-like vouchers.</param>
        public static void RerollBossBlind(bool isPlayerReroll = false)
        {
            if(isPlayerReroll && (Globals.CurBossBlindRerollsAllowed == 0 || !Globals.CanAfford(Globals.CurrentBossBlindRerollCost)))
            {
                return;
            }

            //Select new Boss Blind
            string targetBossBlindName;
            //TODO: Account for the big boss blinds at the end.
            if (BossBlindDb.AvailableBossBlinds.Count == 0)
            {
                //If no boss options available, reset the pool.
                BossBlindDb.BossBlindsAlreadyUsed.Clear();
            }
            var oldBossBlind = CurrentBossBlind;
            var availableBosses = BossBlindDb.AvailableBossBlinds.ToList();
            targetBossBlindName = availableBosses[Globals.randomNext(availableBosses.Count)];
            BossBlindDb.BossBlindsAlreadyUsed.Add(targetBossBlindName);
            if (isPlayerReroll)
            {
                BossBlindDb.BossBlindsAlreadyUsed.Remove(oldBossBlind);//if player reroll, that boss can be reused.
                Globals.EmitMoneyLoss(Globals.CurrentBossBlindRerollCost, null, false);
                if (Globals.CurBossBlindRerollsAllowed > 0)
                    Globals.CurBossBlindRerollsAllowed--;
            }
            CurrentBossBlind = targetBossBlindName;
        }

        /// <summary>
        /// Set up new skip tags for the small and big blinds, usually only called when starting a new ante.
        /// </summary>
        /// <param name="makeUnique">A boolean indicating whether to guarantee the two tags be unique (as in, not the same as each other) or not.</param>
        public static void InitNewTags(bool makeUnique)
        {
            var values = Enum.GetValues(typeof(TagType)).Cast<TagType>().Where(x => x != TagType.NONE).ToArray();
            if (makeUnique)
            {
                var shuffled = values.OrderBy(x => Globals.randomNext(Int32.MaxValue)).ToArray();
                CurSmallBlindTag = shuffled[0];
                CurBigBlindTag = shuffled[1];
            }
            else
            {
                CurSmallBlindTag = values[Globals.randomNext(values.Length)];
                CurBigBlindTag = values[Globals.randomNext(values.Length)];
            }
        }

        /// <summary>
        /// If possible, skip the currently selectede blind, incrementing selection and adding a tag.
        /// </summary>
        public static void DoSkip()
        {
            if (!SkipAvailable)
            {
                return;
            }

            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new EventContext() { Context = EventContextType.BlindSkip } });
            TagDb.AddTagOfType(CurrentTag);

            IncrementBlind();
        }

        /// <summary>
        /// Start the currently selected blind, moving from blind selection round to play round.
        /// </summary>
        public static void StartSelectedBlind()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new EventContext() { Context = EventContextType.StartSelectedBlind } });
            CloseBlindSelectionRound();
            InitializePlayRound(CurrentSelectedBlind);
            //AttemptRoundInitialize(InitializePlayRound, CurrentSelectedBlind);
        }

        /// <summary>
        /// Chooses the deck and stake to use to start the run.
        /// </summary>
        /// <param name="deckDBName">The DB name of the deck selected for this run.</param>
        /// <param name="stakeChosen">The StakeType selected for this run.</param>
        public static void DeckChosen(string deckDBName, StakeType stakeChosen)
        {
            //Should only happen at the very start of a run.
            //So once a deck is chosen, initialize ante and move into blind selection.
            if (!DeckDb.IsDeckUnlocked(deckDBName) || !UnlockManager.IsStakeUnlockedForDeck(deckDBName, stakeChosen))
                return;

            CurrentDeckDbName = deckDBName;
            DeckDb.BecomeDeck(deckDBName);
            StakeManager.SetStake(stakeChosen);
            CloseDeckSelectionRound();
            StartNewAnte();
            InitializeBlindSelectionRound();
        }
    }
}
