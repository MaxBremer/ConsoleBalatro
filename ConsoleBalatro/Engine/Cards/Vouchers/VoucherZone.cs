using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Vouchers
{
    public class VoucherZone : CardZone, IJokerContainer
    {
        private Dictionary<Card, List<EngineEventListener>> ActiveVoucherEffects = new();

        public VoucherZone() 
        {
            Name = "Vouchers";

            EngineEventHandler.StartListening(new EngineEventListener()
            {
                MyContextType = EventContextType.CardDrawnToZone,
                MyAction = args =>
                {
                    if(args is EngineCardDrawnToZoneArgs drawnArgs && drawnArgs.ZoneDrawnTo == this)
                    {
                        AddVoucherEffs(drawnArgs.CardBeingDrawn);
                    }
                }
            });

            EngineEventHandler.StartListening(new EngineEventListener()
            {
                MyContextType = EventContextType.CardDiscarded,
                MyAction = args =>
                {
                    if(args is EngineCardDiscardedFromZoneArgs discArgs && discArgs.ZoneCardIsLeaving == this)
                    {
                        RemoveVoucherEffs(discArgs.CardBeingDiscarded);
                    }
                }
            });
        }

        public void AddJokerEffs(Card jokerCard)
        {
            AddVoucherEffs(jokerCard);
        }

        public void RemoveJokerEffs(Card jokerCard)
        {
            RemoveVoucherEffs(jokerCard);
        }

        private void AddVoucherEffs(Card jokerCard)
        {
            if(jokerCard.JokerData == null || jokerCard.JokerData.isVoucher == false)
            {
                return;
            }
            if(jokerCard.JokerData.Listeners.Count > 0)
            {
                ActiveVoucherEffects.Add(jokerCard, new List<EngineEventListener>());
                foreach (var listener in jokerCard.JokerData.Listeners)
                {
                    EngineEventHandler.StartListening(listener);
                    ActiveVoucherEffects[jokerCard].Add(listener);
                }
            }

            if(jokerCard.JokerData.OnJokerGainEffs.Count > 0)
            {
                foreach (var eff in jokerCard.JokerData.OnJokerGainEffs)
                {
                    eff();
                }
            }

            //TODO: This should probably be handled by an event listener instead of being hardcoded here.
            if (!string.IsNullOrEmpty(jokerCard.JokerData.SuccessorVoucherDBName))
            {
                MarketOptionsManager.AddToVoucherPool(jokerCard.JokerData.SuccessorVoucherDBName);
            }

            var args = new EngineVoucherRedeemedArgs() { BeingRedeemed = jokerCard };
            EngineEventHandler.TriggerEvent(args);
        }

        private void RemoveVoucherEffs(Card jokerCard)
        {
            if (jokerCard.JokerData == null || jokerCard.JokerData.isVoucher == false || !ActiveVoucherEffects.ContainsKey(jokerCard))
            {
                return;
            }
            if (jokerCard.JokerData.Listeners.Count > 0)
            {
                foreach (var listener in jokerCard.JokerData.Listeners)
                {
                    EngineEventHandler.StopListening(listener);
                    ActiveVoucherEffects[jokerCard].Remove(listener);
                }
                ActiveVoucherEffects.Remove(jokerCard);
            }

            if (jokerCard.JokerData.OnJokerRemovalEffs.Count > 0)
            {
                foreach (var eff in jokerCard.JokerData.OnJokerRemovalEffs)
                {
                    eff();
                }
            }

            if(!string.IsNullOrEmpty(jokerCard.JokerData.SuccessorVoucherDBName))
            {
                //If the voucher being removed has a successor, it should now be removed from the voucher pool (if it's there).
                //You would have to re-acquire this one to re-unlock the successor.
                //TODO: Or maybe don't do this idk, maybe getting it once unlocks the successor for the rest of run.
                MarketOptionsManager.AttemptToRemoveFromVoucherPool(jokerCard.JokerData.SuccessorVoucherDBName);
            }
        }
    }
}
