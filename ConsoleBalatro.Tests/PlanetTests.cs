using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleBalatro.Tests
{
    public class PlanetTests : TestClassBase
    {
        [Theory]
        [InlineData(PlayedHandType.HIGHCARD, "Pluto")]
        [InlineData(PlayedHandType.PAIR, "Mercury")]
        [InlineData(PlayedHandType.TWOPAIR, "Uranus")]
        [InlineData(PlayedHandType.THREEOFAKIND, "Venus")]
        [InlineData(PlayedHandType.FOUROFAKIND, "Mars")]
        [InlineData(PlayedHandType.FLUSH, "Jupiter")]
        [InlineData(PlayedHandType.FULLHOUSE, "Earth")]
        [InlineData(PlayedHandType.STRAIGHT, "Saturn")]
        [InlineData(PlayedHandType.STRAIGHTFLUSH, "Neptune")]
        [InlineData(PlayedHandType.FIVEOFAKIND, "Planet X")]
        [InlineData(PlayedHandType.FLUSHFIVE, "Eris")]
        [InlineData(PlayedHandType.FLUSHHOUSE, "Ceres")]
        public void UseConsumable_PlanetCard_LevelsCorrectHand(PlayedHandType handType, string expectedName)
        {
            ResetToBlindSelection();
            AddPlanetForHand(handType);
            Assert.True(AllHandsLevel(1));
            Assert.Single(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(expectedName, ZoneManager.ConsumableZone.Cards[0].ConsumableData.ConsumableName);
            UseCon();

            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(2, ScoreHandler.HandLevels[handType]);
            foreach (var hLevel in ScoreHandler.HandLevels.Where(x => x.Key != handType))
            {
                Assert.Equal(1, hLevel.Value);
            }
        }

        #region Helpers
        private void AddPlanetForHand(PlayedHandType handType)
        {
            ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakePlanetCard(handType));
            //if (!MarketOptionsManager.IsHiddenPlanet(handType))
            //{
            //    ZoneManager.ConsumableZone.DrawTargetFrom(MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.PLANET_CARD], MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.PLANET_CARD].Cards.First(x => x.ConsumableData.PlanetHandType == handType));
            //}
            //else
            //{
            //    ZoneManager.ConsumableZone.DrawTargetFrom(MarketOptionsManager.SpecialPool_HiddenPlanets, MarketOptionsManager.SpecialPool_HiddenPlanets.Cards.First(x => x.ConsumableData.PlanetHandType == handType));
            //}
        }
        #endregion
    }
}
