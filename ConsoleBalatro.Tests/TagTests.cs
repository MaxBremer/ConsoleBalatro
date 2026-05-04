using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Tags;
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
    public class TagTests : TestClassBase
    {
        [Theory]
        [InlineData(4, 8)]
        [InlineData(15, 30)]
        [InlineData(40, 80)]
        [InlineData(0, 0)]
        [InlineData(50, 90)]
        public void SkipSmallBlind_TriggersEconomyTag_CorrectlyDoublesMoney(int startingMoney, int expectedMoney)
        {
            ResetToBlindSelection();
            FlowHandler.CurSmallBlindTag = TagType.ECONOMY;
            Globals.Money = startingMoney;
            var record = CaptureTagEvents();
            FlowHandler.DoSkip();
            Assert.Equal(BlindType.BIG, FlowHandler.CurrentSelectedBlind);
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(1, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.ECONOMY, record.TagsTriggeredInstantly[0].JokerData.TagData.MyType);
            Assert.Equal(expectedMoney, Globals.Money);
        }

        [Fact]
        public void SkipBothBlinds_TriggersSpeedTags_CorrectlyAddsMoney()
        {
            ResetToBlindSelection();
            FlowHandler.CurSmallBlindTag = TagType.SPEED;
            FlowHandler.CurBigBlindTag = TagType.SPEED;
            var record = CaptureTagEvents();

            Assert.Equal(0, Globals.Money);
            FlowHandler.DoSkip();
            Assert.Equal(BlindType.BIG, FlowHandler.CurrentSelectedBlind);
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(1, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.SPEED, record.TagsTriggeredInstantly[0].JokerData.TagData.MyType);
            Assert.Equal(5, Globals.Money);

            FlowHandler.DoSkip();
            Assert.Equal(BlindType.BOSS, FlowHandler.CurrentSelectedBlind);
            Assert.Equal(2, record.TagAddEventCount);
            Assert.Equal(2, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.SPEED, record.TagsTriggeredInstantly[1].JokerData.TagData.MyType);
            Assert.Equal(15, Globals.Money);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void PlayOneRoundThenSkip_TriggersHandyTag_CorrectlyAddsMoney(bool playTwoHands)
        {
            ResetToFirstBlindPlayRound();
            if (playTwoHands)
            {
                PlayHand("AS");
            }
            PlayHand("AS,AS,AS,AS,AS");
            FlowHandler.ClosePostRound();
            FlowHandler.CloseMarketRound();
            FlowHandler.CurBigBlindTag = TagType.HANDY;
            var oldMoney = Globals.Money;
            var record = CaptureTagEvents();
            FlowHandler.DoSkip();

            Assert.Equal(BlindType.BOSS, FlowHandler.CurrentSelectedBlind);
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(1, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.HANDY, record.TagsTriggeredInstantly[0].JokerData.TagData.MyType);
            Assert.Equal(playTwoHands ? oldMoney + 2 : oldMoney + 1, Globals.Money);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void PlayOneRoundWithDiscardsThenSkip_TriggersGarbageTag_CorrectlyAddsMoney(int numDisc)
        {
            ResetToFirstBlindPlayRound();
            for (int i = 0; i < numDisc; i++)
            {
                DiscardHand("AS");
            }
            PlayHand("AS,AS,AS,AS,AS");
            FlowHandler.ClosePostRound();
            FlowHandler.CloseMarketRound();
            FlowHandler.CurBigBlindTag = TagType.GARBAGE;
            var oldMoney = Globals.Money;
            var record = CaptureTagEvents();
            FlowHandler.DoSkip();

            Assert.Equal(BlindType.BOSS, FlowHandler.CurrentSelectedBlind);
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(1, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.GARBAGE, record.TagsTriggeredInstantly[0].JokerData.TagData.MyType);
            Assert.Equal(oldMoney + numDisc, Globals.Money);
        }

        [Fact]
        public void SkipThenPlayRound_TriggersCouponTagInMarket_MarketIsFreeButNotVoucher()
        {
            ResetToBlindSelection();
            FlowHandler.CurSmallBlindTag = TagType.COUPON;
            var record = CaptureTagEvents();
            FlowHandler.DoSkip();
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(0, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            FlowHandler.StartSelectedBlind();
            PlayHand("AS,AS,AS,AS,AS");
            FlowHandler.ClosePostRound();
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(0, record.TriggerInstantCount);
            Assert.Equal(1, record.TriggerListenerCount);
            Assert.Equal(TagType.COUPON, record.TagsTriggeredViaListener[0].JokerData.TagData.MyType);

            Assert.NotEqual(0, ZoneManager.VoucherMarketZone.Cards[0].BuyCost);
            foreach (var c in ZoneManager.MainMarketZone.Cards)
            {
                Assert.Equal(0, c.BuyCost);
            }
            foreach (var c in ZoneManager.PackMarketZone.Cards)
            {
                Assert.Equal(0, c.BuyCost);
            }
        }

        [Fact]
        public void SkipTwoRounds_TriggersDoubleAndSpeedTags_MoneyIncreasesCorrectly()
        {
            ResetToBlindSelection();
            FlowHandler.CurSmallBlindTag = TagType.DOUBLE_TAG;
            FlowHandler.CurBigBlindTag = TagType.SPEED;
            var record = CaptureTagEvents();

            FlowHandler.DoSkip();
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(0, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.DOUBLE_TAG, record.TagsAdded[0].JokerData.TagData.MyType);

            FlowHandler.DoSkip();
            Assert.Equal(3, record.TagAddEventCount);
            Assert.Equal(2, record.TriggerInstantCount);
            Assert.Equal(1, record.TriggerListenerCount);
            Assert.Equal(TagType.DOUBLE_TAG, record.TagsAdded[0].JokerData.TagData.MyType);
            Assert.Equal(TagType.SPEED, record.TagsAdded[1].JokerData.TagData.MyType);
            Assert.Equal(TagType.SPEED, record.TagsAdded[2].JokerData.TagData.MyType);
            Assert.Equal(20, Globals.Money);
        }

        [Fact]
        public void SkipOneRoundThenPlay_TriggersJuggleTag_HandSizeIncreasesCorrectly()
        {
            ResetToBlindSelection();
            FlowHandler.CurBigBlindTag = TagType.JUGGLE;
            var record = CaptureTagEvents();
            FlowHandler.StartSelectedBlind();
            var initialHandSize = Globals.HandSize;
            PlayHand("AS,AS,AS,AS,AS");
            FlowHandler.ClosePostRound();
            FlowHandler.CloseMarketRound();
            FlowHandler.DoSkip();
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(0, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.JUGGLE, record.TagsAdded[0].JokerData.TagData.MyType);

            FlowHandler.StartSelectedBlind();
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(0, record.TriggerInstantCount);
            Assert.Equal(1, record.TriggerListenerCount);
            Assert.Equal(initialHandSize + 3, Globals.HandSize);
            Assert.Equal(initialHandSize + 3, ZoneManager.HandZone.Cards.Count);
        }

        [Fact]
        public void SkipOneRound_TriggersBossRerollTag_CorrectlyRerollsBoss()
        {
            ResetToBlindSelection();
            FlowHandler.CurSmallBlindTag = TagType.BOSS_REROLL;
            var record = CaptureTagEvents();
            var oldBoss = FlowHandler.CurrentBossBlind;
            Assert.False(string.IsNullOrEmpty(oldBoss));
            FlowHandler.DoSkip();
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(1, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.BOSS_REROLL, record.TagsAdded[0].JokerData.TagData.MyType);
            Assert.NotEqual(oldBoss, FlowHandler.CurrentBossBlind);
            Assert.False(string.IsNullOrEmpty(FlowHandler.CurrentBossBlind));
        }

        [Fact]
        public void SkipBothRoundsPlayBoss_TriggersInvestmentTag_CorrectlyAddsPostRoundMoney()
        {
            ResetToBlindSelection();
            FlowHandler.CurSmallBlindTag = TagType.INVESTMENT;
            FlowHandler.CurBigBlindTag = TagType.COUPON;//Won't matter, we never go to market in this test, just need a known tag I can ignore.
            var record = CaptureTagEvents();
            FlowHandler.DoSkip();
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(0, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.INVESTMENT, record.TagsAdded[0].JokerData.TagData.MyType);
            FlowHandler.DoSkip();
            Assert.Equal(2, record.TagAddEventCount);
            Assert.Equal(0, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.COUPON, record.TagsAdded[1].JokerData.TagData.MyType);

            Globals.Money = 0;//Ensures that only blind, hands, and investment are money sources after this round.
            FlowHandler.StartSelectedBlind();
            PlayHand("AS,AS,AS,AS,AS");

            Assert.Equal(GameState.PostRoundRewardsMenu, Globals.CurrentGameState);
            Assert.NotEmpty(Globals.GameStateStack);
            Assert.NotNull(Globals.CurrentGameStateObj);
            var moneySources = Globals.CurrentGameStateObj.PostRoundMoneySources;
            //5$ for blind, 3$ for remaining hand, 25$ for investment tag. 33 total.
            Assert.Equal(33, Globals.CurrentGameStateObj.PostRoundMoneyToGive);
            Assert.Equal(3, moneySources.Count);
            Assert.StartsWith("Investment", moneySources[2].Item1);
            Assert.Equal(25, moneySources[2].Item2);
            Assert.Equal(2, record.TagAddEventCount);
            Assert.Equal(0, record.TriggerInstantCount);
            Assert.Equal(1, record.TriggerListenerCount);
            Assert.Equal(TagType.INVESTMENT, record.TagsTriggeredViaListener[0].JokerData.TagData.MyType);

            FlowHandler.ClosePostRound();
            Assert.Equal(33, Globals.Money);
        }

        [Fact]
        public void SkipThenGoToMarket_RerollsTagTriggersCorrectly_SetsRerollCostTo0()
        {
            ResetToBlindSelection();
            FlowHandler.CurSmallBlindTag = TagType.REROLLS;
            var record = CaptureTagEvents();

            FlowHandler.DoSkip();
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(0, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.REROLLS, record.TagsAdded[0].JokerData.TagData.MyType);

            FlowHandler.StartSelectedBlind();
            PlayHand("AS,AS,AS,AS,AS");
            FlowHandler.ClosePostRound();
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(0, record.TriggerInstantCount);
            Assert.Equal(1, record.TriggerListenerCount);
            Assert.Equal(TagType.REROLLS, record.TagsTriggeredViaListener[0].JokerData.TagData.MyType);
            Assert.Equal(0, Globals.CurrentRerollCost);
            MarketGeneralManager.RerollMainMarket();
            Assert.Equal(1, Globals.CurrentRerollCost);
            //Make sure its not 0 in next market

            FlowHandler.CloseMarketRound();
            FlowHandler.StartSelectedBlind();
            PlayHand("AS,AS,AS,AS,AS");
            FlowHandler.ClosePostRound();
            Assert.Equal(5, Globals.CurrentRerollCost);
        }

        [Fact]
        public void SkipOne_TopUpTagTriggers_GivesTwoJokers()
        {
            ResetToBlindSelection();
            FlowHandler.CurSmallBlindTag = TagType.TOP_UP;
            var record = CaptureTagEvents();

            Assert.Empty(ZoneManager.JokerZone.Cards);
            FlowHandler.DoSkip();
            Assert.Equal(1, record.TagAddEventCount);
            Assert.Equal(1, record.TriggerInstantCount);
            Assert.Equal(0, record.TriggerListenerCount);
            Assert.Equal(TagType.TOP_UP, record.TagsAdded[0].JokerData.TagData.MyType);
            Assert.Equal(TagType.TOP_UP, record.TagsTriggeredInstantly[0].JokerData.TagData.MyType);
            Assert.Equal(2, ZoneManager.JokerZone.Cards.Count);
        }

        #region Helpers
        private static TagEventCapture CaptureTagEvents()
        {
            var capture = new TagEventCapture();
            //Listen for added tags.
            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.TagAdded,
                MyAction = args =>
                {
                    var tagAdd = Assert.IsType<EngineTagAddedEventArgs>(args);
                    if (tagAdd.isPostAdd)
                        capture.TagsAdded.Add(tagAdd.TagCard);
                }
            });

            //Listen for instant-add triggers.
            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.TagActivatedInstantly,
                MyAction = args =>
                {
                    var tagTrigger = Assert.IsType<EngineTagTriggeredArgs>(args);
                    capture.TagsTriggeredInstantly.Add(tagTrigger.TagThatTriggered);
                }
            });

            //Listen for listener-based triggers.
            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.TagActivatedViaListener,
                MyAction = args =>
                {
                    var tagTrigger = Assert.IsType<EngineTagTriggeredArgs>(args);
                    capture.TagsTriggeredViaListener.Add(tagTrigger.TagThatTriggered);
                }
            });
            return capture;
        }
        private sealed class TagEventCapture
        {
            public int TagAddEventCount => TagsAdded.Count;
            public int TriggerInstantCount => TagsTriggeredInstantly.Count;
            public int TriggerListenerCount => TagsTriggeredViaListener.Count;
            public List<Card> TagsAdded { get; } = new();
            public List<Card> TagsTriggeredInstantly { get; } = new();
            public List<Card> TagsTriggeredViaListener { get; } = new();
        }
        #endregion
    }
}
