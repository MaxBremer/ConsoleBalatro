using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
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
    public class SpectralTests : TestClassBase
    {
        [Theory]
        [InlineData("Talisman", Seal.GOLD)]
        [InlineData("Deja Vu", Seal.RED)]
        [InlineData("Trance", Seal.BLUE)]
        [InlineData("Medium", Seal.PURPLE)]
        public void ActivateConsumable_SealSpectrals_CorrectlyAppliesSeal(string spectralName, Seal sealType)
        {
            ResetToFirstBlindPlayRound();
            AddSpectral(spectralName);
            Assert.Single(ZoneManager.ConsumableZone.Cards);

            var targetCard = ZoneManager.HandZone.Cards[0];
            targetCard.ToggleSelect();
            Assert.Equal(Seal.NONE, targetCard.Seal);
            ConsumableManager.UseConsumable(ZoneManager.ConsumableZone.Cards[0]);

            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(sealType, targetCard.Seal);
            for (int i = 1; i < ZoneManager.HandZone.Cards.Count; i++)
            {
                Assert.Equal(Seal.NONE, ZoneManager.HandZone.Cards[i].Seal);
            }
        }

        [Theory]
        [InlineData("Grim", "ACE", 2)]
        [InlineData("Familiar", "FACE", 3)]
        [InlineData("Incantation", "NUMBERED", 4)]
        public void ActivateConsumable_CardGenSpectrals_CorrectlyDestroysCardAndCreatesCards(string spectralName, string rankGroup, int numCardsGenerated)
        {
            ResetToFirstBlindPlayRound();
            AddSpectral(spectralName);
            var record = CaptureCardChangeEvents();

            var oldCardCount = ZoneManager.HandZone.Cards.Count;
            var oldCards = new List<Card>();
            oldCards.AddRange(ZoneManager.HandZone.Cards);
            ConsumableManager.UseConsumable(ZoneManager.ConsumableZone.Cards[0]);

            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(1, record.NumCardsDestroyed);
            Assert.Equal(numCardsGenerated, record.NumCardsAdded);
            Assert.Equal(oldCardCount + (numCardsGenerated - 1), ZoneManager.HandZone.Cards.Count);
            foreach (var c in record.CardsAddedToHand)
            {
                Assert.NotEqual(Enhancement.NONE, c.Enhancement);
                Assert.Contains(c.Rank, EngineUtils.RankGroups[rankGroup]);
                Assert.Contains(c, ZoneManager.HandZone.Cards);
            }
        }

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


        #region Helpers
        private static CardChangeCaptures CaptureCardChangeEvents()
        {
            var capture = new CardChangeCaptures();
            //Listen for Destroyed cards.
            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.CardDestroyed,
                MyAction = args =>
                {
                    var cardDestroyArgs = Assert.IsType<EngineCardDestroyedArgs>(args);
                    capture.CardsDestroyed.Add(cardDestroyArgs.CardDestroyed);
                }
            });

            //Listen for Added cards.
            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.CardDrawnToZone,
                MyAction = args =>
                {
                    var cardAddArgs = Assert.IsType<EngineCardDrawnToZoneArgs>(args);
                    if(cardAddArgs.ZoneDrawnTo == ZoneManager.HandZone)
                    {
                        capture.CardsAddedToHand.Add(cardAddArgs.CardBeingDrawn);
                    }
                }
            });

            return capture;
        }
        private sealed class CardChangeCaptures
        {
            public int NumCardsDestroyed => CardsDestroyed.Count;
            public int NumCardsAdded => CardsAddedToHand.Count;
            public List<Card> CardsAddedToHand { get; set; } = new();
            public List<Card> CardsDestroyed { get; set; } = new();
        }
        #endregion
    }
}
