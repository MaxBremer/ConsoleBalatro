using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
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
            UseCon();

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
            UseCon();

            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(1, record.NumCardsDestroyed);
            Assert.Equal(numCardsGenerated, record.NumCardsAdded);
            Assert.Equal(oldCardCount + (numCardsGenerated - 1), ZoneManager.HandZone.Cards.Count);
            foreach (var c in record.CardsAdded)
            {
                Assert.NotEqual(Enhancement.NONE, c.Enhancement);
                Assert.Contains(c.Rank, EngineUtils.RankGroups[rankGroup]);
                Assert.Contains(c, ZoneManager.HandZone.Cards);
            }
        }

        [Fact]
        public void ActivateConsumable_Sigil_SetsSuitOfHand()
        {
            ResetToFirstBlindPlayRound();
            AddSpectral("Sigil");

            UseCon();
            var suit = ZoneManager.HandZone.Cards[0].Suit;
            foreach (var c in ZoneManager.HandZone.Cards)
            {
                Assert.Equal(suit, c.Suit);
            }
        }

        [Fact]
        public void ActivateConsumable_Ouija_SetsRankOfHand()
        {
            ResetToFirstBlindPlayRound();
            AddSpectral("Ouija");
            var oldHandSize = Globals.HandSize;

            UseCon();
            var rank = ZoneManager.HandZone.Cards[0].Rank;
            Assert.Equal(oldHandSize - 1, Globals.HandSize);
            foreach (var c in ZoneManager.HandZone.Cards)
            {
                Assert.Equal(rank, c.Rank);
            }
        }

        [Fact]
        public void ActivateConsumable_Ectoplasm_SetsNegativeEditionCorrectly()
        {
            ResetToBlindSelection();
            AddSpectral("Ectoplasm");
            AddJoker("JIMBO");
            var oldMaxCapacity = ZoneManager.JokerZone.MaxCapacity;
            var oldHandSize = Globals.HandSize;

            Assert.Equal(Edition.BASE, ZoneManager.JokerZone.Cards[0].Edition);
            UseCon();
            Assert.Equal(Edition.NEGATIVE, ZoneManager.JokerZone.Cards[0].Edition);
            Assert.Equal(oldHandSize - 1, Globals.HandSize);
            Assert.Equal(oldMaxCapacity + 1, ZoneManager.JokerZone.MaxCapacity);
        }

        [Fact]
        public void ActivateConsumable_Immolate_CorrectlyDestroysCardsAndGivesMoney()
        {
            ResetToFirstBlindPlayRound();
            AddSpectral("Immolate");
            var oldCardCount = ZoneManager.HandZone.Cards.Count;
            var record = CaptureCardChangeEvents();
            var oldMoney = Globals.Money;

            UseCon();
            Assert.Equal(5, record.NumCardsDestroyed);
            Assert.Equal(oldCardCount - 5, ZoneManager.HandZone.Cards.Count);
            foreach (var c in record.CardsDestroyed)
            {
                Assert.DoesNotContain(c, ZoneManager.HandZone.Cards);
            }
            Assert.Equal(oldMoney + 20, Globals.Money);
        }

        [Fact]
        public void ActivateConsumable_Aura_CorrectlyGivesEdition()
        {
            ResetToFirstBlindPlayRound();
            AddSpectral("Aura");
            var targetCard = ZoneManager.HandZone.Cards[0];
            targetCard.ToggleSelect();
            Assert.Equal(Edition.BASE, targetCard.Edition);
            var validEditions = new List<Edition> { Edition.FOIL, Edition.HOLOGRAPHIC, Edition.POLYCHROME };

            UseCon();
            Assert.NotEqual(Edition.BASE, targetCard.Edition);
            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Contains(targetCard.Edition, validEditions);
        }

        [Fact]
        public void ActivateConsumable_Wraith_CorrectlyGeneratesJokerAndRemovesMoney()
        {
            ResetToBlindSelection();
            AddSpectral("Wraith");
            Globals.EmitMoneyGain(35, null);
            Assert.Equal(35, Globals.Money);
            Assert.Empty(ZoneManager.JokerZone.Cards);
            var record = CaptureCardChangeEvents();

            UseCon();
            Assert.Equal(0, Globals.Money);
            Assert.Single(ZoneManager.JokerZone.Cards);
            Assert.Equal(1, record.NumCardsAdded);
            Assert.Equal(JokerRarity.RARE, ZoneManager.JokerZone.Cards[0].JokerData.Rarity);
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
            UseCon();

            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Equal(oldCardCount + 2, ZoneManager.HandZone.Cards.Count);
            var lastCard = ZoneManager.HandZone.Cards.Last();
            Assert.Equal(targetCard.Rank, lastCard.Rank);
            Assert.Equal(targetCard.Suit, lastCard.Suit);
            Assert.Equal(targetCard.Edition, lastCard.Edition);
            Assert.Equal(targetCard.Enhancement, lastCard.Enhancement);
            Assert.Equal(targetCard.Seal, lastCard.Seal);
        }

        [Fact]
        public void ActivateConsumable_Ankh_CorrectlyCopiesJokerAndDestroysOthers()
        {
            ResetToBlindSelection();
            AddSpectral("Ankh");
            var jokersToAdd = new List<string> { "JIMBO", "FOUR FINGERS", "GOLDEN JOKER" };
            foreach (var j in jokersToAdd)
            {
                AddJoker(j);
            }
            Assert.Equal(3, ZoneManager.JokerZone.Cards.Count);
            Assert.Single(ZoneManager.ConsumableZone.Cards);
            var record = CaptureCardChangeEvents();
            UseCon();

            Assert.Equal(2, record.NumCardsDestroyed);
            Assert.Equal(1, record.NumCardsAdded);
            Assert.Equal(2, ZoneManager.JokerZone.Cards.Count);
            var firstJokeName = ZoneManager.JokerZone.Cards[0].JokerData.DBName;
            Assert.Contains(firstJokeName, jokersToAdd);
            Assert.Equal(firstJokeName, ZoneManager.JokerZone.Cards[0].JokerData.DBName);
        }

        //THIS IS MOSTLY FOR FOUR FINGERS, NOT ACTUALLY ANKH.
        //TODO: DO THIS KIND OF THING IN JOKER TESTS
        [Fact]
        public void ActivateConsumable_Ankh_TheFourFingersTest()
        {
            ResetToBlindSelection();
            AddSpectral("Ankh");
            Assert.Equal(5, EngineUtils.LenFlush);
            Assert.Equal(5, EngineUtils.LenStraight);
            for (var i = 0; i < 3; i++)
            {
                AddJoker("FOUR FINGERS");
                Assert.Equal(4, EngineUtils.LenFlush);
                Assert.Equal(4, EngineUtils.LenStraight);
            }
            UseCon();
            Assert.Equal(4, EngineUtils.LenFlush);
            Assert.Equal(4, EngineUtils.LenStraight);

            ZoneManager.DestroyCard(ZoneManager.JokerZone.Cards[0]);
            Assert.Equal(4, EngineUtils.LenFlush);
            Assert.Equal(4, EngineUtils.LenStraight);
            ZoneManager.DestroyCard(ZoneManager.JokerZone.Cards[0]);
            Assert.Equal(5, EngineUtils.LenFlush);
            Assert.Equal(5, EngineUtils.LenStraight);
            Assert.Empty(ZoneManager.JokerZone.Cards);
        }

        [Fact]
        public void ActivateConsumable_Hex_CorrectlyDestroysOthersAndSetsEdition()
        {
            ResetToBlindSelection();
            AddSpectral("Hex");
            var jokersToAdd = new List<string> { "JIMBO", "FOUR FINGERS", "GOLDEN JOKER" };
            foreach (var j in jokersToAdd)
            {
                AddJoker(j);
            }
            Assert.Equal(3, ZoneManager.JokerZone.Cards.Count);
            Assert.Single(ZoneManager.ConsumableZone.Cards);
            var record = CaptureCardChangeEvents();
            UseCon();

            Assert.Equal(2, record.NumCardsDestroyed);
            Assert.Equal(0, record.NumCardsAdded);
            Assert.Single(ZoneManager.JokerZone.Cards);
            Assert.Contains(ZoneManager.JokerZone.Cards[0].JokerData.DBName, jokersToAdd);
            Assert.Equal(Edition.POLYCHROME, ZoneManager.JokerZone.Cards[0].Edition);
        }

        [Fact]
        public void ActivateConsumable_Soul_CorrectlyCreatesLegendary()
        {
            ResetToBlindSelection();
            AddSpectral("SOUL");
            Assert.Single(ZoneManager.ConsumableZone.Cards);
            Assert.Empty(ZoneManager.JokerZone.Cards);
            var record = CaptureCardChangeEvents();
            UseCon();

            Assert.Equal(1, record.NumCardsAdded);
            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.Single(ZoneManager.JokerZone.Cards);
            Assert.Equal(ZoneManager.JokerZone.Cards[0], record.CardsAdded[0]);
            Assert.Equal(JokerRarity.LEGENDARY, ZoneManager.JokerZone.Cards[0].JokerData.Rarity);
        }

        [Fact]
        public void ActivateConsumable_BlackHole_CorrectlyLevelsAllHands()
        {
            ResetToBlindSelection();
            AddSpectral("Black Hole");
            Assert.Single(ZoneManager.ConsumableZone.Cards);
            Assert.True(AllHandsLevel(1));
            UseCon();

            Assert.Empty(ZoneManager.ConsumableZone.Cards);
            Assert.True(AllHandsLevel(2));
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
                    if(cardAddArgs.ZoneDrawnTo == ZoneManager.HandZone || cardAddArgs.ZoneDrawnTo == ZoneManager.JokerZone)
                    {
                        capture.CardsAdded.Add(cardAddArgs.CardBeingDrawn);
                    }
                }
            });

            return capture;
        }
        private sealed class CardChangeCaptures
        {
            public int NumCardsDestroyed => CardsDestroyed.Count;
            public int NumCardsAdded => CardsAdded.Count;
            public List<Card> CardsAdded { get; set; } = new();
            public List<Card> CardsDestroyed { get; set; } = new();
        }
        #endregion
    }
}
