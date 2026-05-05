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

        public void ResetToBlindSelection()
        {
            ResetEngineForTest();
            FlowHandler.StartNewAnte();
            FlowHandler.InitializeBlindSelectionRound();
        }

        public void ResetToFirstBlindPlayRound()
        {
            ResetToBlindSelection();
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

        public void AddTarot(string tarotName)
        {
            ZoneManager.ConsumableZone.DrawTargetFrom(MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.TAROT_CARD], MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.TAROT_CARD].Cards.First(x => x.ConsumableData.ConsumableName == tarotName));
        }

        public void AddSpectral(string spectralName)
        {
            ZoneManager.ConsumableZone.DrawTargetFrom(MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.SPECTRAL_CARD], MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.SPECTRAL_CARD].Cards.First(x => x.ConsumableData.ConsumableName == spectralName));
        }

        public void UseCon() => ConsumableManager.UseConsumable(ZoneManager.ConsumableZone.Cards[0]);

        public bool AllHandsLevel(int lvl) => !ScoreHandler.HandLevels.Any(x => x.Value != lvl);
    }
}
