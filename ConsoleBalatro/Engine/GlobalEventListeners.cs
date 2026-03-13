using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public static class GlobalEventListeners
    {
        public static void SetupGlobalListeners()
        {
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = ChangeZoneSizeForNegative });

            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = ForceStoneCardConsideration, MyContextType = EventContextType.SelectedCardBeingConsideredForCalc });

            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = AddEnhancementBonusFromTriggeringCard, MyContextType = EventContextType.CardTrigger });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = AddEditionBonusFromTriggeringCard, MyContextType = EventContextType.CardTrigger });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = AddEnhancementBonusPostScoringForInHandCards, MyContextType = EventContextType.CardTrigger });

            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = AddSealEffectForScoringCards });

        }

        public static void ChangeZoneSizeForNegative(EngineEventArgs args)
        {
            //TODO: Setting existing card to negative should ALSO change the zone max size.
            if(args is EngineCardDrawnToZoneArgs drawArgs && drawArgs.CardBeingDrawn.Edition == Edition.NEGATIVE && drawArgs.ZoneDrawnTo.MaxCapacity != -1)
            {
                drawArgs.ZoneDrawnTo.MaxCapacity += 1;
            }
            else if(args is EngineCardDiscardedFromZoneArgs discArgs && discArgs.CardBeingDiscarded.Edition == Edition.NEGATIVE && discArgs.ZoneCardIsLeaving.MaxCapacity != -1)
            {
                discArgs.ZoneCardIsLeaving.MaxCapacity -= 1;
            }
        }

        //Handle all scoring bonuses for Edition cards triggered.
        private static void AddEditionBonusFromTriggeringCard(EngineEventArgs args)
        {
            if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger)
            {
                var c = triggerArgs.CardThatIsTriggering;
                if(c.Edition == Edition.HOLOGRAPHIC)
                {
                    Globals.EmitMultAdd(10, c);
                }else if(c.Edition == Edition.FOIL)
                {
                    Globals.EmitChipsAdd(50, c);
                }else if(c.Edition == Edition.POLYCHROME)
                {
                    Globals.EmitMultMult(1.5, c);
                }
            }
        }

        //Handle all scoring bonuses from enhanced cards triggered.
        private static void AddEnhancementBonusFromTriggeringCard(EngineEventArgs args)
        {
            if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger)
            {
                var c = triggerArgs.CardThatIsTriggering;
                if(c.Enhancement == Enhancement.GLASS)
                {
                    //TODO: Chance of glass card dying after.
                    Globals.EmitMultMult(2, c);
                }else if(c.Enhancement == Enhancement.MULT)
                {
                    Globals.EmitMultAdd(4, c);
                }else if(c.Enhancement == Enhancement.BONUSCHIPS)
                {
                    Globals.EmitChipsAdd(30, c);
                }else if(c.Enhancement == Enhancement.LUCKY)
                {
                    if(Globals.RollRandom(1, 5))
                    {
                        Globals.EmitMultAdd(20, c);
                    }

                    if(Globals.RollRandom(1, 15))
                    {
                        Globals.EmitMoneyGain(20, c);
                    }
                }
            }
        }

        private static void AddEnhancementBonusPostScoringForInHandCards(EngineEventArgs args)
        {
            if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.isInHandPostScoringTrigger)
            {
                if(triggerArgs.CardThatIsTriggering.Enhancement == Enhancement.STEEL)
                {
                    Globals.EmitMultMult(1.5, triggerArgs.CardThatIsTriggering);
                }
            }
        }

        private static void AddSealEffectForScoringCards(EngineEventArgs args)
        {
            if(args is EngineCardPreTriggerArgs preArgs && preArgs.CardAboutToTrigger.Seal == Seal.RED)
            {
                preArgs.numTriggersToDo += 1;
            }else if(args is EngineCardTriggerArgs triggerArgs && triggerArgs.isScoringTrigger)
            {
                if(triggerArgs.CardThatIsTriggering.Seal == Seal.GOLD)
                {
                    Globals.EmitMoneyGain(3, triggerArgs.CardThatIsTriggering);
                }
            }else if(args is EngineCardDiscardedFromHandArgs discArgs)
            {
                if(discArgs.CardBeingDiscarded.Seal == Seal.PURPLE)
                {
                    if(ZoneManager.ConsumableZone.HasRoom)
                        MarketOptionsManager.DrawNumMarketItems(BuyItemType.PLANET_CARD, 1, ZoneManager.ConsumableZone);
                }
            }
        }

        //If selected card considered for scoring is Stone, force it.
        private static void ForceStoneCardConsideration(EngineEventArgs args)
        {
            if(args.MyContext.Context == EventContextType.SelectedCardBeingConsideredForCalc && args is EngineCardChosenForPlayedHandArgs chosenArgs && chosenArgs.CardBeingConsidered.Enhancement == Enhancement.STONE)
            {
                chosenArgs.WillBeIncludedInCalc = true;
            }
        }
    }
}
