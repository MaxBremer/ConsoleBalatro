using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using Xunit;

namespace ConsoleBalatro.Tests;

public class FlowHandlerRoundStateTests : TestClassBase
{
    [Fact]
    public void MenuStart_ToBlindSelection_PushesBlindsMenuState()
    {
        ResetEngineForTest();
        Globals.PushGameState(new GameStateObj { GameState = GameState.MainMenu });

        EngineGameStateChangeArgs? observedPush = null;
        var listener = new EngineEventListener
        {
            MyContextType = EventContextType.GameStatePush,
            MyAction = args =>
            {
                if (args is EngineGameStateChangeArgs gsArgs && gsArgs.NewStateBeingPushed?.GameState == GameState.BlindsMenu)
                {
                    observedPush = gsArgs;
                }
            }
        };
        EngineEventHandler.StartListening(listener);

        FlowHandler.InitializeBlindSelectionRound();

        Assert.Equal(GameState.BlindsMenu, Globals.CurrentGameState);
        Assert.NotNull(observedPush);
        Assert.Equal(GameState.MainMenu, observedPush!.OldStatePushedOver!.GameState);
        Assert.Equal(GameState.BlindsMenu, observedPush.NewStateBeingPushed!.GameState);
    }

    [Fact]
    public void BlindSelection_ToPlayRound_InitializesRoundStateAndCounters()
    {
        ResetEngineForTest();
        Globals.PushGameState(new GameStateObj { GameState = GameState.BlindsMenu });
        FlowHandler.CurrentSelectedBlind = BlindType.BIG;

        FlowHandler.StartSelectedBlind();

        Assert.Equal(GameState.PlayRound, Globals.CurrentGameState);
        Assert.Equal(Globals.MaxHandsPerRound, Globals.CurHandsRemaining);
        Assert.Equal(Globals.MaxDiscardsPerRound, Globals.CurDiscardsRemaining);
        Assert.Equal(FlowHandler.GetChipsForBlindType(BlindType.BIG), Globals.RequiredChipsForCurrentBlind);
        Assert.Empty(ZoneManager.HiddenBlindAttributeZone.Cards);
    }

    [Fact]
    public void ActiveRound_ToRoundEndResolution_ClosesRoundAndPushesPostRoundRewards()
    {
        ResetEngineForTest();
        Globals.PushGameState(new GameStateObj { GameState = GameState.PlayRound });
        FlowHandler.CurrentSelectedBlind = BlindType.SMALL;
        Globals.Money = 10;
        Globals.CurHandsRemaining = 2;

        FlowHandler.ClosePlayRound();

        Assert.Equal(GameState.PostRoundRewardsMenu, Globals.CurrentGameState);
        Assert.Equal(BlindType.BIG, FlowHandler.CurrentSelectedBlind);
        Assert.Empty(ZoneManager.HiddenBlindAttributeZone.Cards);
        Assert.Contains(Globals.CurrentGameStateObj.PostRoundMoneySources, x => x.Item1 == "Blind" && x.Item2 == 3);
        Assert.Contains(Globals.CurrentGameStateObj.PostRoundMoneySources, x => x.Item1 == "Interest" && x.Item2 == 2);
        Assert.Contains(Globals.CurrentGameStateObj.PostRoundMoneySources, x => x.Item1 == "Hands Remaining" && x.Item2 == 2);
    }

    [Fact]
    public void ClosePlayRound_WhenNotInPlayRound_DoesNotJumpState()
    {
        ResetEngineForTest();
        Globals.PushGameState(new GameStateObj { GameState = GameState.BlindsMenu });
        FlowHandler.CurrentSelectedBlind = BlindType.SMALL;

        FlowHandler.ClosePlayRound();

        Assert.Equal(GameState.BlindsMenu, Globals.CurrentGameState);
        Assert.Equal(BlindType.SMALL, FlowHandler.CurrentSelectedBlind);
    }

    [Fact]
    public void CalcPostRoundMoney_RespectsMoneyAndHandsThresholdToggles()
    {
        ResetEngineForTest();
        FlowHandler.CurrentSelectedBlind = BlindType.SMALL;

        Globals.Money = 4;
        Globals.CurHandsRemaining = 0;
        var lowThreshold = FlowHandler.CalcPostRoundMoneyWithSources();
        Assert.DoesNotContain(lowThreshold, x => x.Item1 == "Interest");
        Assert.DoesNotContain(lowThreshold, x => x.Item1 == "Hands Remaining");

        Globals.Money = 5;
        Globals.CurHandsRemaining = 1;
        var thresholdMet = FlowHandler.CalcPostRoundMoneyWithSources();
        Assert.Contains(thresholdMet, x => x.Item1 == "Interest" && x.Item2 == 1);
        Assert.Contains(thresholdMet, x => x.Item1 == "Hands Remaining" && x.Item2 == 1);
    }

    [Fact]
    public void IncrementBlind_FromBoss_ProgressesAnteAndResetsToSmall()
    {
        ResetEngineForTest();
        FlowHandler.CurrentAnte = 0;
        FlowHandler.CurrentSelectedBlind = BlindType.BOSS;

        FlowHandler.IncrementBlind();

        Assert.Equal(1, FlowHandler.CurrentAnte);
        Assert.Equal(BlindType.SMALL, FlowHandler.CurrentSelectedBlind);
        Assert.False(string.IsNullOrWhiteSpace(FlowHandler.CurrentBossBlind));
    }
}
