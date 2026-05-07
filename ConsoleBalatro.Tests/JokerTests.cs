using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleBalatro.Tests
{
    public class JokerTests : TestClassBase
    {
        [Fact]
        public void PlayHand_HasJimbo_CorrectlyAddsMult()
        {
            var s = JokerSetup("JIMBO");

            PlayHand("AS,AS");
            Assert.Single(s.record.MultSources);
            Assert.Contains(s.jok, s.record.MultSources);
            Assert.Equal(4, s.record.MultFromEmits);
        }

        [Theory]
        [InlineData(Suit.DIAMONDS, "D", "GREEDY JOKER")]
        [InlineData(Suit.HEARTS, "H", "LUSTY JOKER")]
        [InlineData(Suit.SPADES, "S", "WRATHFUL JOKER")]
        [InlineData(Suit.CLUBS, "C", "GLUTTONOUS JOKER")]
        public void PlayHand_HasSuitMultJoker_CorrectlyAddsMult(Suit targetSuit, string suitString, string jokerName)
        {
            var s = JokerSetup(jokerName);
            var record = s.record;
            var jok = s.jok;

            var handStr = "A" + suitString + ",A" + suitString;
            PlayHand(handStr);
            Assert.Equal(targetSuit, ZoneManager.HiddenPlayZone.Cards[0].Suit);
            Assert.Equal(2, record.MultSources.Count);
            Assert.Equal(jok, record.MultSources[0]);
            Assert.Equal(jok, record.MultSources[1]);
            Assert.Equal(6, record.MultFromEmits);
            Assert.Equal(256, record.FinalTotalGain);
        }

        [Theory]
        [InlineData("JOLLY JOKER", PlayedHandType.PAIR, "KS,KS", 0, 0, 8)]
        [InlineData("ZANY JOKER", PlayedHandType.THREEOFAKIND, "KS,KS,KS", 0, 0, 12)]
        [InlineData("MAD JOKER", PlayedHandType.TWOPAIR, "KS,KS,QS,QS", 0, 0, 10)]
        [InlineData("CRAZY JOKER", PlayedHandType.STRAIGHT, "KS,QS,JD,1C,9D", 0, 0, 12)]
        [InlineData("DROLL JOKER", PlayedHandType.FLUSH, "KS,QS,2S,5S,6S", 0, 0, 10)]
        [InlineData("SLY JOKER", PlayedHandType.PAIR, "KS,KS", 50, 20)]
        [InlineData("WILY JOKER", PlayedHandType.THREEOFAKIND, "KS,KS,KS", 100, 30)]
        [InlineData("CLEVER JOKER", PlayedHandType.TWOPAIR, "KS,KS,QS,QS", 80, 40)]
        [InlineData("DEVIOUS JOKER", PlayedHandType.STRAIGHT, "KS,QD,JS,1D,9S", 100, 49)]
        [InlineData("CRAFTY JOKER", PlayedHandType.FLUSH, "KS,JS,9S,2S,3S", 80, 34)]
        [InlineData("HALF JOKER", PlayedHandType.HIGHCARD, "KS,JS,9S", 0, 0, 20)]
        public void PlayHand_HasSpecificHandBonusJoker_CorrectlyAddsMultOrChips(string jokerName, PlayedHandType handType, string handString, int chipsAdded, int chipsFromCardEmits, double multAdded = 0)
        {
            var s = JokerSetup(jokerName);
            var record = s.record;
            var jok = s.jok;

            var handSize = handString.Split(",").Count();

            PlayHand(handString);
            if(multAdded != 0)
            {
                Assert.Single(record.MultSources);
                Assert.Equal(jok, record.MultSources[0]);
            }
            else
            {
                Assert.Empty(record.MultSources);
            }
            if(chipsAdded != 0)
            {
                Assert.Equal(handSize + 1, record.ChipSources.Count);
                Assert.Contains(jok, record.ChipSources);
                Assert.Equal(record.ChipsFromEmits - chipsFromCardEmits, chipsAdded);
            }
        }

        [Fact]
        public void PlayHand_WithStencilJoker_AddsAppropriateMultMult()
        {
            var s = JokerSetup("STENCIL JOKER");

            PlayHand("AS");
            Assert.Single(s.record.MultMultSources);
            Assert.Equal(s.jok, s.record.MultMultSources[0]);
            Assert.Equal(5, s.record.MultMultFromEmits);
            //record reset
            s.record.MultMultSources.Clear();
            s.record.MultMultFromEmits = 1;
            AddJoker("JIMBO");
            //now the mult should go down to 4
            PlayHand("AS");
            Assert.Single(s.record.MultMultSources);
            Assert.Equal(s.jok, s.record.MultMultSources[0]);
            Assert.Equal(4, s.record.MultMultFromEmits);
        }


        [Fact]
        public void PlayHand_WithFourFingers_AllowsFourCardStraightAndFlush()
        {
            var s = JokerSetup("FOUR FINGERS");

            PlayHand("KS,QS,JS,1S");
            Assert.Single(s.record.PlayedHandTypes);
            Assert.Equal(PlayedHandType.STRAIGHTFLUSH, s.record.PlayedHandTypes[0]);

            ZoneManager.JokerZone.RemoveCard(s.jok);
            Assert.Equal(5, EngineUtils.LenFlush);
            Assert.Equal(5, EngineUtils.LenStraight);
        }

        [Fact]
        public void PlayHand_WithMime_DoublesInHandCardTriggers()
        {
            JokerSetup("MIME");
            var record = CaptureScoringContributions();

            var cards = BuildKnownHand("AS,KH", selectAll: false);
            cards[0].isSelected = true;
            cards[1].Enhancement = Enhancement.STEEL;

            Globals.PlayCurrentlySelectedHand();
            Assert.Equal(2.25, record.MultMultFromEmits);
            Assert.Equal(2, record.MultMultSources.Count);
            Assert.All(record.MultMultSources, c => Assert.Equal(cards[1], c));
        }

        [Fact]
        public void AddRemoveCreditCard_UpdatesMinimumMoneyAllowed()
        {
            ResetToFirstBlindPlayRound();
            Assert.Equal(0, Globals.MinimumMoneyAllowed);

            AddJoker("CREDIT CARD");
            Assert.Equal(-20, Globals.MinimumMoneyAllowed);

            ZoneManager.JokerZone.RemoveCard(ZoneManager.JokerZone.Cards[0]);
            Assert.Equal(0, Globals.MinimumMoneyAllowed);
        }

        [Fact]
        public void CloseRound_WithGoldenJoker_AddsPostRoundMoneySource()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("GOLDEN JOKER");

            Globals.RequiredChipsForCurrentBlind = 1;
            PlayHand("AS");

            Assert.Equal(GameState.PostRoundRewardsMenu, Globals.CurrentGameState);
            Assert.Contains(Globals.CurrentGameStateObj.PostRoundMoneySources, x => x.Item1 == "Golden Joker" && x.Item2 == 4);
        }

        [Fact]
        public void CloseBlindSelection_WithCeremonialDagger_DestroysRightJokerAndAddsMult()
        {
            ResetToBlindSelection();
            AddJoker("CEREMONIAL DAGGER");
            AddJoker("JIMBO");

            var dagger = GetJoker(0);
            var sacrificed = GetJoker(1);
            var expectedAddedMult = sacrificed.SellCost * 2;

            FlowHandler.CloseBlindSelectionRound();

            Assert.Single(ZoneManager.JokerZone.Cards);
            Assert.Equal(dagger, ZoneManager.JokerZone.Cards[0]);
            Assert.Equal(expectedAddedMult, dagger.JokerData.DataDict["MULTAMOUNT"].DoubleData);
        }

        [Fact]
        public void PlayHand_WithMysticSummit_AddsMultOnlyWhenNoDiscardsRemain()
        {
            var s = JokerSetup("MYSTIC SUMMIT");

            Globals.CurDiscardsRemaining = 1;
            PlayHand("AS");
            Assert.Empty(s.record.MultSources);

            s.record.MultSources.Clear();
            s.record.MultFromEmits = 0;

            Globals.CurDiscardsRemaining = 0;
            PlayHand("AS");
            Assert.Single(s.record.MultSources);
            Assert.Equal(s.jok, s.record.MultSources[0]);
            Assert.Equal(15, s.record.MultFromEmits);
        }

        [Fact]
        public void CloseBlindSelection_WithMarbleJoker_AddsStoneCardToDeck()
        {
            ResetToBlindSelection();
            AddJoker("MARBLE JOKER");
            var beforeCount = ZoneManager.DeckZone.Cards.Count;

            FlowHandler.CloseBlindSelectionRound();

            Assert.Equal(beforeCount + 1, ZoneManager.DeckZone.Cards.Count);
            Assert.Contains(ZoneManager.DeckZone.Cards, c => c.Enhancement == Enhancement.STONE);
        }

        [Fact]
        public void PlayHand_WithLoyaltyCard_TriggersEverySixthHand()
        {
            var s = JokerSetup("LOYALTY CARD");
            Globals.CurHandsRemaining = 9;

            for (var i = 0; i < 5; i++)
                PlayHand("AS");

            //No mult applied, and 1 remaining before next trigger, as in next trigger will have mult.
            Assert.Empty(s.record.MultMultSources);
            Assert.Equal(1, s.jok.JokerData.DataDict["REMAINING"].IntData);

            PlayHand("AS");

            Assert.Single(s.record.MultMultSources);
            Assert.Equal(s.jok, s.record.MultMultSources[0]);
            Assert.Equal(4, s.record.MultMultFromEmits);
            Assert.Equal(6, s.jok.JokerData.DataDict["REMAINING"].IntData);
        }

        [Fact]
        public void PlayHand_With8Ball_CreatesTarotWhenRollSucceeds()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("8 BALL");
            RigNextRoll(true);
            var beforeCount = ZoneManager.ConsumableZone.Cards.Count;

            PlayHand("8S");

            Assert.Equal(beforeCount + 1, ZoneManager.ConsumableZone.Cards.Count);
            Assert.True(ZoneManager.ConsumableZone.Cards.Last().isConsumable);
        }

        [Fact]
        public void PlayHand_WithMisprint_RollsMultInExpectedRange()
        {
            var s = JokerSetup("MISPRINT");

            PlayHand("AS");

            Assert.Single(s.record.MultSources);
            Assert.Equal(s.jok, s.record.MultSources[0]);
            Assert.InRange(s.record.MultFromEmits, 0, 23);
            Assert.Equal(s.record.MultFromEmits, s.jok.JokerData.DataDict["MULTAMOUNT"].DoubleData);
        }

        [Fact]
        public void PlayHand_WithDusk_RetriggersPlayedCardsOnlyOnFinalHand()
        {
            JokerSetup("DUSK");
            var record = CaptureScoringContributions();
            var card = BuildKnownHand("AS")[0];

            Globals.CurHandsRemaining = 2;
            Globals.PlayCurrentlySelectedHand();
            Assert.Single(record.ChipSources, x => x == card);

            record.ChipSources.Clear();
            record.ChipsFromEmits = 0;

            card.isSelected = true;
            ZoneManager.HandZone.Cards.Add(card);
            Globals.CurHandsRemaining = 1;
            Globals.PlayCurrentlySelectedHand();
            Assert.Equal(2, record.ChipSources.Count(x => x == card));
        }

        /*[Theory]
        [InlineData("TEMP UNCOMMON JOKER")]
        [InlineData("TEMP RARE JOKER")]
        [InlineData("TEMP LEGENDARY JOKER")]
        public void AddJoker_WithTempJokers_LoadsWithoutErrors(string jokerName)
        {
            ResetToFirstBlindPlayRound();
            AddJoker(jokerName);

            Assert.Contains(ZoneManager.JokerZone.Cards, x => x.isJoker && x.JokerData.DBName == jokerName);
        }*/
        private Card GetJoker(int ind) => ZoneManager.JokerZone.Cards[ind];
        private (Card jok, ContributionCapture record) JokerSetup(string jokerName)
        {
            ResetToFirstBlindPlayRound();
            var record = CaptureScoringContributions();
            Assert.Empty(ZoneManager.JokerZone.Cards);
            AddJoker(jokerName);
            Assert.Single(ZoneManager.JokerZone.Cards);
            var jok = GetJoker(0);
            return (jok, record);
        }
    }
}
