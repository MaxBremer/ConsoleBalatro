using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using Xunit;

namespace ConsoleBalatro.Tests;

public class ScoringFlowTests
{
    public ScoringFlowTests()
    {
        ResetStaticState();
        SetupControlledGameState();
    }

    [Fact]
    public void ScorePlayedHand_BaselineHighCard_ProducesExpectedTotals()
    {
        var selected = BuildKnownHand("AS,2D,3C,4H,6S");

        var contributions = CaptureScoringContributions();
        Globals.PlayCurrentlySelectedHand();

        Assert.Equal(21, contributions.ChipsFromEmits);
        Assert.Equal(1d, contributions.MultAfterEmits);
        Assert.Equal(26, contributions.FinalTotalGain);
        Assert.Equal(1, contributions.TotalGainEvents);
    }

    [Fact]
    public void ScorePlayedHand_Modifiers_AddIncrementalContributions_AndTrackSources()
    {
        var selected = BuildKnownHand("AS,2D,3C,4H,6S");

        selected[0].Enhancement = Enhancement.BONUSCHIPS; // +30 chips once
        selected[1].Seal = Seal.RED; // retrigger once for +2 chips

        var joker = new Card();
        JokerDb.MakeCardJoker(joker, "JIMBO"); // +4 mult once
        ZoneManager.JokerZone.AddCard(joker);

        var contributions = CaptureScoringContributions();
        Globals.PlayCurrentlySelectedHand();

        Assert.Equal(53, contributions.ChipsFromEmits);
        Assert.Equal(5d, contributions.MultAfterEmits);
        Assert.Equal(265, contributions.FinalTotalGain);

        Assert.Contains(selected[0], contributions.ChipSources);
        Assert.Contains(selected[1], contributions.ChipSources);
        Assert.Contains(joker, contributions.MultSources);
    }

    [Fact]
    public void ScorePlayedHand_DebuffedCard_DoesNotContribute()
    {
        var selected = BuildKnownHand("AS,2D,3C,4H,6S");
        selected[0].Debuffed = true;

        var contributions = CaptureScoringContributions();
        Globals.PlayCurrentlySelectedHand();

        Assert.Equal(10, contributions.ChipsFromEmits); // 2+3+4+1
        Assert.DoesNotContain(selected[0], contributions.ChipSources);
        Assert.Equal(15, contributions.FinalTotalGain);
    }

    [Fact]
    public void ScorePlayedHand_EmptySelection_FailsGracefullyAndKeepsState()
    {
        var expectedBefore = Globals.TotalCurrentChips;

        Globals.PlayCurrentlySelectedHand();

        Assert.Equal(expectedBefore, Globals.TotalCurrentChips);
        Assert.Empty(ZoneManager.CurrentlyBeingPlayedZone.Cards);
        Assert.Equal(2, Globals.CurHandsRemaining);
    }

    private static List<Card> BuildKnownHand(string handDef)
    {
        ZoneManager.HandZone.Cards.Clear();
        var cards = CardFactory.CardListFromDefString(handDef, ",");
        ZoneManager.HandZone.AddCards(cards);

        foreach (var c in cards)
        {
            c.isSelected = true;
        }

        return cards;
    }

    private static ContributionCapture CaptureScoringContributions()
    {
        var capture = new ContributionCapture();

        EngineEventHandler.StartListening(new EngineEventListener
        {
            MyContextType = EventContextType.GainEmit,
            MyAction = args =>
            {
                var gain = Assert.IsType<EngineChipsMultGainEmitArgs>(args);
                if (gain.ChipsGainEmitted >= 0)
                {
                    capture.ChipsFromEmits += gain.ChipsGainEmitted;
                    capture.ChipSources.Add(gain.SourceOfEmit);
                }

                if (gain.MultGainEmitted >= 0)
                {
                    capture.MultAfterEmits += gain.MultGainEmitted;
                    capture.MultSources.Add(gain.SourceOfEmit);
                }
            }
        });

        EngineEventHandler.StartListening(new EngineEventListener
        {
            MyContextType = EventContextType.TotalChipsGained,
            MyAction = args =>
            {
                var total = Assert.IsType<EngineTotalChipsGainArgs>(args);
                capture.FinalTotalGain = total.AmountBeingGained;
                capture.TotalGainEvents += 1;
            }
        });

        return capture;
    }

    private sealed class ContributionCapture
    {
        public int ChipsFromEmits { get; set; }
        public double MultAfterEmits { get; set; } = 1;
        public int FinalTotalGain { get; set; }
        public int TotalGainEvents { get; set; }
        public List<Card> ChipSources { get; } = new();
        public List<Card> MultSources { get; } = new();
    }

    private static void ResetStaticState()
    {
        EngineEventHandler.GeneralListeners.Clear();
        EngineEventHandler.SpecificListeners.Clear();
        EngineEventHandler.ToBeAdded.Clear();
        EngineEventHandler.ToBeRemoved.Clear();
        EngineEventHandler.SavedEvents.Clear();
        EngineEventHandler.CallDepth = 0;

        ScoreHandler.CurrentHandStats.Clear();
        ScoreHandler.HandLevels.Clear();

        Globals.CurrentChips = 0;
        Globals.CurrentMult = 0;
        Globals.TotalCurrentChips = 0;
        Globals.RequiredChipsForCurrentBlind = int.MaxValue;
        Globals.ClearGameStateStack();
    }

    private static void SetupControlledGameState()
    {
        ZoneManager.HandZone = ZoneManager.MakeHand(8);
        ZoneManager.CurrentlyBeingPlayedZone = ZoneManager.MakeZone("CurrentlyBeingPlayed");
        ZoneManager.HiddenPlayZone = ZoneManager.MakeZone("Played");
        ZoneManager.DiscardZone = ZoneManager.MakeZone("Discard");
        ZoneManager.JokerZone = ZoneManager.MakeJokerZone();
        ZoneManager.ActiveVoucherZone = ZoneManager.MakeZone("ActiveVoucher");

        ScoreHandler.InitializeHandStatTracker();
        GlobalEventListeners.SetupGlobalListeners();

        Globals.PushGameState(new GameStateObj { GameState = GameState.PlayRound });
        Globals.CurHandsRemaining = 2;
        Globals.CurDiscardsRemaining = 1;
    }
}
