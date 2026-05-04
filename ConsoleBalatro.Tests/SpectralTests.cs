using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleBalatro.Tests
{
    public class SpectralTests : TestClassBase
    {
        [Fact]
        public void ActivateConsumable_Cryptid_CorrectlyCopiesCard()
        {
            ResetToFirstBlindPlayRound();

            AddSpectral("Cryptid");
            var oldCardCount = ZoneManager.HandZone.Cards.Count;
            var targetCard = ZoneManager.HandZone.Cards[0];
            targetCard.Edition = Edition.HOLOGRAPHIC;
            targetCard.Enhancement = Enhancement.GOLD;
            targetCard.Seal = Seal.RED;
            targetCard.ToggleSelect();
            ConsumableManager.UseConsumable(ZoneManager.ConsumableZone.Cards[0]);

            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(oldCardCount + 2, ZoneManager.HandZone.Cards.Count);
            var lastCard = ZoneManager.HandZone.Cards.Last();
            Assert.Equal(targetCard.Rank, lastCard.Rank);
            Assert.Equal(targetCard.Suit, lastCard.Suit);
            Assert.Equal(targetCard.Edition, lastCard.Edition);
            Assert.Equal(targetCard.Enhancement, lastCard.Enhancement);
            Assert.Equal(targetCard.Seal, lastCard.Seal);
        }
    }
}
