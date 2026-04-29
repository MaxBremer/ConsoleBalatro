using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using Xunit;

namespace ConsoleBalatro.Tests;

public class BasicTests
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
}
