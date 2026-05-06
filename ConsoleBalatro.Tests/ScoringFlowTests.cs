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

            TestWithExpectations(contributions, PlayedHandType.HIGHCARD, Rank.ACE, Suit.SPADES, 1, 11, 16);
            Assert.Equal(5, ZoneManager.HiddenPlayZone.Cards.Count);//Cards should be moved to play zone.
        }

        [Fact]
        public void ScorePlayedHands_MultipleDifferentHands_CountsOfTimesPlayedTrackedCorrectly()
        {
            ResetToFirstBlindPlayRound();

            PlayHand("AS");
            PlayHand("AS");
            PlayHand("2D,2D");
            PlayHand("KC,KC,KC,KC");

            Assert.Equal(2, ScoreHandler.HandNumTimesPlayed[PlayedHandType.HIGHCARD]);
            Assert.Equal(1, ScoreHandler.HandNumTimesPlayed[PlayedHandType.PAIR]);
            Assert.Equal(1, ScoreHandler.HandNumTimesPlayed[PlayedHandType.FOUROFAKIND]);
            Assert.Equal(0, ScoreHandler.HandNumTimesPlayed[PlayedHandType.THREEOFAKIND]);

            FlowHandler.ClosePostRound();
            FlowHandler.StartSelectedBlind();
            PlayHand("AS");
            PlayHand("2D,2D");
            PlayHand("KC,KC,KC");
            PlayHand("KC,KC,KC,KC");
            Assert.Equal(3, ScoreHandler.HandNumTimesPlayed[PlayedHandType.HIGHCARD]);
            Assert.Equal(2, ScoreHandler.HandNumTimesPlayed[PlayedHandType.PAIR]);
            Assert.Equal(2, ScoreHandler.HandNumTimesPlayed[PlayedHandType.FOUROFAKIND]);
            Assert.Equal(1, ScoreHandler.HandNumTimesPlayed[PlayedHandType.THREEOFAKIND]);
        }

        [Theory]
        [InlineData("9C,9D,2S,3H,4C", PlayedHandType.PAIR, Rank.NINE, Suit.CLUBS, 2, 18, 56)]
        [InlineData("9C,9D,2S,2H,4C", PlayedHandType.TWOPAIR, Rank.NINE, Suit.CLUBS, 4, 22, 84)]
        [InlineData("JC,JD,JS,3H,4C", PlayedHandType.THREEOFAKIND, Rank.JACK, Suit.CLUBS, 3, 30, 180)]
        [InlineData("AC,5D,2S,3H,4C", PlayedHandType.STRAIGHT, Rank.ACE, Suit.CLUBS, 5, 25, 220)]//Ace-low straight
        [InlineData("AC,1D,JS,QH,KC", PlayedHandType.STRAIGHT, Rank.ACE, Suit.CLUBS, 5, 51, 324)]//Ace-high straight
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

            TestWithExpectations(contributions, playedHandType, firstChipSourceRank, firstChipSourceSuit, numChipContributors, chipsFromEmits, totalChipsAtEnd);
        }

        [Theory]
        [InlineData(11, 16)]
        [InlineData(41, 46, 2, 0, 0d, Edition.BASE, Enhancement.BONUSCHIPS)]
        [InlineData(61, 66, 2, 0, 0d, Edition.FOIL)]
        [InlineData(11, 80, 1, 1, 4d, Edition.BASE, Enhancement.MULT)]
        [InlineData(11, 32, 1, 0, 0d, Edition.BASE, Enhancement.GLASS, Seal.NONE, 1, 2d)]
        [InlineData(11, 48, 1, 0, 0d, Edition.POLYCHROME, Enhancement.GLASS, Seal.NONE, 2, 3d)]
        [InlineData(22, 243, 2, 0, 0d, Edition.POLYCHROME, Enhancement.GLASS, Seal.RED, 4, 9d)]
        [InlineData(22, 108, 2, 0, 0d, Edition.BASE, Enhancement.GLASS, Seal.RED, 2, 4d)]
        [InlineData(11, 176, 1, 1, 10d, Edition.HOLOGRAPHIC)]
        [InlineData(11, 240, 1, 2, 14d, Edition.HOLOGRAPHIC, Enhancement.MULT)]
        [InlineData(22, 783, 2, 4, 28d, Edition.HOLOGRAPHIC, Enhancement.MULT, Seal.RED)]
        [InlineData(22, 27, 2, 0, 0d, Edition.BASE, Enhancement.NONE, Seal.RED)]
        [InlineData(82, 87, 4, 0, 0d, Edition.BASE, Enhancement.BONUSCHIPS, Seal.RED)]
        [InlineData(82, 1827, 4, 2,20d, Edition.HOLOGRAPHIC, Enhancement.BONUSCHIPS, Seal.RED)]
        [InlineData(61, 1386, 2, 1, 20d, Edition.FOIL, Enhancement.LUCKY)]
        [InlineData(11, 24, 1, 0, 0d, Edition.POLYCHROME, Enhancement.NONE, Seal.NONE, 1, 1.5d)]
        [InlineData(11, 120, 1, 1, 4d, Edition.POLYCHROME, Enhancement.MULT, Seal.NONE, 1, 1.5d)]
        [InlineData(22, 465, 2, 2, 8d, Edition.POLYCHROME, Enhancement.MULT, Seal.RED, 2, 2.25d)]//TODO: Rounds down, but result is +.75. Should round up?
        public void ScorePlayedHand_HighCardWithEditionEnhancementAndSeal_SuccessfullyCalculates(int chipContributions, int totalChipsAtEnd, int numChipContributors = 1, int numMultContributors = 0, double multAmountContribution = 0d, Edition edition = Edition.BASE, Enhancement enhancement = Enhancement.NONE, Seal seal = Seal.NONE, int numMultMultContributors = 0, double multMultContributed = 1d)
        {
            ResetToFirstBlindPlayRound();

            var selected = BuildKnownHand("AS,2D,3C,4H,6S");
            selected[0].Edition = edition;
            selected[0].Enhancement = enhancement;
            selected[0].Seal = seal;

            if(enhancement == Enhancement.LUCKY)
                RigNextRoll(true);
            else if(enhancement == Enhancement.GLASS)
                RigNextRoll(false);

                var contributions = CaptureScoringContributions();
            Globals.PlayCurrentlySelectedHand();

            TestWithExpectations(contributions, PlayedHandType.HIGHCARD, Rank.ACE, Suit.SPADES, numChipContributors, chipContributions, totalChipsAtEnd, numMultContributors: numMultContributors, multFromEmits: multAmountContribution, numMultMultContributors: numMultMultContributors, multMultFromEmits: multMultContributed);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ScorePlayedHand_PairWithInHandModifiers_SuccessfullyCalculates(bool addRedSeal)
        {
            ResetToFirstBlindPlayRound();

            var selected = BuildKnownHand("AS,AD,3C,4H,6S");

            selected[2].SetEnhancementOfficial(Enhancement.STEEL);
            if (addRedSeal)
                selected[2].Seal = Seal.RED;
            selected[2].isSelected = false;

            var contributions = CaptureScoringContributions();
            Globals.PlayCurrentlySelectedHand();

            TestWithExpectations(contributions, PlayedHandType.PAIR, Rank.ACE, Suit.SPADES, 2, 22, addRedSeal ? 144 : 96, numMultMultContributors: addRedSeal ? 2 : 1, multMultFromEmits: addRedSeal ? 2.25d : 1.5d);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ScorePlayedHand_WithStoneCard_SuccessfullyCalculates(bool doSecondStone)
        {
            ResetToFirstBlindPlayRound();

            var selected = BuildKnownHand("AS,AD,3C,4H,6S");

            selected[2].SetEnhancementOfficial(Enhancement.STONE);
            if (doSecondStone)
                selected[3].SetEnhancementOfficial(Enhancement.STONE);

            var contributions = CaptureScoringContributions();
            Globals.PlayCurrentlySelectedHand();

            TestWithExpectations(contributions, PlayedHandType.PAIR, Rank.ACE, Suit.SPADES, doSecondStone ? 4 : 3, doSecondStone ? 122 : 72, doSecondStone ? 264 : 164);
        }

        [Fact]
        public void ScorePlayedHand_GlassCard_DestroysAfterScoring()
        {
            ResetToFirstBlindPlayRound();
            var selected = BuildKnownHand("AS,2D,3C,4H,6S");
            selected[0].SetEnhancementOfficial(Enhancement.GLASS);
            RigNextRoll(true);
            Globals.PlayCurrentlySelectedHand();
            Assert.Empty(ZoneManager.CurrentlyBeingPlayedZone.Cards);
            Assert.Equal(4, ZoneManager.HiddenPlayZone.Cards.Count);//cause glass card was destroyed.
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

        [Fact]
        public void ScorePlayedHand_NotPlayRound_FailsGracefully()
        {
            ResetToBlindSelection();
            var selected = BuildKnownHand("AS,2D,3C,4H,6S");//Played hand WOULD be high-card Ace.

            var expectedBefore = Globals.TotalCurrentChips;

            Globals.PlayCurrentlySelectedHand();

            Assert.Equal(expectedBefore, Globals.TotalCurrentChips);
            Assert.Empty(ZoneManager.CurrentlyBeingPlayedZone.Cards);
            Assert.Empty(ZoneManager.HiddenPlayZone.Cards);
            Assert.Equal(GameState.BlindsMenu, Globals.CurrentGameState);
        }

        #region Helpers

        private static void TestWithExpectations(ContributionCapture contributions, PlayedHandType playedHandType, Rank firstChipSourceRank, Suit firstChipSourceSuit, int numChipContributors, int chipsFromEmits, int totalChipsAtEnd, int numMultContributors = 0, double multFromEmits = 0d, int numMultMultContributors = 0, double multMultFromEmits = 1d)
        {
            //So remember, base hand score doesn't emit anything. Only cards calced do.
            Assert.Equal(chipsFromEmits, contributions.ChipsFromEmits);
            Assert.Equal(multFromEmits, contributions.MultFromEmits);
            Assert.Equal(multMultFromEmits, contributions.MultMultFromEmits);
            Assert.Equal(totalChipsAtEnd, contributions.FinalTotalGain);

            Assert.NotEmpty(contributions.ChipSources);
            Assert.Equal(numMultContributors, contributions.MultSources.Count);
            Assert.Equal(numChipContributors, contributions.ChipSources.Count);
            Assert.Equal(numMultMultContributors, contributions.MultMultSources.Count);
            Assert.Equal(firstChipSourceRank, contributions.ChipSources[0].Rank);
            Assert.Equal(firstChipSourceSuit, contributions.ChipSources[0].Suit);

            Assert.Single(contributions.PlayedHandTypes);
            Assert.Equal(playedHandType, contributions.PlayedHandTypes[0]);
        }

        

    }
        #endregion
}
