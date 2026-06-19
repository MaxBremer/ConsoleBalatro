using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleBalatro.Tests
{
    public class StickerTests : TestClassBase
    {
        [Fact]
        public void AddJoker_WithEternalSticker_CannotDestroyOrSell()
        {
            var s = JokerSetup("JIMBO");
            s.jok.AddSticker(Sticker.ETERNAL);
            AddJoker("CEREMONIAL DAGGER");
            ZoneManager.JokerZone.SwapCardPositions(0, 1);

            Assert.False(Globals.CanBeSold(s.jok));
            Assert.True(Globals.CanBeSold(GetJoker(0)));
            FlowHandler.StartSelectedBlind();
            Assert.Equal(2, ZoneManager.JokerZone.Cards.Count);
        }

        [Fact]
        public void AddJoker_WithRentalSticker_ReducesCostAndCharges()
        {
            var s = JokerSetup("PERKEO");
            s.jok.AddSticker(Sticker.RENTAL);
            Assert.Equal(1, s.jok.BuyCost);
            Assert.Equal(1, s.jok.SellCost);
            FlowHandler.StartSelectedBlind();
            Globals.Money = 10;
            PlayHand("AS,AS,AS,AS,AS");
            Assert.Equal(7, Globals.Money);
        }

        [Fact]
        public void AddJoker_WithPerishableSticker_DebuffsAfterFiveRounds()
        {
            var s = JokerSetup("JIMBO");
            s.jok.AddSticker(Sticker.PERISHABLE);
            FlowHandler.CurSmallBlindTag = TagType.HANDY;
            FlowHandler.CurBigBlindTag = TagType.HANDY;
            FlowHandler.CurrentBossBlind = "THE PSYCHIC";
            var expValue = 5;
            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(expValue, s.jok.PerishCountdownVal);
                FlowHandler.StartSelectedBlind();
                Globals.RequiredChipsForCurrentBlind = 1;
                PlayHand("AS,AS,AS,AS,AS");
                expValue--;
                FlowHandler.ClosePostRound();
                FlowHandler.CloseMarketRound();
            }
            Assert.Equal(expValue, s.jok.PerishCountdownVal);
            Assert.False(s.jok.Debuffed);
            FlowHandler.StartSelectedBlind();
            Globals.RequiredChipsForCurrentBlind = 1;
            PlayHand("AS");
            Assert.True(s.jok.Debuffed);
        }

        private Card GetJoker(int ind) => ZoneManager.JokerZone.Cards[ind];

        private (Card jok, ContributionCapture record) JokerSetup(string jokerName)
        {
            ResetToBlindSelection();
            var record = CaptureScoringContributions();
            Assert.Empty(ZoneManager.JokerZone.Cards);
            AddJoker(jokerName);
            Assert.Single(ZoneManager.JokerZone.Cards);
            var jok = GetJoker(0);
            return (jok, record);
        }
    }
}
