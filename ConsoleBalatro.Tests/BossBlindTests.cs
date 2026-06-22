using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleBalatro.Tests
{
    public class BossBlindTests : TestClassBase
    {
        [Fact]
        public void PlayBossBlind_TheNeedle_GetOnlyOneHand()
        {
            SetupBossBlind("THE NEEDLE");
            FlowHandler.StartSelectedBlind();
            Assert.Equal(1, Globals.CurHandsRemaining);
            Assert.True(600 > Globals.RequiredChipsForCurrentBlind);//Reduced chip requirement.
        }

        [Fact]
        public void PlayBossBlind_TheWater_GetNoDiscards()
        {
            SetupBossBlind("THE WATER");
            FlowHandler.StartSelectedBlind();
            Assert.Equal(0, Globals.CurDiscardsRemaining);
        }

        [Fact]
        public void PlayBossBlind_TheOx_MostCommonHandZeroesMoney()
        {
            SetupBossBlind("THE OX");
            FlowHandler.StartSelectedBlind();
            Globals.Money = 10;
            //Since no hands have been played yet, any hand is most common hand (or should be).
            FlowHandler.StartSelectedBlind();
            PlayHand("AS");
            Assert.Equal(0, Globals.Money);
            PlayHand("AS");
            Globals.Money = 5;
            //Now, most common hand is highcard, so a pair should not zero money.
            PlayHand("2S,2S");
            Assert.Equal(5, Globals.Money);
        }
        [Theory]
        [InlineData("THE WALL", 1200)]
        [InlineData("THE NEEDLE", 300)]
        [InlineData("VIOLET VESSEL", 1800)]
        public void PlayBoss_ReqChangeBoss_ChangesReqAsExpected(string bossDBName, int expectedChips)
        {
            SetupBossBlind(bossDBName);
            FlowHandler.StartSelectedBlind();
            Assert.Equal(expectedChips, Globals.RequiredChipsForCurrentBlind);
        }

        [Fact]
        public void PlayBoss_TheHook_DiscardsOnPlayAsExpected()
        {
            SetupBossBlind("THE HOOK");
            var rec = CaptureDiscards();

            FlowHandler.StartSelectedBlind();
            ZoneManager.HandZone.Cards[0].ToggleSelect();
            Globals.PlayCurrentlySelectedHand();
            Assert.Equal(2, rec.NumCardsDiscarded);
        }

        [Fact]
        public void PlayBoss_TheTooth_TakesMoneyAsExpected()
        {
            SetupBossBlind("THE TOOTH");

            Globals.Money = 10;
            FlowHandler.StartSelectedBlind();
            PlayHand("AS,AS,AS");
            Assert.Equal(7, Globals.Money);
        }

        [Fact]
        public void PlayBoss_TheManacle_TakesHandsizeAsExpected()
        {
            SetupBossBlind("THE MANACLE");

            var oldHandSize = Globals.HandSize;
            FlowHandler.StartSelectedBlind();
            Assert.Equal(oldHandSize - 1, Globals.HandSize);
        }

        [Fact]
        public void PlayBoss_TheWheel_FlipsCardFacedown()
        {
            SetupBossBlind("THE WHEEL");

            RigNextRoll(true);
            FlowHandler.StartSelectedBlind();
            Assert.True(ZoneManager.HandZone.Cards[0].FaceDown);
        }

        [Fact]
        public void PlayBoss_ThePlant_FaceCardsDebuffed()
        {
            SetupBossBlind("THE PLANT");

            FlowHandler.StartSelectedBlind();
            EnsureFaceCard();

            var targetCards = ZoneManager.HandZone.Cards.Where(c => EngineUtils.isFace(c));
            foreach (var card in targetCards)
            {
                Assert.True(card.Debuffed);
                Assert.True(card.DebuffedByBoss);
            }
        }

        [Theory]
        [InlineData("THE CLUB", Suit.CLUBS)]
        [InlineData("THE GOAD", Suit.SPADES)]
        [InlineData("THE WINDOW", Suit.DIAMONDS)]
        [InlineData("THE HEAD", Suit.HEARTS)]
        public void PlayBoss_SuitDebuffer_DebuffsProperSuits(string bossDbName, Suit beingDebuffed)
        {
            SetupBossBlind(bossDbName);

            FlowHandler.StartSelectedBlind();
            var targetCards = ZoneManager.HandZone.Cards.Where(c => c.IsSuit(beingDebuffed));
            foreach (var c in targetCards)
            {
                Assert.True(c.Debuffed);
                Assert.True(c.DebuffedByBoss);
            }
        }

        [Fact]
        public void PlayBoss_TheMark_FaceCardsFaceDown()
        {
            SetupBossBlind("THE MARK");

            FlowHandler.StartSelectedBlind();
            EnsureFaceCard();

            var targetCards = ZoneManager.HandZone.Cards.Where(c => EngineUtils.isFace(c));
            foreach (var card in targetCards)
            {
                Assert.True(card.FaceDown);
            }
        }

        [Fact]
        public void PlayBoss_TheHouse_FirstHandFaceDown()
        {
            SetupBossBlind("THE HOUSE");

            FlowHandler.StartSelectedBlind();
            Assert.True(ZoneManager.HandZone.Cards.All(c => c.FaceDown));

            ZoneManager.HandZone.Cards[0].ToggleSelect();
            Globals.DiscardSelectedFromHand();

            var upCard = ZoneManager.HandZone.Cards[^1];
            Assert.False(upCard.FaceDown);
            Assert.True(ZoneManager.HandZone.Cards.Where(x => x != upCard).All(c => c.FaceDown));
        }

        [Fact]
        public void PlayBoss_TheFish_DrawFacedownAfterHand()
        {
            SetupBossBlind("THE FISH");

            FlowHandler.StartSelectedBlind();
            Assert.DoesNotContain(ZoneManager.HandZone.Cards, c => c.FaceDown);

            ZoneManager.HandZone.Cards[0].ToggleSelect();
            Globals.PlayCurrentlySelectedHand();

            var upCard = ZoneManager.HandZone.Cards[^1];
            Assert.True(upCard.FaceDown);
            Assert.DoesNotContain(ZoneManager.HandZone.Cards.Where(x => x != upCard), c => c.FaceDown);

            ZoneManager.HandZone.Cards[0].ToggleSelect();
            Globals.DiscardSelectedFromHand();
            Assert.False(ZoneManager.HandZone.Cards[^1].FaceDown);
        }

        [Fact]
        public void PlayBoss_TheArm_LevelsDownPlayedHand()
        {
            SetupBossBlind("THE ARM");

            ScoreHandler.LevelUpHand(PlayedHandType.HIGHCARD);
            ScoreHandler.LevelUpHand(PlayedHandType.TWOPAIR);
            Assert.Equal(2, ScoreHandler.HandLevels[PlayedHandType.HIGHCARD]);
            Assert.Equal(2, ScoreHandler.HandLevels[PlayedHandType.TWOPAIR]);

            FlowHandler.StartSelectedBlind();
            PlayHand("AS");
            Assert.Equal(1, ScoreHandler.HandLevels[PlayedHandType.HIGHCARD]);
            Assert.Equal(2, ScoreHandler.HandLevels[PlayedHandType.TWOPAIR]);

            PlayHand("2S,2S,3S,3S");
            Assert.Equal(1, ScoreHandler.HandLevels[PlayedHandType.HIGHCARD]);
            Assert.Equal(1, ScoreHandler.HandLevels[PlayedHandType.TWOPAIR]);
        }

        [Fact]
        public void PlayBoss_ThePsychic_OnlyAllowsFiveCardPlays()
        {
            SetupBossBlind("THE PSYCHIC");
            var rec = CaptureScoringContributions();

            FlowHandler.StartSelectedBlind();
            PlayHand("AS");
            Assert.Equal(0, Globals.TotalCurrentChips);
            Assert.Empty(rec.ChipSources);
            PlayHand("AS,AS,AS,AS");
            Assert.Equal(0, Globals.TotalCurrentChips);
            Assert.Empty(rec.ChipSources);
            PlayHand("AS,5D,4D,8D,JD");
            Assert.Equal(16, Globals.TotalCurrentChips);
            var aSp = Assert.Single(rec.ChipSources);
            Assert.Equal(Rank.ACE, aSp.Rank);

        }

        [Fact]
        public void PlayBoss_TheMouth_AllowsOnlyOneHandType()
        {
            SetupBossBlind("THE MOUTH");
            var rec = CaptureScoringContributions();

            FlowHandler.StartSelectedBlind();
            PlayHand("AS");
            Assert.Equal(16, Globals.TotalCurrentChips);
            Assert.Single(rec.ChipSources);
            rec.Reset();
            PlayHand("AS,AS,AS,AS");
            Assert.Equal(16, Globals.TotalCurrentChips);
            Assert.Empty(rec.ChipSources);
            PlayHand("2S,3S");
            Assert.Equal(24, Globals.TotalCurrentChips);
            Assert.Single(rec.ChipSources);
        }

        [Fact]
        public void PlayBoss_TheFlint_HalvesBaseChipsMult()
        {
            SetupBossBlind("THE FLINT");

            FlowHandler.StartSelectedBlind();
            Globals.RequiredChipsForCurrentBlind = 9999999;
            PlayHand("AS");
            //int division rounds down I think? So 5 x 1 -> 2 x 0.5 since mult is a double, plus 11, 13 / 2 = 6.
            Assert.Equal(6, Globals.TotalCurrentChips);
            //flush five, 160 x 16 -> 80 x 8, plus 10 from 2s, 90 x 8 = 720.
            PlayHand("2S,2S,2S,2S,2S");
            Assert.Equal(720 + 6, Globals.TotalCurrentChips);
        }

        [Fact]
        public void PlayBoss_TheSerpent_RedrawsExactlyThree()
        {
            SetupBossBlind("THE SERPENT");

            FlowHandler.StartSelectedBlind();
            var oldHandCt = ZoneManager.HandZone.Cards.Count;
            ZoneManager.HandZone.Cards[0].ToggleSelect();
            Globals.DiscardSelectedFromHand();
            //redrawing 3 exactly, should be +2 total, where normally would be same number.
            Assert.Equal(oldHandCt + 2, ZoneManager.HandZone.Cards.Count);

            for (int i = 0; i < 5; i++)
            {
                ZoneManager.HandZone.Cards[i].ToggleSelect();
            }
            oldHandCt = ZoneManager.HandZone.Cards.Count;
            Globals.DiscardSelectedFromHand();
            //again, normally, same amount. However, should now be -2, as we went down 5 plus 3.
            Assert.Equal(oldHandCt - 2, ZoneManager.HandZone.Cards.Count);
        }

        [Fact]
        public void PlayBoss_TheEye_AllowsNoDuplicateHands()
        {
            SetupBossBlind("THE EYE");
            var rec = CaptureScoringContributions();

            FlowHandler.StartSelectedBlind();
            PlayHand("AS");
            Assert.Equal(16, Globals.TotalCurrentChips);
            Assert.Single(rec.ChipSources);
            rec.Reset();
            PlayHand("2S");
            Assert.Equal(16, Globals.TotalCurrentChips);
            Assert.Empty(rec.ChipSources);
            PlayHand("2S,2S");
            Assert.Equal(44, Globals.TotalCurrentChips);
            Assert.Equal(2, rec.ChipSources.Count);
        }

        private void EnsureFaceCard()
        {
            while (!ZoneManager.HandZone.Cards.Any(x => EngineUtils.isFace(x)))
            {
                ZoneManager.HandZone.DrawFrom(ZoneManager.DeckZone, ignoreSpaceLimits: true);
            }
        }

        private void SetupBossBlind(string bossDBName)
        {
            ResetEngineForTest();
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
            FlowHandler.CurrentBossBlind = bossDBName;
            FlowHandler.CurSmallBlindTag = Engine.Cards.Tags.TagType.HANDY;
            FlowHandler.CurBigBlindTag = Engine.Cards.Tags.TagType.HANDY;
            FlowHandler.DoSkip();
            FlowHandler.DoSkip();
        }
    }
}
