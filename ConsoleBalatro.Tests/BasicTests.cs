using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using Xunit;

namespace ConsoleBalatro.Tests;

public class BasicTests : TestClassBase
{
    [Fact]
    public void PlayingCardFromDefString_SetsRankSuitAndBaseChips()
    {
        var card = CardFactory.PlayingCardFromDefString("AS");

        Assert.Equal(Rank.ACE, card.Rank);
        Assert.Equal(Suit.SPADES, card.Suit);
        Assert.Equal(11, card.ChipsBase);
    }

    [Fact]
    public void StoneEnhancement_HidesRankAndSuit_AndSetsChipsTo50()
    {
        var card = CardFactory.PlayingCardFromDefString("9H");

        card.Enhancement = Enhancement.STONE;

        Assert.Equal(Rank.NONE, card.Rank);
        Assert.Equal(Suit.NONE, card.Suit);
        Assert.Equal(50, card.ChipsBase);
    }

    [Fact]
    public void ReorderCards_UsesProvidedOrderingString()
    {
        var first = CardFactory.PlayingCardFromDefString("2S");
        var second = CardFactory.PlayingCardFromDefString("3S");
        var third = CardFactory.PlayingCardFromDefString("4S");
        var cards = new List<Card> { first, second, third };

        var reversedOrder = $"{third.ID}|{second.ID}|{first.ID}";

        DataManager.ReorderCards(cards, reversedOrder);

        Assert.Equal(new[] { third.ID, second.ID, first.ID }, cards.Select(c => c.ID));
    }

    [Fact]
    public void PlayCards_SomeCardsDebuffed_CorrectlyIgnoresDebuffed()
    {
        ResetToFirstBlindPlayRound();
        BuildKnownHand("AS,AS,AS");
        ZoneManager.HandZone.Cards[2].ToggleSelect();
        ZoneManager.HandZone.Cards[1].Debuffed = true;
        ZoneManager.HandZone.Cards[2].Debuffed = true;
        var scorer = ZoneManager.HandZone.Cards[0];

        ZoneManager.HandZone.Cards[2].SetEnhancementOfficial(Enhancement.STEEL);

        var record = CaptureScoringContributions();
        Globals.PlayCurrentlySelectedHand();
        Assert.Equal(11, record.ChipsFromEmits);
        var c = Assert.Single(record.ChipSources);
        Assert.Equal(scorer, c);
        Assert.Equal(1, record.MultMultFromEmits);
        Assert.Empty(record.MultMultSources);
    }

    [Fact]
    public void ChipCounts_CanStoreTrillionSizedValues()
    {
        ScoreHandler.ResetScoresPostRound();
        Globals.CurrentChips = 1_000_000_000_000;
        Globals.CurrentMult = 2;

        ScoreHandler.FinalPlayChipsCalc();

        Assert.Equal(2_000_000_000_000, Globals.TotalCurrentChips);
        ScoreHandler.ResetScoresPostRound();
    }

    [Fact]
    public void ChipCounts_CapAndFormatAtMaximum()
    {
        ScoreHandler.ResetScoresPostRound();
        Globals.CurrentChips = Globals.MaxChipCount;
        Globals.CurrentMult = 2;

        ScoreHandler.FinalPlayChipsCalc();

        Assert.Equal(Globals.MaxChipCount, Globals.TotalCurrentChips);
        Assert.Equal("infinite", Globals.FormatChipCount(Globals.TotalCurrentChips));
        ScoreHandler.ResetScoresPostRound();
    }

}
