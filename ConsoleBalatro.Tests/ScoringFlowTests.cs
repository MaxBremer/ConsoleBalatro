using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleBalatro.Tests
{
    public class ScoringFlowTests : TestClassBase
    {
        [Fact]
        public void ScorePlayedHand_BaselineHighCard_ProducesExpectedTotals()
        {
            ResetToFirstBlindPlayRound();

            var selected = BuildKnownHand("AS,2D,3C,4H,6S");//Played hand should be high-card Ace.

            var contributions = CaptureScoringContributions();
            Globals.PlayCurrentlySelectedHand();

            //So remember, base hand score doesn't emit anything. Only cards calced do.
            Assert.Equal(11, contributions.ChipsFromEmits);//Should gain 11 chips from the Ace only
            Assert.Equal(0d, contributions.MultAfterEmits);//No mult gained from anywhere
            Assert.Equal(16, contributions.FinalTotalGain);//5 for high card base + 11 for Ace.

            //There should only be 1 chip source and no mult sources, that 1 chip source should be ace of spades.
            Assert.NotEmpty(contributions.ChipSources);
            Assert.Single(contributions.ChipSources);
            Assert.Empty(contributions.MultSources);
            Assert.Equal(Rank.ACE, contributions.ChipSources[0].Rank);
            Assert.Equal(Suit.SPADES, contributions.ChipSources[0].Suit);

            //Should be one played hand, a high card ace.
            Assert.Single(contributions.PlayedHandTypes);
            Assert.Equal(PlayedHandType.HIGHCARD, contributions.PlayedHandTypes[0]);
        }

        [Fact]
        public void ScorePlayedHand_EmptySelection_FailsGracefullyAndKeepsState()
        {
            ResetToFirstBlindPlayRound();

            var expectedBefore = Globals.TotalCurrentChips;

            Globals.PlayCurrentlySelectedHand();

            Assert.Equal(expectedBefore, Globals.TotalCurrentChips);
            Assert.Empty(ZoneManager.CurrentlyBeingPlayedZone.Cards);
            Assert.Empty(ZoneManager.HiddenPlayZone.Cards);
            Assert.Equal(4, Globals.CurHandsRemaining);//Base hands per round is 4
        }

        [Theory]
        [InlineData("9C,9D,2S,3H,4C", PlayedHandType.PAIR, Rank.NINE, Suit.CLUBS, 2, 18, 56)]
        [InlineData("9C,9D,2S,2H,4C", PlayedHandType.TWOPAIR, Rank.NINE, Suit.CLUBS, 4, 22, 84)]
        [InlineData("JC,JD,JS,3H,4C", PlayedHandType.THREEOFAKIND, Rank.JACK, Suit.CLUBS, 3, 30, 180)]
        [InlineData("AC,5D,2S,3H,4C", PlayedHandType.STRAIGHT, Rank.ACE, Suit.CLUBS, 5, 25, 220)]
        [InlineData("9H,9H,2H,3H,4H", PlayedHandType.FLUSH, Rank.NINE, Suit.HEARTS, 5, 27, 248)]
        [InlineData("9H,9S,2D,2C,2H", PlayedHandType.FULLHOUSE, Rank.NINE, Suit.HEARTS, 5, 24, 256)]
        [InlineData("1H,1C,1S,1D,4H", PlayedHandType.FOUROFAKIND, Rank.TEN, Suit.HEARTS, 4, 40, 700)]
        [InlineData("8H,9H,7H,6H,5H", PlayedHandType.STRAIGHTFLUSH, Rank.EIGHT, Suit.HEARTS, 5, 35, 1080)]
        [InlineData("AH,AH,AH,AH,AS", PlayedHandType.FIVEOFAKIND, Rank.ACE, Suit.HEARTS, 5, 55, 2100)]
        [InlineData("8H,8H,7H,7H,7H", PlayedHandType.FLUSHHOUSE, Rank.EIGHT, Suit.HEARTS, 5, 37, 2478)]
        [InlineData("AS,AS,AS,AS,AS", PlayedHandType.FLUSHFIVE, Rank.ACE, Suit.SPADES, 5, 55, 3440)]
        public void ScorePlayedHand_GivenHandAndOutcome_SuccesfullyCalculates(string handToBuild, PlayedHandType playedHandType, Rank firstChipSourceRank, Suit firstChipSourceSuit, int numChipContributors, int chipsFromEmits, int totalChipsAtEnd)
        {
            ResetToFirstBlindPlayRound();

            var selected = BuildKnownHand(handToBuild);

            var contributions = CaptureScoringContributions();
            Globals.PlayCurrentlySelectedHand();

            //So remember, base hand score doesn't emit anything. Only cards calced do.
            Assert.Equal(chipsFromEmits, contributions.ChipsFromEmits);
            Assert.Equal(0d, contributions.MultAfterEmits);//No mult gained from anywhere
            Assert.Equal(totalChipsAtEnd, contributions.FinalTotalGain);

            Assert.NotEmpty(contributions.ChipSources);
            Assert.Empty(contributions.MultSources);
            Assert.Equal(numChipContributors, contributions.ChipSources.Count);
            Assert.Equal(firstChipSourceRank, contributions.ChipSources[0].Rank);
            Assert.Equal(firstChipSourceSuit, contributions.ChipSources[0].Suit);

            Assert.Single(contributions.PlayedHandTypes);
            Assert.Equal(playedHandType, contributions.PlayedHandTypes[0]);
        }

        #region Helpers

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

            //Individual Mult/Chip gains
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

            //Final, total chip gain.
            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.TotalChipsGained,
                MyAction = args =>
                {
                    var total = Assert.IsType<EngineTotalChipsGainArgs>(args);
                    capture.FinalTotalGain = total.AmountBeingGained;
                }
            });

            //Hand(s) played.
            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.HandPlayedCalculated,
                MyAction = args =>
                {
                    var played = Assert.IsType<EngineHandPlayArgs>(args);
                    capture.PlayedHandTypes.Add(played.HandBeingPlayed);
                }
            });

            return capture;
        }

        private sealed class ContributionCapture
        {
            public int ChipsFromEmits { get; set; }
            public double MultAfterEmits { get; set; }
            public int FinalTotalGain { get; set; }
            public List<Card> ChipSources { get; } = new();
            public List<Card> MultSources { get; } = new();

            public List<PlayedHandType> PlayedHandTypes { get; set; } = new();
        }

    }
        #endregion
}
