using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ConsoleBalatro.Tests
{
    public class TestClassBase
    {
        public void ResetEngineForTest()
        {
            EngineEventHandler.ResetFullEventHandler();

            Globals.ResetGlobalValues();
            Globals.ClearGameStateStack();
            Globals.InitializeMain();
            Globals.ClearGameStateStack();
            Globals.ResetGlobalValues();

            FlowHandler.CurrentAnte = 0;
            FlowHandler.CurrentSelectedBlind = BlindType.SMALL;
            FlowHandler.CurrentTempChanges = null;
            FlowHandler.CurrentBossBlind = "";
            BossBlindDb.BossBlindsAlreadyUsed.Clear();

            Globals.Money = 0;
            Globals.CurMaxInterest = 5;
            Globals.SetStartOfRoundStats();
            Globals.RequiredChipsForCurrentBlind = -1;

            ZoneManager.HiddenBlindAttributeZone.ClearCards(true);
        }

        public void ResetToBlindSelection(bool returnVoucher = false)
        {
            ResetEngineForTest();
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
            if(returnVoucher)
                MarketOptionsManager.ReturnMarketItemFromZone(ZoneManager.VoucherMarketZone.Cards[0], ZoneManager.VoucherMarketZone);
        }

        public void ResetToFirstBlindPlayRound(bool resetVoucher = false)
        {
            ResetToBlindSelection(resetVoucher);
            FlowHandler.StartSelectedBlind();
        }

        public List<Card> BuildKnownHand(string handDef, bool selectAll = true)
        {
            ZoneManager.HandZone.Cards.Clear();
            var cards = CardFactory.CardListFromDefString(handDef, ",");
            ZoneManager.HandZone.AddCards(cards);
            if (selectAll)
            {
                foreach (var c in cards)
                {
                    c.isSelected = true;
                }
            }

            return cards;
        }

        public void PlayHand(string handDef, bool upgrade = false)
        {
            BuildKnownHand(handDef);
            if (upgrade)
            {
                foreach (var c in ZoneManager.HandZone.Cards)
                {
                    c.Seal = Seal.RED;
                    c.Edition = Edition.HOLOGRAPHIC;
                }
            }
            Globals.PlayCurrentlySelectedHand();
        }

        public void DiscardHand(string handDef)
        {
            BuildKnownHand(handDef);
            Globals.DiscardSelectedFromHand();
        }

        public void AddJoker(string jokerName)
        {
            ZoneManager.JokerZone.AddCard(JokerDb.GenerateJokerCard(jokerName));
        }

        public void RigNextRoll(bool desiredResult)
        {
            var listener = new EngineEventListener()
            {
                MyContextType = EventContextType.RandomRollHappening,
            };
            listener.MyAction = args =>
            {
                if (args is EngineRandomRollArgs rollArgs && rollArgs.OverrideResult == null)
                {
                    rollArgs.OverrideResult = desiredResult;
                    listener.RemoveAfterTriggering = true;
                }
            };
            EngineEventHandler.StartListening(listener);
        }

        protected static ContributionCapture CaptureScoringContributions()
        {
            var capture = new ContributionCapture();

            //Individual Mult/Chip gains
            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.GainEmit,
                MyAction = args =>
                {
                    var gain = Assert.IsType<EngineChipsMultGainEmitArgs>(args);
                    if (gain.ChipsGainEmitted >= 0)
                    {
                        capture.ChipsFromEmits += gain.ChipsGainEmitted;
                        capture.ChipSources.Add(gain.SourceOfEmit);
                    }

                    if (gain.MultGainEmitted >= 0)
                    {
                        capture.MultFromEmits += gain.MultGainEmitted;
                        capture.MultSources.Add(gain.SourceOfEmit);
                    }

                    if (gain.MultMultEmitted >= 0)
                    {
                        capture.MultMultFromEmits *= gain.MultMultEmitted;
                        capture.MultMultSources.Add(gain.SourceOfEmit);
                    }
                }
            });

            //Final, total chip gain.
            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.TotalChipsGained,
                MyAction = args =>
                {
                    var total = Assert.IsType<EngineTotalChipsGainArgs>(args);
                    capture.FinalTotalGain = total.AmountBeingGained;
                }
            });

            //Hand(s) played.
            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.HandPlayedCalculated,
                MyAction = args =>
                {
                    var played = Assert.IsType<EngineHandPlayArgs>(args);
                    capture.PlayedHandTypes.Add(played.HandBeingPlayed);
                }
            });

            return capture;
        }

        protected class ContributionCapture
        {
            public int ChipsFromEmits { get; set; }
            public double MultFromEmits { get; set; }
            public double MultMultFromEmits { get; set; } = 1d;
            public int FinalTotalGain { get; set; }
            public List<Card> ChipSources { get; } = new();
            public List<Card> MultSources { get; } = new();
            public List<Card> MultMultSources { get; } = new();

            public List<PlayedHandType> PlayedHandTypes { get; set; } = new();
        }

        public void AddTarot(string tarotName)
        {
            ZoneManager.ConsumableZone.DrawTargetFrom(MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.TAROT_CARD], MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.TAROT_CARD].Cards.First(x => x.ConsumableData.ConsumableName == tarotName));
        }

        public void AddSpectral(string spectralName)
        {
            ZoneManager.ConsumableZone.DrawTargetFrom(MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.SPECTRAL_CARD], MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.SPECTRAL_CARD].Cards.First(x => x.ConsumableData.ConsumableName == spectralName));
        }

        public void AddVoucher(string voucherName)
        {
            ZoneManager.ActiveVoucherZone.DrawTargetFrom(MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.VOUCHER], MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.VOUCHER].Cards.First(x => x.isVoucher && x.JokerData.JokerName == voucherName));
        }

        public bool VoucherIsInMarket(string voucherName)
        {
            return MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.VOUCHER].Cards.Any(x => x.isVoucher && x.JokerData.JokerName == voucherName);
        }

        public void UseCon() => ConsumableManager.UseConsumable(ZoneManager.ConsumableZone.Cards[0]);

        public bool AllHandsLevel(int lvl) => !ScoreHandler.HandLevels.Any(x => x.Value != lvl);
    }
}
