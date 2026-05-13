using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
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
            Assert.Equal(targetSuit, ZoneManager.HiddenPlayZone.Cards[1].Suit);
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
            s.record.Reset();
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
            var record = CaptureScoringContributions();

            var dagger = GetJoker(0);
            var sacrificed = GetJoker(1);
            var expectedAddedMult = sacrificed.SellCost * 2;

            FlowHandler.StartSelectedBlind();

            Assert.Single(ZoneManager.JokerZone.Cards);
            Assert.Equal(dagger, ZoneManager.JokerZone.Cards[0]);
            Assert.Equal(expectedAddedMult, dagger.JokerData.DataDict["MULTAMOUNT"].DoubleData);
            PlayHand("AS");
            Assert.Single(record.MultSources);
            Assert.Contains(dagger, record.MultSources);
            Assert.Equal(expectedAddedMult, record.MultFromEmits);

        }

        [Fact]
        public void PlayHand_WithMysticSummit_AddsMultOnlyWhenNoDiscardsRemain()
        {
            var s = JokerSetup("MYSTIC SUMMIT");

            Globals.CurDiscardsRemaining = 1;
            PlayHand("AS");
            Assert.Empty(s.record.MultSources);

            s.record.Reset();

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

            record.Reset();

            card.isSelected = true;
            ZoneManager.HandZone.Cards.Add(card);
            Globals.CurHandsRemaining = 1;
            Globals.PlayCurrentlySelectedHand();
            Assert.Equal(2, record.ChipSources.Count(x => x == card));
        }
        [Fact]
        public void PlayHand_WithRaisedFist_AddsDoubleLowestHeldInHandRankToMult()
        {
            var s = JokerSetup("RAISED FIST");
            BuildKnownHand("KS,2H,5C", selectAll: false);
            ZoneManager.HandZone.Cards[0].isSelected = true;
            Globals.PlayCurrentlySelectedHand();

            Assert.Single(s.record.MultSources);
            Assert.Equal(s.jok, s.record.MultSources[0]);
            Assert.Equal(4, s.record.MultFromEmits);
            s.record.Reset();

            BuildKnownHand("KS,2H,5C", selectAll: false);
            ZoneManager.HandZone.Cards[0].isSelected = true;
            ZoneManager.HandZone.Cards[1].isSelected = true;
            Globals.PlayCurrentlySelectedHand();
            Assert.Single(s.record.MultSources);
            Assert.Equal(s.jok, s.record.MultSources[0]);
            Assert.Equal(10, s.record.MultFromEmits);
        }

        [Fact]
        public void StartMarket_WithChaosTheClown_MakesFirstRerollFree()
        {
            JokerSetup("CHAOS THE CLOWN");
            PlayHand("AS,AS,AS,AS,AS");
            Globals.CurrentRerollCost = 999;
            FlowHandler.ClosePostRound();

            Assert.Equal(GameState.ShopMenu, Globals.CurrentGameState);
            Assert.Equal(0, Globals.CurrentRerollCost);

            MarketGeneralManager.RerollMainMarket();
            Assert.Equal(Globals.BaseRerollCost, Globals.CurrentRerollCost);
        }

        [Fact]
        public void PlayHand_WithFibonacci_AddsMultForEachPlayedFibonacciRank()
        {
            var s = JokerSetup("FIBONACCI");
            PlayHand("AS,2S,3S,5S,8S");

            Assert.Equal(5, s.record.MultSources.Count);
            Assert.All(s.record.MultSources, x => Assert.Equal(s.jok, x));
            Assert.Equal(40, s.record.MultFromEmits);
        }

        [Fact]
        public void PlayHand_WithSteelJoker_AddsMultMultBasedOnSteelCardsInDeck()
        {
            var s = JokerSetup("STEEL JOKER");
            var steelInDeck = ZoneManager.DeckZone.Cards[0];
            steelInDeck.Enhancement = Enhancement.STEEL;

            var steelInHand = ZoneManager.DeckZone.Cards[1];
            steelInHand.Enhancement = Enhancement.STEEL;
            ZoneManager.HandZone.DrawTargetFrom(ZoneManager.DeckZone, steelInHand, ignoreSpaceLimits: true);

            var steelInDiscard = ZoneManager.DeckZone.Cards[1];
            steelInDiscard.Enhancement = Enhancement.STEEL;
            ZoneManager.DiscardZone.DrawTargetFrom(ZoneManager.DeckZone, steelInDiscard);

            BuildKnownHand("AS", clearHand: false);
            Globals.PlayCurrentlySelectedHand();

            Assert.Equal(2, s.record.MultMultSources.Count);//1 from the steel card in hand, 1 from the steel joker.
            Assert.Equal(s.jok, s.record.MultMultSources[1]);//Steel is a global listener, so it'll always go first.
            Assert.Equal(1.6 * 1.5, s.record.MultMultFromEmits);//(expected steel joker) * (expected steel card in hand)
        }

        [Fact]
        public void PlayHand_WithScaryFace_AddsChipsForPlayedFaceCards()
        {
            var s = JokerSetup("SCARY FACE");
            PlayHand("JS,QS,KS,1S,1S");

            Assert.Equal(3, s.record.ChipSources.Count(x => x == s.jok));
            Assert.Equal(90, s.record.ChipsFromEmits - 50);
        }

        [Fact]
        public void PlayHand_WithAbstractJoker_AddsMultBasedOnJokerCount()
        {
            var s = JokerSetup("ABSTRACT JOKER");
            AddJoker("JIMBO");
            PlayHand("AS");

            Assert.Contains(s.jok, s.record.MultSources);
            Assert.Equal(6 + 4, s.record.MultFromEmits);
            s.record.Reset();

            AddJoker("JIMBO");
            PlayHand("AS");
            Assert.Contains(s.jok, s.record.MultSources);
            Assert.Equal(9 + 4 + 4, s.record.MultFromEmits);
        }

        [Fact]
        public void CloseRound_WithDelayedGratification_GivesMoneyOnlyIfNoDiscardsUsed()
        {
            ResetToBlindSelection();
            AddJoker("DELAYED GRATIFICATION");//TODO: Delayed Grat sets its Max_Discards at start of play round, so if added during play round Max_Discards will be its default of 0. May want to change this in the future, but for now just add during blind selection.
            FlowHandler.StartSelectedBlind();
            Globals.RequiredChipsForCurrentBlind = 1;
            PlayHand("AS");

            Assert.Contains(Globals.CurrentGameStateObj.PostRoundMoneySources, x => x.Item1 == "Delayed Gratification" && x.Item2 == 6);

            ResetToBlindSelection();
            AddJoker("DELAYED GRATIFICATION");
            FlowHandler.StartSelectedBlind();
            DiscardHand("AS");
            Globals.RequiredChipsForCurrentBlind = 1;
            PlayHand("KS");
            Assert.DoesNotContain(Globals.CurrentGameStateObj.PostRoundMoneySources, x => x.Item1 == "Delayed Gratification");
        }

        [Fact]
        public void PlayHand_WithHack_RetriggersPlayedCardsTwoThroughFive()
        {
            JokerSetup("HACK");
            var record = CaptureScoringContributions();
            
            BuildKnownHand("2S,3S,4S,5S,AS");
            var played = ZoneManager.CardsSelectedInHand.ToList();
            Globals.PlayCurrentlySelectedHand();

            Assert.Equal(2, record.ChipSources.Count(x => x == played[0]));
            Assert.Equal(2, record.ChipSources.Count(x => x == played[1]));
            Assert.Equal(2, record.ChipSources.Count(x => x == played[2]));
            Assert.Equal(2, record.ChipSources.Count(x => x == played[3]));
            Assert.Equal(1, record.ChipSources.Count(x => x == played[4]));
            Assert.Equal(Rank.ACE, played[4].Rank);//just making sure the correct card didn't retrigger.
        }

        [Fact]
        public void PlayHand_WithPareidolia_MakesAllScoredCardsCountAsFaceCards()
        {
            var s = JokerSetup("SCARY FACE");
            AddJoker("PAREIDOLIA");
            PlayHand("2S,4S,6S,8S,1S");

            Assert.Equal(5, s.record.ChipSources.Count(x => x == s.jok));
            Assert.Equal(150, s.record.ChipsFromEmits - 30);

            FlowHandler.ClosePostRound();
            FlowHandler.CloseMarketRound();
            FlowHandler.StartSelectedBlind();
            s.record.Reset();
            ZoneManager.DestroyCard(ZoneManager.JokerZone.Cards[1]);//remove Pareidolia
            PlayHand("KS,4S,6S,8S,1S");
            Assert.Single(s.record.ChipSources, x => x == s.jok);
            Assert.Equal(30, s.record.ChipsFromEmits - 38);
        }

        [Fact]
        public void AddRemovePareidolia_UpdatesFaceRankGroup()
        {
            ResetToFirstBlindPlayRound();
            Assert.False(EngineUtils.isFace(CardFactory.PlayingCardFromRankSuit(Rank.TWO, Suit.SPADES)));

            AddJoker("PAREIDOLIA");
            Assert.True(EngineUtils.isFace(CardFactory.PlayingCardFromRankSuit(Rank.TWO, Suit.SPADES)));

            ZoneManager.JokerZone.RemoveCard(ZoneManager.JokerZone.Cards[0]);
            Assert.False(EngineUtils.isFace(CardFactory.PlayingCardFromRankSuit(Rank.TWO, Suit.SPADES)));
        }

        [Fact]
        public void CloseRound_WithGrosMichel_AddsMultAndCanDestroyAtRoundEnd()
        {
            var s = JokerSetup("GROS MICHEL");
            RigNextRoll(false);
            PlayHand("AS,AS,AS,AS,AS");
            Assert.Single(s.record.MultSources);
            Assert.Equal(15, s.record.MultFromEmits);
            Assert.Contains(s.jok, ZoneManager.JokerZone.Cards);

            ResetToFirstBlindPlayRound();
            AddJoker("GROS MICHEL");
            RigNextRoll(true);
            PlayHand("AS,AS,AS,AS,AS");
            Assert.DoesNotContain(ZoneManager.JokerZone.Cards, x => x.JokerData.DBName == "GROS MICHEL");
        }

        [Fact]
        public void PlayHand_WithEvenSteven_AddsMultForEvenRanks()
        {
            var s = JokerSetup("EVEN STEVEN");
            PlayHand("2S,4S,6S,8S,1S");

            Assert.Equal(5, s.record.MultSources.Count(x => x == s.jok));
            Assert.Equal(20, s.record.MultFromEmits);
        }

        [Fact]
        public void PlayHand_WithOddTodd_AddsChipsForOddRanks()
        {
            var s = JokerSetup("ODD TODD");
            PlayHand("AS,9S,7S,5S,3S");

            Assert.Equal(5, s.record.ChipSources.Count(x => x == s.jok));
            Assert.Equal(155, s.record.ChipsFromEmits - 35);
        }

        [Fact]
        public void PlayHand_WithScholar_AddsChipsAndMultForAces()
        {
            var s = JokerSetup("SCHOLAR");
            PlayHand("AS,9S,7S,5S,3S");

            Assert.Equal(1, s.record.ChipSources.Count(x => x == s.jok));
            Assert.Single(s.record.MultSources);
            Assert.Equal(20, s.record.ChipsFromEmits - 35);
            Assert.Equal(4, s.record.MultFromEmits);
        }

        [Fact]
        public void PlayHand_WithBusinessCard_GivesMoneyOnFaceCardWhenRollSucceeds()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("BUSINESS CARD");
            RigNextRoll(true);
            var beforeMoney = Globals.Money;

            PlayHand("JS");

            Assert.Equal(beforeMoney + 2, Globals.Money);
        }

        [Fact]
        public void PlayHand_WithSupernova_AddsPlayedHandCountToMult()
        {
            var s = JokerSetup("SUPERNOVA");

            PlayHand("AS,AS");
            Assert.Single(s.record.MultSources);
            Assert.Equal(s.jok, s.record.MultSources[0]);
            Assert.Equal(1, s.record.MultFromEmits);
        }

        [Fact]
        public void PlayHand_WithRideTheBus_ResetsOnScoringFaceCard()
        {
            var s = JokerSetup("RIDE THE BUS");

            PlayHand("2S");
            Assert.Equal(1, s.jok.JokerData.DataDict["MULTAMOUNT"].DoubleData);

            s.record.Reset();
            PlayHand("3S");
            Assert.Single(s.record.MultSources);
            Assert.Equal(1, s.record.MultFromEmits);
            Assert.Equal(2, s.jok.JokerData.DataDict["MULTAMOUNT"].DoubleData);

            s.record.Reset();
            PlayHand("KS");
            Assert.Single(s.record.MultSources);
            Assert.Equal(2, s.record.MultFromEmits);
            Assert.Equal(0, s.jok.JokerData.DataDict["MULTAMOUNT"].DoubleData);
        }

        [Fact]
        public void PlayHand_WithSpaceJoker_UpgradesPlayedHandLevelWhenRollSucceeds()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("SPACE JOKER");
            RigNextRoll(true);
            var beforeLevel = ScoreHandler.HandLevels[PlayedHandType.HIGHCARD];

            PlayHand("AS");

            Assert.Equal(beforeLevel + 1, ScoreHandler.HandLevels[PlayedHandType.HIGHCARD]);
        }

        [Fact]
        public void CloseRound_WithEgg_IncreasesBonusSellValue()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("EGG");
            var egg = ZoneManager.JokerZone.Cards.Single();

            Globals.RequiredChipsForCurrentBlind = 1;
            PlayHand("AS");

            Assert.Equal(3, egg.BonusSellValue);
            Assert.Equal(5, egg.SellCost);
        }

        [Fact]
        public void CloseBlindSelection_WithBurglar_AddsHandsAndRemovesDiscards()
        {
            ResetToBlindSelection();
            AddJoker("BURGLAR");

            FlowHandler.StartSelectedBlind();

            Assert.Equal(Globals.MaxHandsPerRound + 3, Globals.CurHandsRemaining);
            Assert.Equal(0, Globals.CurDiscardsRemaining);
        }

        [Fact]
        public void PlayHand_WithBlackboard_GivesMultMultOnlyForBlackHeldCards()
        {
            var s = JokerSetup("BLACKBOARD");
            BuildKnownHand("AS,2S,3C", selectAll: false);
            ZoneManager.HandZone.Cards[0].isSelected = true;
            Globals.PlayCurrentlySelectedHand();
            Assert.Single(s.record.MultMultSources);
            Assert.Equal(3, s.record.MultMultFromEmits);

            s.record.Reset();
            BuildKnownHand("AS,2H", selectAll: false);
            ZoneManager.HandZone.Cards[0].isSelected = true;
            Globals.PlayCurrentlySelectedHand();
            Assert.Empty(s.record.MultMultSources);
        }

        [Fact]
        public void PlayHand_WithRunner_GainsAndAppliesChipsBeforeScoringOnStraights()
        {
            var s = JokerSetup("RUNNER");
            PlayHand("2S,3D,4H,5C,6S");
            Assert.Equal(15, s.jok.JokerData.DataDict["CHIPAMOUNT"].IntData);
            Assert.Contains(s.jok, s.record.ChipSources);
            Assert.Equal(20 + 15, s.record.ChipsFromEmits);//base from cards in hand + runner chips
        }

        [Fact]
        public void PlayHands_WithIceCream_LosesChipsAndDestroysAtZero()
        {
            var s = JokerSetup("ICE CREAM");
            PlayHand("AS");
            Assert.Contains(s.jok, s.record.ChipSources);
            Assert.Equal(95, s.jok.JokerData.DataDict["CHIPAMOUNT"].IntData);

            s.jok.JokerData.DataDict["CHIPAMOUNT"].IntData = 5;
            PlayHand("AS");
            Assert.DoesNotContain(s.jok, ZoneManager.JokerZone.Cards);
        }

        [Fact]
        public void PlayHand_WithDNA_FirstSingleCardHandCopiesCardToDeckAndHand()
        {
            var s = JokerSetup("DNA");
            var beforeDeck = ZoneManager.DeckZone.Cards.Count;
            PlayHand("AS");

            Assert.Equal(beforeDeck - 8 + 1, ZoneManager.DeckZone.Cards.Count);
            Assert.Contains(ZoneManager.HandZone.Cards, x => x.Rank == Rank.ACE && x.Suit == Suit.SPADES);
        }

        [Fact]
        public void PlayHand_WithSplash_AllSelectedCardsTrigger()
        {
            JokerSetup("SPLASH");
            var record = CaptureScoringContributions();
            BuildKnownHand("3S,3D,3C,6C,8C");
            var played = ZoneManager.CardsSelectedInHand.ToList();
            Globals.PlayCurrentlySelectedHand();

            Assert.All(played, c => Assert.Contains(c, record.ChipSources));
        }

        [Fact]
        public void PlayHand_WithBlueJoker_AddsChipsBasedOnRemainingDeck()
        {
            var s = JokerSetup("BLUE JOKER");
            var expectedAmt = (ZoneManager.DeckZone.Cards.Count * 2) + 11;//Set this before the play because redraw happens, changing amt of cards in deck.
            PlayHand("AS");
            Assert.Contains(s.jok, s.record.ChipSources);
            Assert.Equal(expectedAmt, s.record.ChipsFromEmits);
        }

        [Fact]
        public void PlayHand_WithSixthSense_FirstSingleSixCreatesSpectralAndDestroysCard()
        {
            var s = JokerSetup("SIXTH SENSE");
            var beforeCons = ZoneManager.ConsumableZone.Cards.Count;
            PlayHand("6S");

            Assert.Equal(beforeCons + 1, ZoneManager.ConsumableZone.Cards.Count);
            Assert.DoesNotContain(ZoneManager.HiddenPlayZone.Cards, x => x.Rank == Rank.SIX && x.Suit == Suit.SPADES);
        }

        [Fact]
        public void UsePlanet_WithConstellation_IncreasesAndAppliesMultMult()
        {
            var s = JokerSetup("CONSTELLATION");
            var planet = ConsumableManager.MakePlanetCard(PlayedHandType.HIGHCARD);
            ZoneManager.ConsumableZone.AddCard(planet);
            ConsumableManager.UseConsumable(planet);

            Assert.Equal(1.1, s.jok.JokerData.DataDict["MULTMULTAMOUNT"].DoubleData, 6);
            PlayHand("AS");
            Assert.Contains(s.jok, s.record.MultMultSources);
            Assert.Equal(1.1, s.record.MultMultFromEmits, 6);
        }

        [Fact]
        public void PlayHand_WithHiker_CardsIncreaseBonusChips()
        {
            var s = JokerSetup("HIKER");
            PlayHand("AS,2S,3S,4S,5S");
            Assert.Equal(5, s.record.ChipSources.Count());
            Assert.Equal(25 + 25, s.record.ChipsFromEmits);
            Assert.Equal(16, s.record.ChipSources[0].ChipsBase); //base 11 + 5 from hiker.
        }

        [Fact]
        public void DiscardHand_WithFaceless_GivesMoneyCorrectly()
        {
            var s = JokerSetup("FACELESS JOKER");
            var oldMoney = Globals.Money;
            DiscardHand("AS,JS,JD,KC,2S");
            Assert.Equal(oldMoney + 5, Globals.Money);
            DiscardHand("AS,3S,JD,KC,2S");
            Assert.Equal(oldMoney + 5, Globals.Money);
        }

        [Fact]
        public void PlayAndDiscard_WithGreenJoker_ChangesMultCorrectly()
        {
            var s = JokerSetup("GREEN JOKER");
            Globals.RequiredChipsForCurrentBlind = 99999;
            PlayHand("AS");
            Assert.Single(s.record.MultSources);
            Assert.Equal(1, s.record.MultFromEmits);
            s.record.Reset();
            PlayHand("AS");
            Assert.Single(s.record.MultSources);
            Assert.Equal(2, s.record.MultFromEmits);
            DiscardHand("AS");
            s.record.Reset();
            PlayHand("AS");
            Assert.Single(s.record.MultSources);
            Assert.Equal(2, s.record.MultFromEmits);
        }

        [Fact]
        public void PlayHand_WithSuperposition_CorrectlyGeneratesTarot()
        {
            var s = JokerSetup("SUPERPOSITION");
            Globals.RequiredChipsForCurrentBlind = 999999;
            PlayHand("AS,2S,3S,4S,5S");
            Assert.Single(ZoneManager.ConsumableZone.Cards);
            Assert.True(ZoneManager.ConsumableZone.Cards.Last().isConsumable);
            PlayHand("2s,3S,4S,5S,6S");
            Assert.Single(ZoneManager.ConsumableZone.Cards);
        }

        [Fact]
        public void PlayHand_WithToDoList_CorrectlyMakesMoney()
        {
            var s = JokerSetup("TO DO LIST");
            s.jok.JokerData.DataDict["HANDTYPE"].HandTypeData = PlayedHandType.PAIR;
            
            var beforeMoney = Globals.Money;
            PlayHand("AS");
            Assert.Equal(beforeMoney, Globals.Money);
            PlayHand("AS,AS");
            Assert.Equal(beforeMoney + 4, Globals.Money);
            PlayHand("AS,AS,AS,AS,AS");
            Assert.Equal(beforeMoney + 4, Globals.Money);
            Assert.NotEqual(PlayedHandType.PAIR, s.jok.JokerData.DataDict["HANDTYPE"].HandTypeData);
        }

        [Fact]
        public void PlayHand_WithCavendish_CorrectlyAddsMult()
        {
            var s = JokerSetup("CAVENDISH");
            RigNextRoll(true);
            PlayHand("AS,2S,3S,4S,5S");
            Assert.Single(s.record.MultMultSources);
            Assert.Equal(s.jok, s.record.MultMultSources[0]);
            Assert.Equal(3, s.record.MultMultFromEmits);
            Assert.Empty(ZoneManager.JokerZone.Cards);
        }

        [Fact]
        public void PlayHand_WithCardSharp_GivesMultMultOnRepeatedHandType()
        {
            var s = JokerSetup("CARD SHARP");
            PlayHand("AS");
            Assert.Empty(s.record.MultMultSources);
            PlayHand("KS");
            Assert.Contains(s.jok, s.record.MultMultSources);
        }

        [Fact]
        public void SkipPack_WithRedCard_GainsMult()
        {
            var s = JokerSetup("RED CARD");
            Assert.Equal(0, s.jok.JokerData.DataDict["MULTAMOUNT"].DoubleData);
            PackActions.SkipCurrentPack();
            Assert.Equal(3, s.jok.JokerData.DataDict["MULTAMOUNT"].DoubleData);
            PlayHand("AS");
            Assert.Contains(s.jok, s.record.MultSources);
            Assert.Equal(3, s.record.MultFromEmits);
        }

        [Fact]
        public void StartBlind_WithMadness_GainsMultMultAndDestroysAnotherJoker()
        {
            ResetToBlindSelection();
            AddJoker("MADNESS");
            AddJoker("JIMBO");
            FlowHandler.StartSelectedBlind();
            Assert.Single(ZoneManager.JokerZone.Cards);
            Assert.Equal("MADNESS", ZoneManager.JokerZone.Cards[0].JokerData.DBName);
            Assert.Equal(1.5, ZoneManager.JokerZone.Cards[0].JokerData.DataDict["MULTMULTAMOUNT"].DoubleData, 6);
        }

        [Fact]
        public void PlayHand_WithSquareJoker_GainsAndAppliesChipsOnFourCardHand()
        {
            var s = JokerSetup("SQUARE JOKER");
            Globals.RequiredChipsForCurrentBlind = 99999;
            PlayHand("AS,2S,3S");
            Assert.Equal(0, s.jok.JokerData.DataDict["CHIPAMOUNT"].IntData);
            Assert.DoesNotContain(s.jok, s.record.ChipSources);

            PlayHand("AS,2S,3S,4S");
            Assert.Equal(4, s.jok.JokerData.DataDict["CHIPAMOUNT"].IntData);
            Assert.Contains(s.jok, s.record.ChipSources);
            PlayHand("AS,2S,3S");
            Assert.Equal(4, s.jok.JokerData.DataDict["CHIPAMOUNT"].IntData);
        }

        [Fact]
        public void PlayHand_WithSeance_CreatesSpectralOnStraightFlush()
        {
            JokerSetup("SEANCE");
            var before = ZoneManager.ConsumableZone.Cards.Count;
            PlayHand("AS,2S,3S,4S,5S");
            Assert.Equal(before + 1, ZoneManager.ConsumableZone.Cards.Count);
        }

        [Fact]
        public void StartBlind_WithRiffRaff_CreatesTwoCommonJokers()
        {
            ResetToBlindSelection();
            AddJoker("RIFF-RAFF");
            FlowHandler.StartSelectedBlind();
            Assert.Equal(3, ZoneManager.JokerZone.Cards.Count);
            Assert.All(ZoneManager.JokerZone.Cards.Where(x => x.JokerData.DBName != "RIFF-RAFF"), x => Assert.Equal(JokerRarity.COMMON, x.JokerData.Rarity));
        }

        [Fact]
        public void PlayHand_WithVampire_ConsumesEnhancementAndGainsMultMult()
        {
            var s = JokerSetup("VAMPIRE");
            var card = BuildKnownHand("AS")[0];
            card.SetEnhancementOfficial(Enhancement.MULT);
            Globals.PlayCurrentlySelectedHand();
            Assert.Equal(Enhancement.NONE, card.Enhancement);
            Assert.Empty(s.record.MultSources);
            Assert.Single(s.record.MultMultSources);
            Assert.Equal(s.jok, s.record.MultMultSources[0]);
            Assert.Equal(1.1, s.jok.JokerData.DataDict["MULTMULTAMOUNT"].DoubleData, 6);
        }

        [Fact]
        public void PlayHand_WithShortcut_UpdatesSkipStrengthAccordingly()
        {
            var s = JokerSetup("SHORTCUT");
            Globals.RequiredChipsForCurrentBlind = 999999;
            Assert.Equal(1, EngineUtils.SkipStrength);
            PlayHand("AS,2S,4S,6S,7S");
            Assert.Single(s.record.PlayedHandTypes);
            Assert.Equal(PlayedHandType.STRAIGHTFLUSH, s.record.PlayedHandTypes[0]);
            PlayHand("8S,1D,JS,QS,KS");
            Assert.Equal(PlayedHandType.STRAIGHT, s.record.PlayedHandTypes[1]);
            ZoneManager.JokerZone.RemoveCard(ZoneManager.JokerZone.Cards[0]);
            Assert.Equal(0, EngineUtils.SkipStrength);
            s.record.Reset();
            PlayHand("8S,1D,JS,QS,KS");
            Assert.Equal(PlayedHandType.HIGHCARD, s.record.PlayedHandTypes[0]);
        }

        [Fact]
        public void AddCardsToDeck_WithHologram_OnlyCountsPermanentAdds()
        {
            var s = JokerSetup("HOLOGRAM");
            var starting = s.jok.JokerData.DataDict["MULTMULTAMOUNT"].DoubleData;
            var created = CardFactory.PlayingCardFromRankSuit(Rank.TWO, Suit.CLUBS);
            ZoneManager.DeckZone.AddCard(created);
            Assert.Equal(starting + 0.25, s.jok.JokerData.DataDict["MULTMULTAMOUNT"].DoubleData, 6);
            ZoneManager.DeckZone.DrawUntilCapacityFrom(ZoneManager.HandZone);
            Assert.Equal(starting + 0.25, s.jok.JokerData.DataDict["MULTMULTAMOUNT"].DoubleData, 6);
            ZoneManager.DrawHandful();
            Assert.Equal(starting + 0.25, s.jok.JokerData.DataDict["MULTMULTAMOUNT"].DoubleData, 6);
            AddSpectral("Grim");
            UseCon();
            Assert.Equal(starting + 0.75, s.jok.JokerData.DataDict["MULTMULTAMOUNT"].DoubleData, 6);
            ZoneManager.HandZone.ClearCards();//we clear cards because Grim could generate Steel cards, which would add MultMult sources.
            ZoneManager.DrawHandful();
            ZoneManager.HandZone.Cards[0].ToggleSelect();
            Globals.PlayCurrentlySelectedHand();//This instead of PlayHand() function cause that actually CREATES new cards, triggering/increasing Holograms mult.
            Assert.Single(s.record.MultMultSources);
            Assert.Equal(s.jok, s.record.MultMultSources[0]);
            Assert.Equal(starting + 0.75, s.record.MultMultFromEmits);
        }

        [Fact]
        public void PlayHand_WithVagabond_CreatesTarotAtFourOrLessMoney()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("VAGABOND");
            Globals.Money = 4;
            var before = ZoneManager.ConsumableZone.Cards.Count;
            PlayHand("AS");
            Assert.Equal(before + 1, ZoneManager.ConsumableZone.Cards.Count);
            Assert.Equal(ConsumableType.TAROT, ZoneManager.ConsumableZone.Cards.First().ConsumableData.Type);
            Globals.Money = 5;
            PlayHand("AS");
            Assert.Equal(before + 1, ZoneManager.ConsumableZone.Cards.Count);
        }

        [Fact]
        public void PlayHand_WithBaron_GivesMultMultPerHeldKing()
        {
            var s = JokerSetup("BARON");
            BuildKnownHand("AS,KS,KH", selectAll: false);
            ZoneManager.HandZone.Cards[0].isSelected = true;
            Globals.PlayCurrentlySelectedHand();
            Assert.Equal(2, s.record.MultMultSources.Count(x => x.Rank == Rank.KING));
            Assert.Equal(1.5 * 1.5, s.record.MultMultFromEmits, 6);
        }

        [Fact]
        public void CloseRound_WithCloud9_GivesMoneyPerNineInDeck()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("CLOUD 9");
            ZoneManager.DeckZone.Cards[0].Rank = Rank.NINE;
            ZoneManager.HandZone.Cards[0].Rank = Rank.NINE;
            var beforeMoney = Globals.Money;
            Globals.RequiredChipsForCurrentBlind = 1;
            PlayHand("AS");
            var x = Globals.CurrentGameStateObj.PostRoundMoneySources;
            FlowHandler.ClosePostRound();
            Assert.Equal(beforeMoney + ZoneManager.GetFullDeckCards().Count(x => x.Rank == Rank.NINE) + FlowHandler.PostRoundFreeMoney[BlindType.SMALL] + 3, Globals.Money);
        }

        [Fact]
        public void BlindChange_WithRocket_IncreasesPayoutAfterBoss()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("ROCKET");
            var rocket = ZoneManager.JokerZone.Cards.Single();
            FlowHandler.CurrentSelectedBlind = BlindType.BOSS;
            FlowHandler.StartSelectedBlind();
            Assert.Equal(1, rocket.JokerData.DataDict["MONEYAMOUNT"].IntData);
            PlayHand("AS,AS,AS,AS,AS");
            Assert.Equal(3, rocket.JokerData.DataDict["MONEYAMOUNT"].IntData);
            Assert.Equal("Rocket", Globals.CurrentGameStateObj.PostRoundMoneySources.Last().Item1);
            FlowHandler.ClosePostRound();
        }

        [Fact]
        public void PlayHand_WithObelisk_ResetsOnMostPlayedHand()
        {
            var s = JokerSetup("OBELISK");
            PlayHand("AS,AS");
            s.record.Reset();
            PlayHand("AS");
            Assert.Single(s.record.MultMultSources);
            Assert.Equal(1.2, s.record.MultMultFromEmits, 6);
            s.record.Reset();
            PlayHand("KS");
            Assert.Empty(s.record.MultMultSources);
            Assert.Equal(1, s.jok.JokerData.DataDict["MULTMULTAMOUNT"].DoubleData, 6);
        }

        [Fact]
        public void PlayHand_WithMidasMask_MakesPlayedFaceCardsGold()
        {
            JokerSetup("MIDAS MASK");
            PlayHand("KS,QS,AS");
            Assert.All(ZoneManager.HiddenPlayZone.Cards.Where(EngineUtils.isFace), x => Assert.Equal(Enhancement.GOLD, x.Enhancement));
        }

        [Fact]
        public void SellLuchador_DuringBossPlay_DisablesBossBlind()
        {
            ResetToFirstBlindPlayRound();
            FlowHandler.CurrentSelectedBlind = BlindType.BOSS;
            FlowHandler.StartSelectedBlind();
            Assert.NotEmpty(ZoneManager.HiddenBlindAttributeZone.Cards);
            AddJoker("LUCHADOR");
            var luchador = ZoneManager.JokerZone.Cards.Single();
            Globals.PerformSell(luchador, ZoneManager.JokerZone);
            Assert.Empty(ZoneManager.HiddenBlindAttributeZone.Cards);
        }

        [Fact]
        public void PlayHand_WithPhotograph_TriggersOnFirstPlayedFaceCardOnly()
        {
            var s = JokerSetup("PHOTOGRAPH");
            Globals.RequiredChipsForCurrentBlind = 999999;
            PlayHand("KS,QS,AS,1S,5S");
            Assert.Single(s.record.MultMultSources);
            Assert.Equal(Rank.KING, s.record.MultMultSources[0].Rank);
            Assert.Equal(2, s.record.MultMultFromEmits, 6);
            s.record.Reset();
            PlayHand("JS,KS,KD");
            Assert.Equal(Rank.KING, s.record.MultMultSources[0].Rank);
            Assert.Equal(2, s.record.MultMultFromEmits, 6);
        }

        [Fact]
        public void CloseRound_WithGiftCard_IncreasesSellValueOfJokersAndConsumables()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("GIFT CARD");
            AddJoker("JIMBO");
            var tarot = ConsumableManager.MakeTarotCard("FOOL");
            ZoneManager.ConsumableZone.AddCard(tarot);
            var jimbo = ZoneManager.JokerZone.Cards.Single(x => x.JokerData.DBName == "JIMBO");
            var oldJimbo = jimbo.SellCost;
            var oldTarot = tarot.SellCost;
            Globals.RequiredChipsForCurrentBlind = 1;
            PlayHand("AS");
            Assert.Equal(1, jimbo.BonusSellValue);
            Assert.Equal(1, tarot.BonusSellValue);
            Assert.Equal(1 + oldJimbo, jimbo.SellCost);
            Assert.Equal(1 + oldTarot, tarot.SellCost);
        }

        [Fact]
        public void TurtleBean_AddsAndThenLosesHandSize_AndSelfDestructsAtZero()
        {
            ResetToFirstBlindPlayRound();
            var baseSize = Globals.HandSize;
            AddJoker("TURTLE BEAN");
            var bean = ZoneManager.JokerZone.Cards.Single();
            Assert.Equal(baseSize + 5, Globals.HandSize);
            bean.JokerData.DataDict["HANDSIZEAMOUNT"].IntData = 1;
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.EndPlayRound } });
            Assert.DoesNotContain(bean, ZoneManager.JokerZone.Cards);
        }

        [Fact]
        public void PlayHand_WithErosion_AddsMultBasedOnMissingDeckCards()
        {
            var s = JokerSetup("EROSION");
            ZoneManager.DestroyCard(ZoneManager.DeckZone.Cards[0], ZoneManager.DeckZone);
            ZoneManager.DestroyCard(ZoneManager.DeckZone.Cards[0], ZoneManager.DeckZone);
            ZoneManager.HandZone.Cards[0].ToggleSelect();
            Globals.PlayCurrentlySelectedHand();
            Assert.Single(s.record.MultSources);
            Assert.Equal(8, s.record.MultFromEmits);
        }

        [Fact]
        public void PlayHand_WithReservedParking_CanGainMoneyFromHeldFaceCards()
        {
            var s = JokerSetup("RESERVED PARKING");
            RigNextRoll(true);
            RigNextRoll(true);
            BuildKnownHand("KS,QS,2D", selectAll: false);
            ZoneManager.HandZone.Cards[2].isSelected = true;
            var before = Globals.Money;
            Globals.PlayCurrentlySelectedHand();
            Assert.Equal(before + 2, Globals.Money);
        }

        [Fact]
        public void Discard_WithMailInRebate_GivesMoneyPerTargetRank()
        {
            var s = JokerSetup("MAIL-IN REBATE");
            s.jok.JokerData.DataDict["TARGETRANK"].SpecificCardRank = Rank.ACE;
            var before = Globals.Money;
            DiscardHand("AS,AH,2D");
            Assert.Equal(before + 10, Globals.Money);
        }

        [Fact]
        public void CloseRound_WithToTheMoon_AddsUncappedInterest()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("TO THE MOON");
            Globals.Money = 100;
            Globals.RequiredChipsForCurrentBlind = 1;
            PlayHand("AS");
            Assert.Contains(Globals.CurrentGameStateObj.PostRoundMoneySources, x => x.Item1 == "To the Moon" && x.Item2 == 20);
        }

        [Fact]
        public void OpenPack_WithHallucination_CreatesTarotWhenRollSucceeds()
        {
            ResetToFirstBlindPlayRound();
            AddJoker("HALLUCINATION");
            RigNextRoll(true);
            var before = ZoneManager.ConsumableZone.Cards.Count;
            var pack = ConsumableManager.MakePack(PackType.BASIC_TAROT);
            PackActions.OpenPack(pack);
            Assert.Equal(before + 1, ZoneManager.ConsumableZone.Cards.Count);
            Assert.Equal(ConsumableType.TAROT, ZoneManager.ConsumableZone.Cards.Last().ConsumableData.Type);
        }

        [Fact]
        public void PlayHand_WithFortuneTeller_UsesTotalTarotsUsedThisRun()
        {
            ResetToFirstBlindPlayRound();
            ZoneManager.HandZone.Cards[0].ToggleSelect();
            AddTarot("The Magician");
            UseCon();
            AddTarot("The Devil");
            UseCon();
            var record = CaptureScoringContributions();
            AddJoker("FORTUNE TELLER");
            var jok = ZoneManager.JokerZone.Cards.Last();
            PlayHand("AS");
            Assert.Single(record.MultSources);
            Assert.Equal(jok, record.MultSources[0]);
            Assert.Equal(2, record.MultFromEmits);
        }

        [Fact]
        public void AddRemoveJuggler_UpdatesHandSize()
        {
            ResetToFirstBlindPlayRound();
            var baseSize = Globals.HandSize;
            AddJoker("JUGGLER");
            var jug = ZoneManager.JokerZone.Cards.Single();
            Assert.Equal(baseSize + 1, Globals.HandSize);
            ZoneManager.JokerZone.RemoveCard(jug);
            Assert.Equal(baseSize, Globals.HandSize);
        }

        [Fact]
        public void AddRemoveDrunkard_UpdatesMaxDiscardsPerRound()
        {
            ResetToFirstBlindPlayRound();
            var baseDiscards = Globals.MaxDiscardsPerRound;
            AddJoker("DRUNKARD");
            var drunk = ZoneManager.JokerZone.Cards.Single();
            Assert.Equal(baseDiscards + 1, Globals.MaxDiscardsPerRound);
            ZoneManager.JokerZone.RemoveCard(drunk);
            Assert.Equal(baseDiscards, Globals.MaxDiscardsPerRound);
        }

        [Fact]
        public void PlayHand_WithStoneJoker_AddsChipsPerStoneCardInDeck()
        {
            var s = JokerSetup("STONE JOKER");
            ZoneManager.DeckZone.Cards[0].SetEnhancementOfficial(Enhancement.STONE);
            ZoneManager.DeckZone.Cards[1].SetEnhancementOfficial(Enhancement.STONE);
            PlayHand("AS");
            Assert.Contains(s.jok, s.record.ChipSources);
            Assert.True(s.record.ChipsFromEmits >= 50);
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
