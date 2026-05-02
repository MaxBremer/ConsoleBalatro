using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleBalatro.Tests
{
    public class TarotTests : TestClassBase
    {
        [Fact]
        public void ActivateConsumable_TheEmperor_ShouldPopulateConsumableZone()
        {
            ResetToBlindSelection();

            AddTarot("The Emperor");
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Equal(2, ZoneManager.ConsumableZone.Cards.Count);
            Assert.NotEqual("The Emperor", ZoneManager.ConsumableZone.Cards[0].ConsumableData.ConsumableName);
            Assert.NotEqual("The Emperor", ZoneManager.ConsumableZone.Cards[1].ConsumableData.ConsumableName);
        }

        [Fact]
        public void ActivateConsumable_HighPriestess_ShouldPopulateConsumableZone()
        {
            ResetToBlindSelection();

            AddTarot("High Priestess");
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Equal(2, ZoneManager.ConsumableZone.Cards.Count);
            Assert.Equal(ConsumableType.PLANET, ZoneManager.ConsumableZone.Cards[0].ConsumableData.Type);
            Assert.Equal(ConsumableType.PLANET, ZoneManager.ConsumableZone.Cards[1].ConsumableData.Type);
            Assert.NotEqual(ZoneManager.ConsumableZone.Cards[0].ConsumableData.PlanetHandType, ZoneManager.ConsumableZone.Cards[1].ConsumableData.PlanetHandType);
        }

        [Fact]
        public void ActivateConsumable_TheFool_ShouldRecrateLastConsumable()
        {
            ResetToBlindSelection();
            var prevPoolCount = MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.TAROT_CARD].Cards.Count;
            AddTarot("The Emperor");
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Globals.PerformSell(ZoneManager.ConsumableZone.Cards[0], ZoneManager.ConsumableZone);
            Globals.PerformSell(ZoneManager.ConsumableZone.Cards[0], ZoneManager.ConsumableZone);
            //ALL TAROTS SHOULD BE RETURNED TO POOL
            Assert.Equal(prevPoolCount, MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.TAROT_CARD].Cards.Count);
            AddTarot("The Fool");
            consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);

            Assert.Single(ZoneManager.ConsumableZone.Cards);
            Assert.Equal("The Emperor", ZoneManager.ConsumableZone.Cards[0].ConsumableData.ConsumableName);
        }

        [Fact]
        public void ActivateConsumable_TheHermit_ShouldDoubleMoney()
        {
            ResetToBlindSelection();

            AddTarot("The Hermit");

            Globals.Money = 15;
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Equal(30, Globals.Money);
        }

        [Fact]
        public void ActivateConsumable_TheHermit_MaxGainIs20()
        {
            ResetToBlindSelection();

            AddTarot("The Hermit");

            Globals.Money = 25;
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Equal(45, Globals.Money);
        }

        [Theory]
        [InlineData("The Devil", Enhancement.GOLD)]
        [InlineData("The Tower", Enhancement.STONE)]
        [InlineData("The Chariot", Enhancement.STEEL)]
        [InlineData("Justice", Enhancement.GLASS)]
        [InlineData("The Lovers", Enhancement.WILD)]
        [InlineData("The Magician", Enhancement.LUCKY, true)]
        [InlineData("The Empress", Enhancement.MULT, true)]
        [InlineData("The Hierophant", Enhancement.BONUSCHIPS, true)]
        public void ActivateConsumable_SingleTargetConsumables_ConvertsSuccessfully(string conName, Enhancement targetEnhance, bool twoTargets = false)
        {
            ResetToFirstBlindPlayRound();
            AddTarot(conName);
            if (twoTargets)
                BuildKnownHand("AS,2S");
            else
                BuildKnownHand("AS");
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(targetEnhance, ZoneManager.HandZone.Cards[0].Enhancement);
            if (twoTargets)
            {
                Assert.Equal(targetEnhance, ZoneManager.HandZone.Cards[1].Enhancement);
            }
        }

        [Fact]
        public void ActivateConsumable_TheHangedMan_ShouldDestroyCards()
        {
            ResetToFirstBlindPlayRound();
            AddTarot("The Hanged Man");
            BuildKnownHand("AS,2S,3S");
            ZoneManager.HandZone.Cards[2].isSelected = false;
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Single(ZoneManager.HandZone.Cards);
            Assert.Equal(Rank.THREE, ZoneManager.HandZone.Cards[0].Rank);
        }

        [Fact]
        public void ActivateConsumable_Strength_ShouldIncreaseRank()
        {
            ResetToFirstBlindPlayRound();
            AddTarot("Strength");
            BuildKnownHand("AS,2S");
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(2, ZoneManager.HandZone.Cards.Count);
            Assert.Equal(Rank.TWO, ZoneManager.HandZone.Cards[0].Rank);
            Assert.Equal(Rank.THREE, ZoneManager.HandZone.Cards[1].Rank);
        }

        [Fact]
        public void AcitvateConsumable_WheelOfFortune_ShouldGiveJokerEdition()
        {
            ResetToFirstBlindPlayRound();
            AddTarot("Wheel of Fortune");
            AddJoker("JIMBO");
            var joker = ZoneManager.JokerZone.Cards[0];
            Assert.Equal(Edition.BASE, joker.Edition);
            RigNextRoll(true);
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.NotEqual(Edition.BASE, joker.Edition);
        }

        [Theory]
        [InlineData("The Stars", Suit.DIAMONDS)]
        [InlineData("The Sun", Suit.HEARTS)]
        [InlineData("The Moon", Suit.CLUBS)]
        [InlineData("The World", Suit.SPADES)]
        public void ActivateConsumable_SuitConsumables_ConvertSuitsCorrectly(string conName, Suit targetSuit)
        {
            ResetToFirstBlindPlayRound();
            AddTarot(conName);
            BuildKnownHand("AS,2S,3S");
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(targetSuit, ZoneManager.HandZone.Cards[0].Suit);
            Assert.Equal(targetSuit, ZoneManager.HandZone.Cards[1].Suit);
            Assert.Equal(targetSuit, ZoneManager.HandZone.Cards[2].Suit);
        }

        [Fact]
        public void ActivateConsumable_Death_ShouldConvertCard()
        {
            ResetToFirstBlindPlayRound();
            AddTarot("Death");
            BuildKnownHand("AS,2S");
            ZoneManager.HandZone.Cards[1].SetEditionOfficial(Edition.HOLOGRAPHIC);
            ZoneManager.HandZone.Cards[1].SetEnhancementOfficial(Enhancement.STEEL);

            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            
            var cardCheck = ZoneManager.HandZone.Cards[0];
            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(2, ZoneManager.HandZone.Cards.Count);
            Assert.Equal(cardCheck.Rank, ZoneManager.HandZone.Cards[1].Rank);
            Assert.Equal(Rank.TWO, cardCheck.Rank);
            Assert.Equal(Edition.HOLOGRAPHIC, cardCheck.Edition);
            Assert.Equal(Enhancement.STEEL, cardCheck.Enhancement);
        }

        [Fact]
        public void ActivateConsumable_Temperance_ShouldGenerateMoney()
        {
            ResetToFirstBlindPlayRound();
            AddTarot("Temperance");
            Globals.Money = 0;
            AddJoker("JIMBO");
            var sellVal = ZoneManager.JokerZone.Cards[0].SellCost;
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Single(ZoneManager.JokerZone.Cards);
            Assert.Equal(sellVal, Globals.Money);
        }

        [Fact]
        public void ActivateConsumable_Judgement_ShouldGenerateRandomJoker()
        {
            ResetToFirstBlindPlayRound();
            AddTarot("Judgement");
            var consumable = ZoneManager.ConsumableZone.Cards[0];
            ConsumableManager.UseConsumable(consumable);
            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Single(ZoneManager.JokerZone.Cards);
            var joker = ZoneManager.JokerZone.Cards[0];
            Assert.Contains(joker.JokerData.DBName, JokerDb.JokerData.Keys);
        }
    }
}
