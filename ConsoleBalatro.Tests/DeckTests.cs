using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Cards.Tags;
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
    public class DeckTests : TestClassBase
    {
        [Fact]
        public void StartRoundWithDeck_RedDeck_CorrectlyAddsDiscard()
        {
            ResetEngineForTest();
            var oldDiscNum = Globals.MaxDiscardsPerRound;
            DeckDb.BecomeDeck("RED");
            Assert.Equal(oldDiscNum + 1, Globals.MaxDiscardsPerRound);
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
            FlowHandler.StartSelectedBlind();
            Assert.Equal(oldDiscNum + 1, Globals.CurDiscardsRemaining);
        }

        [Fact]
        public void StartRoundWithDeck_BlueDeck_CorrectlyAddsHand()
        {
            ResetEngineForTest();
            var oldHandNum = Globals.MaxHandsPerRound;
            DeckDb.BecomeDeck("BLUE");
            Assert.Equal(oldHandNum + 1, Globals.MaxHandsPerRound);
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
            FlowHandler.StartSelectedBlind();
            Assert.Equal(oldHandNum + 1, Globals.CurHandsRemaining);
        }

        [Fact]
        public void StartRoundWithDeck_YellowDeck_CorrectlyAddsMoney()
        {
            ResetEngineForTest();
            var oldMoney = Globals.Money;
            DeckDb.BecomeDeck("YELLOW");
            Assert.Equal(oldMoney + 10, Globals.Money);
        }

        [Fact]
        public void BeatRoundWithDeck_GreenDeck_NoInterestGainMoneyPerHandDisc()
        {
            ResetToBlindDeckSetup("GREEN");
            FlowHandler.StartSelectedBlind();
            var handsLeft = Globals.CurHandsRemaining - 1;//after we play a hand.
            var discLeft = Globals.CurDiscardsRemaining;
            PlayHand("AS,AS,AS,AS,AS");
            var monSources = Globals.CurrentGameStateObj.PostRoundMoneySources;
            Assert.Single(monSources, x => x.Item1.ToUpper().Contains("DISCARDS"));
            Assert.Equal(discLeft, monSources.Last().Item2);
            Assert.Equal(handsLeft * 2, monSources[1].Item2);
            Assert.Contains("BLIND", monSources[0].Item1.ToUpper());
            Assert.Equal(3, monSources.Count);
        }

        [Fact]
        public void StartRoundWithDeck_BlackDeck_AddsJokerLosesHand()
        {
            ResetEngineForTest();
            var oldJokers = ZoneManager.JokerZone.MaxCapacity;
            var oldHands = Globals.MaxHandsPerRound;
            DeckDb.BecomeDeck("BLACK");
            Assert.Equal(oldJokers + 1, ZoneManager.JokerZone.MaxCapacity);
            Assert.Equal(oldHands - 1, Globals.MaxHandsPerRound);
        }

        [Fact]
        public void StartWithDeck_MagicDeck_GivesCorrectVoucherAndCons()
        {
            ResetToBlindDeckSetup("MAGIC");
            var vouch = Assert.Single(ZoneManager.ActiveVoucherZone.Cards);
            Assert.True(vouch.IsVoucher);
            Assert.Equal("CRYSTAL BALL", vouch.JokerData?.DBName);
            Assert.Equal(2, ZoneManager.ConsumableZone.Cards.Count);
            Assert.Equal("FOOL", ZoneManager.ConsumableZone.Cards[0].ConsumableData.DBName);
            Assert.Equal("FOOL", ZoneManager.ConsumableZone.Cards[1].ConsumableData.DBName);
        }

        [Fact]
        public void StartWithDeck_NebulaDeck_GivesCorrectVoucherAndTakesConSlot()
        {
            ResetToBlindDeckSetup("NEBULA");
            var vouch = Assert.Single(ZoneManager.ActiveVoucherZone.Cards);
            Assert.True(vouch.IsVoucher);
            Assert.Equal("TELESCOPE", vouch.JokerData?.DBName);
            Assert.Equal(1, ZoneManager.ConsumableZone.MaxCapacity);
        }

        [Fact]
        public void StartWithDeck_GhostDeck_GivesCorrectConAndRollOdds()
        {
            ResetToBlindDeckSetup("GHOST");
            var hex = Assert.Single(ZoneManager.ConsumableZone.Cards);
            Assert.True(hex.isConsumable);
            Assert.Equal("HEX", hex.ConsumableData.DBName);
            var pretendRollArgs = new EngineMarketTypeBeingChosenArgs() { MyContext = new() { Context = Engine.Events.EventContextType.MarketTypeBeingChosen }, WeightsBeingRolled = new() };
            EngineEventHandler.TriggerEvent(pretendRollArgs);
            Assert.Contains(pretendRollArgs.WeightsBeingRolled, x => x.Key == Engine.Market.BuyItemType.SPECTRAL_CARD);
            //TODO: Maybe roll for an actual card? idk tho.
        }

        [Fact]
        public void StartWithDeck_AbandonedDeck_RemovesAllFaceCards()
        {
            ResetToBlindDeckSetup("ABANDONED");
            Assert.Equal(52 - 12, ZoneManager.DeckZone.Cards.Count);//12 total face cards removed
            Assert.DoesNotContain(ZoneManager.DeckZone.Cards, EngineUtils.isFace);
        }

        [Fact]
        public void StartWithDeck_CheckeredDeck_ContainsOnlySpadesAndHearts()
        {
            ResetToBlindDeckSetup("CHECKERED");
            Assert.Equal(52, ZoneManager.DeckZone.Cards.Count);//same number of cards
            Assert.DoesNotContain(ZoneManager.DeckZone.Cards, x => x.IsSuit(Engine.Cards.Enums.Suit.DIAMONDS));
            Assert.DoesNotContain(ZoneManager.DeckZone.Cards, x => x.IsSuit(Engine.Cards.Enums.Suit.CLUBS));
            Assert.Equal(26, ZoneManager.DeckZone.Cards.Count(x => x.IsSuit(Engine.Cards.Enums.Suit.SPADES)));
            Assert.Equal(26, ZoneManager.DeckZone.Cards.Count(x => x.IsSuit(Engine.Cards.Enums.Suit.HEARTS)));
        }

        [Fact]
        public void StartWithDeck_ZodiacDeck_GivesCorrectVouchers()
        {
            ResetToBlindDeckSetup("ZODIAC");
            Assert.Equal(3, ZoneManager.ActiveVoucherZone.Cards.Count);
            Assert.Equal("TAROT MERCHANT", ZoneManager.ActiveVoucherZone.Cards[0].JokerData?.DBName);
            Assert.Equal("PLANET MERCHANT", ZoneManager.ActiveVoucherZone.Cards[1].JokerData?.DBName);
            Assert.Equal("OVERSTOCK", ZoneManager.ActiveVoucherZone.Cards[2].JokerData?.DBName);
            //for thoroughness sake, go to market and check overstock
            FlowHandler.StartSelectedBlind();
            PlayHand("AS,AS,AS,AS,AS");
            FlowHandler.ClosePostRound();
            Assert.Equal(3, ZoneManager.MainMarketZone.Cards.Count);
        }

        [Fact]
        public void StartRoundWithDeck_PaintedDeck_AddsHandSizeLosesJoker()
        {
            ResetEngineForTest();
            var oldJokers = ZoneManager.JokerZone.MaxCapacity;
            var oldHands = Globals.HandSize;
            DeckDb.BecomeDeck("PAINTED");
            Assert.Equal(oldJokers - 1, ZoneManager.JokerZone.MaxCapacity);
            Assert.Equal(oldHands + 2, Globals.HandSize);
            //For thoroughness sake, go to round and check.
            FlowHandler.StartSelectedBlind();
            Assert.Equal(10, ZoneManager.HandZone.Cards.Count);
        }

        [Fact]
        public void BeatBossWithDeck_AnaglyphDeckGivesDoubleTag()
        {
            ResetToBlindDeckSetup("ANAGLYPH");
            FlowHandler.CurSmallBlindTag = TagType.SPEED;
            FlowHandler.CurBigBlindTag = TagType.SPEED;
            FlowHandler.CurrentBossBlind = "THE PSYCHIC";
            FlowHandler.DoSkip();
            FlowHandler.DoSkip();
            FlowHandler.StartSelectedBlind();
            Globals.RequiredChipsForCurrentBlind = 1;
            Assert.Empty(ZoneManager.TagZone.Cards);
            PlayHand("AS,AS,AS,AS,AS");
            Assert.Equal(GameState.PostRoundRewardsMenu, Globals.CurrentGameState);
            var doubleTag = Assert.Single(ZoneManager.TagZone.Cards);
            Assert.True(doubleTag.IsTag);
            Assert.Equal("Double Tag", doubleTag.JokerData?.JokerName);
        }

        [Fact]
        public void StartWithDeck_PlasmaDeck_HigherChipsAutoBalance()
        {
            ResetToBlindDeckSetup("PLASMA");
            FlowHandler.StartSelectedBlind();
            Assert.Equal(600, Globals.RequiredChipsForCurrentBlind);
            var rec = CaptureScoringContributions();
            PlayHand("KS");//So, base score high-card is 5 x 1. Plus 10 from King, 15 x 1, normally 15 final chips. After balancing, its 8x8 for 64.
            Assert.Equal(64, rec.FinalTotalGain);
        }

        [Fact]
        public void StartWithDeck_ErraticDeck_HasAtLeastOneDuplicateCard()
        {
            //So, fun statistics fact; it is essentially guaranteed (1 - (1.24 x 10^-20) chance) that after randomization, there are at least two cards in deck that share the same rank and suit.
            //Normal deck doesn't have that. So checking that PRETTY MUCH checks that erratic deck randomized the deck successfully.
            ResetToBlindDeckSetup("ERRATIC");
            Assert.Contains(ZoneManager.DeckZone.Cards.GroupBy(c => new { c.Rank, c.Suit }), g => g.Count() > 1);
        }
    }
}
