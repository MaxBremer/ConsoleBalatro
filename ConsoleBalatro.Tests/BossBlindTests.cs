using ConsoleBalatro.Engine;
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
