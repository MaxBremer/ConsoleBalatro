using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Vouchers;
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
    public class VoucherTests : TestClassBase
    {
        [Theory]
        [InlineData("Grabber", "Nacho Tong")]
        [InlineData("Magic Trick", "Illusion")]
        [InlineData("Planet Merchant", "Planet Tycoon")]
        [InlineData("Tarot Merchant", "Tarot Tycoon")]
        [InlineData("Director's Cut", "Retcon")]
        public void AddVoucher_Predecessor_CorrectlyAddsSuccessor(string oldName, string newName)
        {
            ResetToBlindSelection(returnVoucher: true);
            //A hilarious bug: tests would fail sometimes. Why?
            //Turns out pool has all vouchers but one after reset, so when that one lines up test fails. Why?
            //CAUSE ONE VOUCHER IS DRAWN TO THE VOUCHER MARKET ZONE AT START OF BLIND U BIG DUMMY.

            Assert.True(VoucherIsInMarket(oldName));
            Assert.False(VoucherIsInMarket(newName));
            AddVoucher(oldName);
            Assert.False(VoucherIsInMarket(oldName));
            Assert.True(VoucherIsInMarket(newName));
        }

        [Fact]
        public void AddVoucher_GrabberAndNachoTong_CorrectlyAddsHandAndIncludesUpgrade()
        {
            ResetToBlindSelection(returnVoucher: true);
            TestIncreasingIntVouchers(() => Globals.MaxHandsPerRound, "Grabber", "Nacho Tong");
        }

        [Fact]
        public void AddVoucher_WastefulAndRecyclomancy_CorrectlyAddsDiscardAndIncludesUpgrade()
        {
            ResetToBlindSelection(returnVoucher: true);
            TestIncreasingIntVouchers(() => Globals.MaxDiscardsPerRound, "Wasteful", "Recyclomancy");
        }

        [Fact]
        public void AddVoucher_SeedMoneyAndMoneyTree_CorrectlyAddsInterestCap()
        {
            ResetToBlindSelection(returnVoucher: true);
            TestIncreasingIntVouchers(() => Globals.CurMaxInterest, "Seed Money", "Money Tree", firstIncrement: 5, secondIncrement: 15);
        }

        [Fact]
        public void AddVoucher_BlankAndAntimatter_CorrectlyAddsJokerCapacity()
        {
            ResetToBlindSelection(returnVoucher: true);
            TestIncreasingIntVouchers(() => ZoneManager.JokerZone.MaxCapacity, "Blank", "Antimatter", firstIncrement: 0, secondIncrement: 1);
        }

        [Fact]
        public void AddVoucher_HieroglyphAndPetroglyph_CorrectlyModifiesAnteAndVals()
        {
            ResetToBlindSelection(returnVoucher: true);
            FlowHandler.CurrentAnte = 3;
            var oldAnte = FlowHandler.CurrentAnte;
            var oldHands = Globals.MaxHandsPerRound;
            var oldDisc = Globals.MaxDiscardsPerRound;
            var oldVoucher = "Hieroglyph";
            var newVoucher = "Petroglyph";
            Assert.False(VoucherIsInMarket(newVoucher));
            Assert.True(VoucherIsInMarket(oldVoucher));
            AddVoucher(oldVoucher);
            Assert.Equal(oldAnte - 1, FlowHandler.CurrentAnte);
            Assert.Equal(oldHands - 1, Globals.MaxHandsPerRound);
            Assert.Equal(oldDisc, Globals.MaxDiscardsPerRound);
            Assert.True(VoucherIsInMarket(newVoucher));
            AddVoucher(newVoucher);
            Assert.Equal(oldAnte - 2, FlowHandler.CurrentAnte);
            Assert.Equal(oldDisc - 1, Globals.MaxDiscardsPerRound);
            Assert.Equal(oldHands - 1, Globals.MaxHandsPerRound);
            Assert.False(VoucherIsInMarket(oldVoucher));
            Assert.False(VoucherIsInMarket(newVoucher));
        }

        [Fact]
        public void AddVoucher_PaintBrushAndPalette_CorrectlyAddsHandSize()
        {
            ResetToBlindSelection(returnVoucher: true);
            TestIncreasingIntVouchers(() => Globals.HandSize, "Paint Brush", "Palette");
        }

        [Fact]
        public void AddVoucher_OverstockAndPlus_CorrectlyAddsMarketSizeAndRefills()
        {
            ResetToFirstShop();
            var oldMarketSize = ZoneManager.MainMarketZone.MaxCapacity;
            var oldVoucher = "Overstock";
            var newVoucher = "Overstock Plus";

            Assert.Equal(2, ZoneManager.MainMarketZone.Cards.Count);
            ZoneManager.DestroyCard(ZoneManager.MainMarketZone.Cards[0]);
            Assert.Single(ZoneManager.MainMarketZone.Cards);

            Assert.False(VoucherIsInMarket(newVoucher));
            Assert.True(VoucherIsInMarket(oldVoucher));
            AddVoucher(oldVoucher);
            Assert.Equal(oldMarketSize + 1, ZoneManager.MainMarketZone.MaxCapacity);
            Assert.Equal(oldMarketSize + 1, ZoneManager.MainMarketZone.Cards.Count);
            Assert.True(VoucherIsInMarket(newVoucher));
            AddVoucher(newVoucher);
            Assert.Equal(oldMarketSize + 2, ZoneManager.MainMarketZone.MaxCapacity);
            Assert.Equal(oldMarketSize + 2, ZoneManager.MainMarketZone.Cards.Count);
            Assert.False(VoucherIsInMarket(oldVoucher));
            Assert.False(VoucherIsInMarket(newVoucher));
        }

        [Fact]
        public void AddVoucher_RerollSurplusAndGlut_CorrectlyDiscountRerolls()
        {
            ResetToFirstShop();
            var oldRerollCost = Globals.CurrentRerollCost;
            TestIncreasingIntVouchers(() => Globals.BaseRerollCost, "Reroll Surplus", "Reroll Glut", firstIncrement: -2, secondIncrement: -4);
            Assert.Equal(oldRerollCost - 4, Globals.CurrentRerollCost);
        }

        [Fact]
        public void AddVoucher_ClearanceSaleAndLiquidation_CorrectlyAppliesDiscounts()
        {
            ResetToFirstShop();
            var oldGlobalDiscount = Globals.DiscountMultiplier;
            var oldFirstItemCost = ZoneManager.MainMarketZone.Cards[0].BuyCost;
            var oldFirstPackCost = ZoneManager.PackMarketZone.Cards[0].BuyCost;
            var oldVoucher = "Clearance Sale";
            var newVoucher = "Liquidation";

            Assert.False(VoucherIsInMarket(newVoucher));
            Assert.True(VoucherIsInMarket(oldVoucher));
            AddVoucher(oldVoucher);
            Assert.Equal(oldGlobalDiscount - 0.25, Globals.DiscountMultiplier);
            Assert.Equal((int)(oldFirstItemCost * 0.75), ZoneManager.MainMarketZone.Cards[0].BuyCost);
            Assert.Equal((int)(oldFirstPackCost * 0.75), ZoneManager.PackMarketZone.Cards[0].BuyCost);
            Assert.True(VoucherIsInMarket(newVoucher));
            AddVoucher(newVoucher);
            Assert.False(VoucherIsInMarket(oldVoucher));
            Assert.False(VoucherIsInMarket(newVoucher));
            Assert.Equal(oldGlobalDiscount - 0.5, Globals.DiscountMultiplier);
            Assert.Equal((int)(oldFirstItemCost * 0.5), ZoneManager.MainMarketZone.Cards[0].BuyCost);
            Assert.Equal((int)(oldFirstPackCost * 0.5), ZoneManager.PackMarketZone.Cards[0].BuyCost);
        }

        [Fact]
        public void AddVoucher_CrystalBallAndOmenGlobe_CorrectlyModifiesConsumableSpaceAndOdds()
        {
            ResetToBlindSelection(returnVoucher: true);
            var oldConCount = ZoneManager.ConsumableZone.MaxCapacity;
            var oldVoucher = "Crystal Ball";
            var newVoucher = "Omen Globe";

            Assert.False(VoucherIsInMarket(newVoucher));
            Assert.True(VoucherIsInMarket(oldVoucher));
            AddVoucher(oldVoucher);
            Assert.Equal(oldConCount + 1, ZoneManager.ConsumableZone.MaxCapacity);
            Assert.True(VoucherIsInMarket(newVoucher));
            Assert.False(VoucherIsInMarket(oldVoucher));
            AddVoucher(newVoucher);
            Assert.False(VoucherIsInMarket(oldVoucher));
            Assert.False(VoucherIsInMarket(newVoucher));

            bool spectralIsOption = false;
            var list = new EngineEventListener()
            {
                MyContextType = EventContextType.PackOddsEstablished,
                MyAction = args =>
                {
                    if (args is EngineOddsEstablishedForPackArgs packArgs)
                    {
                        spectralIsOption = packArgs.Odds.ContainsKey(BuyItemType.SPECTRAL_CARD);
                    }
                }
            };
            EngineEventHandler.StartListening(list);
            PackActions.OpenPack(ConsumableManager.MakePack(PackType.BASIC_TAROT));

            //after the open, we should've seen that spectral cards were a possibility.
            //can't do an actual check for spectral cards cause random, would have to set up some kind of rigging for weighted-odds random selection.
            //and frankly, I don't wanna. This is fine.
            Assert.True(spectralIsOption);
        }

        [Fact]
        public void AddVoucher_Telescope_CorrectlyGivesCommonHandInPacks()
        {
            //i mean it'll happen every time, but COULD happen by coincidence, so doing this a couple times just makes it even less likely that this would happen by coincidence.
            //not impossible, but cmon. give me a break.
            for (int i = 0; i < 5; i++)
            {
                ResetToFirstShop();
                Assert.False(VoucherIsInMarket("Observatory"));
                Assert.True(VoucherIsInMarket("Telescope"));
                AddVoucher("Telescope");
                Assert.True(VoucherIsInMarket("Observatory"));
                Assert.False(VoucherIsInMarket("Telescope"));
                PackActions.OpenPack(ConsumableManager.MakePack(PackType.JUMBO_PLANET));
                Assert.Equal(PlayedHandType.FLUSHFIVE, ZoneManager.PackOptionZone.Cards[0].ConsumableData.PlanetHandType);
                Assert.Equal("Eris", ZoneManager.PackOptionZone.Cards[0].ConsumableData.ConsumableName);
            }
        }

        [Fact]
        public void AddVoucher_Observatory_CorrectlySetsUpNewPlanetMult()
        {
            ResetToFirstBlindPlayRound(resetVoucher: true);
            var record = CaptureScoringContributions();

            Assert.False(VoucherIsInMarket("Observatory"));
            Assert.True(VoucherIsInMarket("Telescope"));
            AddVoucher("Telescope");
            Assert.True(VoucherIsInMarket("Observatory"));
            Assert.False(VoucherIsInMarket("Telescope"));
            AddVoucher("Observatory");
            Assert.False(VoucherIsInMarket("Observatory"));
            Assert.False(VoucherIsInMarket("Telescope"));

            PlayHand("AS");
            Assert.Empty(record.MultMultSources);
            Assert.Equal(1, record.MultMultFromEmits);
            Assert.Equal(16, Globals.TotalCurrentChips);

            ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakePlanetCard(PlayedHandType.HIGHCARD));
            var con = ZoneManager.ConsumableZone.Cards[0];

            PlayHand("AS");
            Assert.Single(record.MultMultSources);
            Assert.Equal(1.5, record.MultMultFromEmits);
            Assert.Contains(con, record.MultMultSources);
            Assert.Equal(16 + 24, Globals.TotalCurrentChips);
        }

        [Fact]
        public void AddVoucher_HoneAndGlowUp_CorrectlyIncreasesOdds()
        {
            ResetToBlindSelection(returnVoucher: true);
            List<int> oldOdds = new List<int>() { GetOdds(Edition.POLYCHROME), GetOdds(Edition.FOIL), GetOdds(Edition.HOLOGRAPHIC) };
            Assert.False(VoucherIsInMarket("Glow Up"));
            Assert.True(VoucherIsInMarket("Hone"));
            AddVoucher("Hone");
            Assert.True(VoucherIsInMarket("Glow Up"));
            Assert.False(VoucherIsInMarket("Hone"));

            Assert.NotEqual(oldOdds[0], GetOdds(Edition.POLYCHROME));
            Assert.NotEqual(oldOdds[1], GetOdds(Edition.FOIL));
            Assert.NotEqual(oldOdds[2], GetOdds(Edition.HOLOGRAPHIC));
            CheckOdds(9, 28, 40);
            AddVoucher("Glow Up");
            CheckOdds(21, 56, 80);
        }

        [Fact]
        public void AddVoucher_DirectorCutAndRetcon_CorrectlySetAvailableBossBlindRerolls()
        {
            ResetToBlindSelection(returnVoucher: true);
            TestIncreasingIntVouchers(() => Globals.BaseBossBlindRerollsAllowed, "Director's Cut", "Retcon", firstIncrement: 1, secondIncrement: -1);
        }

        [Fact]
        public void AddVoucher_TarotMerchantAndTycoon_CorrectlyIncreaseOdds()
        {
            ResetToBlindSelection(returnVoucher: true);
            TestIncreasingIntVouchers(() => GetIntArgOfRoll(BuyItemType.TAROT_CARD), "Tarot Merchant", "Tarot Tycoon", firstIncrement: 5, secondIncrement: 15);
        }

        [Fact]
        public void AddVoucher_PlanetMerchantAndTycoon_CorrectlyIncreaseOdds()
        {
            ResetToBlindSelection(returnVoucher: true);
            TestIncreasingIntVouchers(() => GetIntArgOfRoll(BuyItemType.PLANET_CARD), "Planet Merchant", "Planet Tycoon", firstIncrement: 5, secondIncrement: 15);
        }

        [Fact]
        public void AddVoucher_MagicTrick_CorrectlyAllowsCardPurchases()
        {
            ResetToBlindSelection(returnVoucher: true);
            TestIncreasingIntVouchers(() => GetIntArgOfRoll(BuyItemType.PLAYING_CARD), "Magic Trick", "Illusion", firstIncrement: 5, secondIncrement: 5);
        }

        private int GetIntArgOfRoll(BuyItemType itemType)
        {
            var args = new EngineMarketTypeBeingChosenArgs
            {
                WeightsBeingRolled = MarketPullManager.MainMarketWeights.ToDictionary(),
                MyContext = new()
                {
                    Context = EventContextType.MarketTypeBeingChosen,
                }
            };
            EngineEventHandler.TriggerEvent(args);
            return args.WeightsBeingRolled.TryGetValue(itemType, out int outcome) ? outcome : 0;
        }

        [Fact]
        public void AddVoucher_Illusion_CorrectlyAllowsPlayingCardEnhancements()
        {
            ResetToBlindSelection(returnVoucher: true);
            //Lookit my cute lil hack :)
            //I wont change it its my baby :)
            //He just a lil guy :)
            //I'm not writing another full fucking test :)
            TestIncreasingIntVouchers(() => Globals.ShopPlayingCardsGetModifiers ? 1 : 0, "Magic Trick", "Illusion", firstIncrement: 0, secondIncrement: 1);
        }

        #region Helpers
        private void TestIncreasingIntVouchers(Func<int> getVal, string oldVoucher, string newVoucher, int firstIncrement = 1, int secondIncrement = 2)
        {
            var oldVal = getVal();
            Assert.False(VoucherIsInMarket(newVoucher));
            Assert.True(VoucherIsInMarket(oldVoucher));
            AddVoucher(oldVoucher);
            Assert.Equal(oldVal + firstIncrement, getVal());
            Assert.True(VoucherIsInMarket(newVoucher));
            Assert.False(VoucherIsInMarket(oldVoucher));
            AddVoucher(newVoucher);
            Assert.Equal(oldVal + secondIncrement, getVal());
            Assert.False(VoucherIsInMarket(oldVoucher));
            Assert.False(VoucherIsInMarket(newVoucher));
        }

        private void ResetToFirstShop()
        {
            ResetToBlindSelection(returnVoucher: true);
            FlowHandler.StartSelectedBlind();
            PlayHand("AS,AS,AS,AS,AS");
            FlowHandler.ClosePostRound();
        }

        private void CheckOdds(int polyOdds, int holoOdds, int foilOdds)
        {
            Assert.Equal(polyOdds, GetOdds(Edition.POLYCHROME));
            Assert.Equal(foilOdds, GetOdds(Edition.FOIL));
            Assert.Equal(holoOdds, GetOdds(Edition.HOLOGRAPHIC));
        }

        private int GetOdds(Edition ed)
        {
            return MarketOptionsManager.RandomEditionOdds[BuyItemType.JOKER][ed];
        }
        #endregion
    }
}
