using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Vouchers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using ConsoleBalatro.Engine.Pools.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Xunit;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Stakes;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ConsoleBalatro.Tests
{
    public static class TestAssemblySetup
    {
        [ModuleInitializer]
        public static void DisablePermanentProgressSavingForTests()
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
        }
    }

    public class TestClassBase
    {
        public TestClassBase()
        {
            UnlockManager.PermanentProgressSavingDisabled = true;
        }

        public void ResetEngineForTest()
        {
            Globals.ResetFullEngine();
        }

        public void ResetToBlindSelection(bool returnVoucher = false)
        {
            ResetEngineForTest();
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
            if (returnVoucher && ZoneManager.VoucherMarketZone != null)
                MarketOptionsManager.ReturnMarketItemFromZone(ZoneManager.VoucherMarketZone.Cards[0], ZoneManager.VoucherMarketZone);
        }

        public void ResetToFirstBlindPlayRound(bool resetVoucher = false)
        {
            ResetToBlindSelection(resetVoucher);
            FlowHandler.StartSelectedBlind();
        }

        public void ResetToBlindDeckSetup(string deckDBName)
        {
            ResetEngineForTest();
            DeckDb.BecomeDeck(deckDBName);
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
        }

        public List<Card> BuildKnownHand(string handDef, bool selectAll = true, bool clearHand = true)
        {
            if (clearHand)
                ZoneManager.HandZone?.Cards.Clear();
            var cards = CardFactory.CardListFromDefString(handDef, ",");
            ZoneManager.HandZone?.AddCards(cards, overrideSpace: !clearHand);//If we're not clearing the hand, we want to allow going over the normal hand limit, since we're likely just adding cards to an already existing hand.
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
                foreach (var c in ZoneManager.HandZone?.Cards ?? [])
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
            ZoneManager.JokerZone?.AddCard(JokerDb.GenerateJokerCard(jokerName));
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

        protected static DiscardCapture CaptureDiscards()
        {
            var capture = new DiscardCapture();

            EngineEventHandler.StartListening(new EngineEventListener
            {
                MyContextType = EventContextType.CardDrawnToZone,
                MyAction = args =>
                {
                    var drawArgs = Assert.IsType<EngineCardDrawnToZoneArgs>(args);
                    if (drawArgs.ZoneDrawnTo == ZoneManager.DiscardZone)
                        capture.CardsDiscarded.Add(drawArgs.CardBeingDrawn);
                }
            });

            return capture;
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

        protected class DiscardCapture
        {
            public int NumCardsDiscarded => CardsDiscarded == null ? 0 : CardsDiscarded.Count;

            public List<Card> CardsDiscarded { get; } = new();
        }

        protected class ContributionCapture
        {
            public BigInteger ChipsFromEmits { get; set; }
            public double MultFromEmits { get; set; }
            public double MultMultFromEmits { get; set; } = 1d;
            public BigInteger FinalTotalGain { get; set; }
            public List<Card> ChipSources { get; } = new();
            public List<Card> MultSources { get; } = new();
            public List<Card> MultMultSources { get; } = new();

            public List<PlayedHandType> PlayedHandTypes { get; set; } = new();

            public void Reset()
            {
                ChipsFromEmits = 0;
                MultFromEmits = 0;
                MultMultFromEmits = 1;
                FinalTotalGain = 0;
                ChipSources.Clear();
                MultSources.Clear();
                MultMultSources.Clear();
                PlayedHandTypes.Clear();
            }
        }

        public void AddTarot(string tarotName)
        {
            ZoneManager.ConsumableZone?.AddCard(ConsumableManager.MakeTarotCard(tarotName.ToUpper()));
        }

        public void AddSpectral(string spectralName)
        {
            ZoneManager.ConsumableZone?.AddCard(ConsumableManager.MakeSpectralCard(spectralName.ToUpper()));
        }

        public void AddVoucher(string voucherName)
        {
            ZoneManager.ActiveVoucherZone?.AddCard(VoucherDb.MakeVoucherCard(voucherName.ToUpper()));
        }

        public bool VoucherIsInMarket(string voucherName)
        {
            return VoucherPoolRules.CurrentValidVouchers.Contains(voucherName.ToUpper()) && (ZoneManager.ActiveVoucherZone == null || !ZoneManager.ActiveVoucherZone.Cards.Any(c => c.IsVoucher && c.JokerData != null && c.JokerData.DBName == voucherName.ToUpper()));
        }

        public void UseCon()
        {
            if (ZoneManager.ConsumableZone != null)
            {
                ConsumableManager.UseConsumable(ZoneManager.ConsumableZone.Cards[0]);
            }
        }

        public bool AllHandsLevel(int lvl) => !ScoreHandler.HandLevels.Any(x => x.Value != lvl);
    }
}
