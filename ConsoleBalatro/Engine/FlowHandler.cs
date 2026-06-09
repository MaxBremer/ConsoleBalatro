using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Cards.Tags;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public static class FlowHandler
    {
        //BASE chip amount (Small blind amt)
        //Big blind is 1.5x, Boss is 2x
        //Index is ante (0 is for 0 OR lower)
        //NOTE: Higher stakes will need a different arr.
        //NOTE: Endless mode, and chips/score at large, will require bigger datatype than int.
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

        public static Dictionary<BlindType, int> PostRoundFreeMoney = new()
        {
            {BlindType.SMALL, 3 },
            {BlindType.BIG, 4 },
            {BlindType.BOSS, 5 },
        };

        public static Dictionary<string, double> BlindSpecificChipMults = new()
        {
            ["THE WALL"] = 4,
            ["THE NEEDLE"] = 1,
            ["VIOLET VESSEL"] = 6,
        };

        public static int CurrentAnte = 0;
        public static int CurrentBaseChipAmount => BaseAnteChipAmounts[CurrentAnte];
        public static BlindType CurrentSelectedBlind = BlindType.SMALL;
        public static TagType CurSmallBlindTag;
        public static TagType CurBigBlindTag;
        public static TagType GetTagTypeOf(BlindType b) => b == BlindType.SMALL ? CurSmallBlindTag : (b == BlindType.BIG ? CurBigBlindTag : TagType.NONE);
        public static TagType CurrentTag => GetTagTypeOf(CurrentSelectedBlind);
        public static string CurrentBossBlind;
        public static bool SkipAvailable => CurrentSelectedBlind != BlindType.BOSS;
        public static bool ShouldDrawVoucher = true;

        public static EnginePlayRoundSetupArgs CurrentTempChanges = null;

        public static void InitializeFlowListeners()
        {
            //XTO-DO: Might not be any flow listeners, idk. This is here if I need it.
            //There was one! I'm a smartie :)
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = RestoreSavedOrderings, MyContextType = EventContextType.GameStatePop });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = SavePlayRoundOrderings, MyContextType= EventContextType.GameStatePush });
        }

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

        public static void InitializePostRound(List<(string, int)> postRoundMoney)
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.StartPostRound } });
            var gsObj = new GameStateObj() { GameState = GameState.PostRoundRewardsMenu };
            gsObj.PostRoundMoneySources.AddRange(postRoundMoney);
            gsObj.PostRoundMoneyToGive = gsObj.PostRoundMoneySources.Select(x => x.Item2).Sum();
            Globals.PushGameState(gsObj); //TODO: context??
        }

        public static void ClosePostRound()
        {
            Globals.EmitMoneyGain(Globals.CurrentGameStateObj.PostRoundMoneyToGive, null);
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.EndPostRound } });
            Globals.PopCurrGameState();
            InitializeMarketRound();
        }

        public static void InitializeMarketRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.StartMarket } });
            Globals.PushGameState(new GameStateObj() { GameState = GameState.ShopMenu });
            
            MarketGeneralManager.FillFreshMarket();
        }

        public static void CloseMarketRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.EndMarket } });
            MarketGeneralManager.MarketClosing();
            Globals.PopCurrGameState();

            InitializeBlindSelectionRound();
        }

        public static void InitializeBlindSelectionRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.StartBlindSelection } });
            Globals.PushGameState(new GameStateObj() { GameState = GameState.BlindsMenu }); //TODO: context??
        }

        public static void CloseBlindSelectionRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.EndBlindSelection } });
            Globals.PopCurrGameState();
        }

        public static void InitializeDeckSelectionRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.StartDeckSelection } });
            Globals.PushGameState(new GameStateObj() { GameState = GameState.DeckSelectMenu }); //TODO: context??
        }

        public static void CloseDeckSelectionRound()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.EndDeckSelection } });
            Globals.PopCurrGameState();
        }

        public static void OpenPackSelectionRound(Card beingOpened)
        {
            var gsObj = new GameStateObj();
            gsObj.GameState = GameState.SelectingPackOption;
            gsObj.TargetPack = beingOpened;
            Globals.PushGameState(gsObj);
        }

        public static void ClosePackSelectionRound()
        {
            ZoneManager.ClosePackSelection();
            Globals.PopCurrGameState();
        }

        public static void GameOver()
        {
            Globals.ClearGameStateStack();
            Globals.PushGameState(new GameStateObj() { GameState = GameState.GameOverMenu });
        }

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
                    StartNewAnte();
                    break;
            }
            var args = new EngineBlindChangeEventArgs();
            args.OldBlindType = oldBlind;
            args.NewBlindType = CurrentSelectedBlind;
            args.MyContext = new EventContext() { Context = EventContextType.BlindChange };
            EngineEventHandler.TriggerEvent(args);
        }

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
        }

        public static void RerollBossBlind(bool isPlayerReroll = false)
        {
            if(isPlayerReroll && (Globals.CurBossBlindRerollsAllowed == 0 || !Globals.CanAfford(10)))//TODO: HARD-SET PRICE OF BOSS REROLL SEEMS BAD.
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
                Globals.EmitMoneyLoss(10, null, false);
                if (Globals.CurBossBlindRerollsAllowed > 0)
                    Globals.CurBossBlindRerollsAllowed--;
            }
            CurrentBossBlind = targetBossBlindName;
        }

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

        public static void StartSelectedBlind()
        {
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new EventContext() { Context = EventContextType.StartSelectedBlind } });
            CloseBlindSelectionRound();
            InitializePlayRound(CurrentSelectedBlind);
            //AttemptRoundInitialize(InitializePlayRound, CurrentSelectedBlind);
        }

        public static void DeckChosen(string deckDBName)
        {
            //Should only happen at the very start of a run.
            //So once a deck is chosen, initialize ante and move into blind selection.
            if (!DeckDb.IsDeckUnlocked(deckDBName))
                return;

            DeckDb.BecomeDeck(deckDBName);
            CloseDeckSelectionRound();
            StartNewAnte();
            InitializeBlindSelectionRound();
        }
    }
}
