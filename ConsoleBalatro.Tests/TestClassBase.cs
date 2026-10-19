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
    //The one important thing that has to happen beforehand, every time; no progress saving from unit tests, you'll overwrite user save data with junk.
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

        /// <summary>
        /// Reset the global balatro engine.
        /// </summary>
        public void ResetEngineForTest()
        {
            Globals.ResetFullEngine();
        }

        /// <summary>
        /// Reset the engine to the blind selection for a fresh run.
        /// </summary>
        /// <param name="returnVoucher">Indicates whether to return the voucher from the voucher market zone.</param>
        public void ResetToBlindSelection(bool returnVoucher = false)
        {
            ResetEngineForTest();
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
            if (returnVoucher && ZoneManager.VoucherMarketZone != null)//WHY DID I WRITE THIS. SHOULDNT RESETENGINE ALSO RESET THE VOUCHER MARKET ZONE. WHYYYYYYY.
                MarketOptionsManager.ReturnMarketItemFromZone(ZoneManager.VoucherMarketZone.Cards[0], ZoneManager.VoucherMarketZone);
        }

        /// <summary>
        /// Reset the engine to the first play round of a fresh run.
        /// </summary>
        /// <param name="resetVoucher">Indicates whether to return the voucher from the voucher market zone.</param>
        public void ResetToFirstBlindPlayRound(bool resetVoucher = false)
        {
            ResetToBlindSelection(resetVoucher);
            FlowHandler.StartSelectedBlind();
        }

        /// <summary>
        /// Helper method to reset the engine, load the passed deck, and start the run.
        /// </summary>
        /// <param name="deckDBName">DB name of the deck to load.</param>
        public void ResetToBlindDeckSetup(string deckDBName)
        {
            ResetEngineForTest();
            DeckDb.BecomeDeck(deckDBName);
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
        }

        /// <summary>
        /// Build the hand passed in the def string in the HandZone.
        /// </summary>
        /// <param name="handDef">The definition string of the hand to build (ie "AS, KS" is one ace of spades one king of spades)</param>
        /// <param name="selectAll">All created cards should also be selected in-game.</param>
        /// <param name="clearHand">Clear out the hand zone before adding this hand.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Test helper to play a hand passed as a def string.
        /// </summary>
        /// <param name="handDef">Definition string of the hand to play (ie "AS,AS" for two ace of spades)</param>
        /// <param name="upgrade">Improve the hand to all red-seal holographic before playing.</param>
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

        /// <summary>
        /// Build the hand given by the passed def string, then discard it.
        /// </summary>
        /// <param name="handDef">Definition string of the hand to discard (ie "AS,AS" for two ace of spades)</param>
        public void DiscardHand(string handDef)
        {
            BuildKnownHand(handDef);
            Globals.DiscardSelectedFromHand();
        }

        /// <summary>
        /// Build the joker of the passed DB name and add it to the joker zone.
        /// </summary>
        /// <param name="jokerName">DB name of the joker to add and build.</param>
        public void AddJoker(string jokerName)
        {
            ZoneManager.JokerZone?.AddCard(JokerDb.GenerateJokerCard(jokerName));
        }

        /// <summary>
        /// A test helper to ensure the next "random" chance returns the passed result.
        /// </summary>
        /// <param name="desiredResult">The desired result for the next random chance roll.</param>
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

        /// <summary>
        /// Create a DiscardCapture object for tracking discards for tests.
        /// </summary>
        /// <returns>A fresh DiscardCapture object actively tracking discards.</returns>
        public static DiscardCapture CaptureDiscards()
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

        /// <summary>
        /// Create a ContributionCapture object for tracking all scoring contributions for tests.
        /// </summary>
        /// <returns>A fresh ContributionCapture object actively tracking scoring contributions.</returns>
        public static ContributionCapture CaptureScoringContributions()
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

        public class DiscardCapture
        {
            public int NumCardsDiscarded => CardsDiscarded == null ? 0 : CardsDiscarded.Count;

            public List<Card> CardsDiscarded { get; } = new();
        }

        public class ContributionCapture
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

        /// <summary>
        /// Add the tarot card with the passed DB name to the consumable zone.
        /// </summary>
        /// <param name="tarotName">The DB name of the tarot card to add.</param>
        public void AddTarot(string tarotName)
        {
            ZoneManager.ConsumableZone?.AddCard(ConsumableManager.MakeTarotCard(tarotName.ToUpper()));
        }

        /// <summary>
        /// Add the spectral card with the passed DB name to the consumable zone.
        /// </summary>
        /// <param name="spectralName">DB name for the spectral to be created.</param>
        public void AddSpectral(string spectralName)
        {
            ZoneManager.ConsumableZone?.AddCard(ConsumableManager.MakeSpectralCard(spectralName.ToUpper()));
        }

        /// <summary>
        /// Add the voucher for the passed DB name to the active voucher zone.
        /// </summary>
        /// <param name="voucherName">DB name for the voucher to be added to active voucher zone.</param>
        public void AddVoucher(string voucherName)
        {
            ZoneManager.ActiveVoucherZone?.AddCard(VoucherDb.MakeVoucherCard(voucherName.ToUpper()));
        }

        /// <summary>
        /// Check whether the passed voucher is in the market pool.
        /// </summary>
        /// <param name="voucherName">The voucher to be checked</param>
        /// <returns>A boolean indicating whether the passed voucher is available.</returns>
        public bool VoucherIsInMarket(string voucherName)
        {
            return VoucherPoolRules.CurrentValidVouchers.Contains(voucherName.ToUpper()) && (ZoneManager.ActiveVoucherZone == null || !ZoneManager.ActiveVoucherZone.Cards.Any(c => c.IsVoucher && c.JokerData != null && c.JokerData.DBName == voucherName.ToUpper()));
        }

        /// <summary>
        /// A helper function to activate the first consumable in the consumable zone.
        /// </summary>

        public void UseCon()
        {
            if (ZoneManager.ConsumableZone != null)
            {
                ConsumableManager.UseConsumable(ZoneManager.ConsumableZone.Cards[0]);
            }
        }

        /// <summary>
        /// Check whether all hands are the passed level.
        /// </summary>
        /// <param name="lvl">The level to check against all hands.</param>
        /// <returns>A boolean indicating whether all hand levels are the passed level.</returns>
        public bool AllHandsLevel(int lvl) => !ScoreHandler.HandLevels.Any(x => x.Value != lvl);
    }
}
