using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public class JokerZone : CardZone
    {
        private Dictionary<Card, List<EngineEventListener>> ActiveJokerEffects = new();

        public JokerZone(int slotNum)
        {
            MaxCapacity = slotNum;
            Name = "Jokers";

            EngineEventHandler.StartListening(new EngineEventListener()
            {
                MyContextType = EventContextType.CardDrawnToZone,
                MyAction = args =>
                {
                    if(args is EngineCardDrawnToZoneArgs drawnArgs && drawnArgs.ZoneDrawnTo == this)
                    {
                        AddJokerEffs(drawnArgs.CardBeingDrawn);
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
                        RemoveJokerEffs(discArgs.CardBeingDiscarded);
                    }
                }
            });
        }

        private void AddJokerEffs(Card jokerCard)
        {
            if (jokerCard.JokerData == null)
            {
                return;
            }
            if (jokerCard.JokerData.Listeners != null && jokerCard.JokerData.Listeners.Count > 0)
            {
                ActiveJokerEffects.Add(jokerCard, new List<EngineEventListener>());
                foreach (var listener in jokerCard.JokerData.Listeners)
                {
                    EngineEventHandler.StartListening(listener);
                    ActiveJokerEffects[jokerCard].Add(listener);
                }
            }

            if (jokerCard.JokerData.OnJokerGainEffs != null && jokerCard.JokerData.OnJokerGainEffs.Count > 0)
            {
                foreach (var onGain in jokerCard.JokerData.OnJokerGainEffs)
                {
                    onGain();
                }
            }
        }
    

        private void RemoveJokerEffs(Card jokerCard)
        {
            if(jokerCard.JokerData == null)
            {
                return;
            }
            if(jokerCard.JokerData.Listeners != null && jokerCard.JokerData.Listeners.Count > 0 && ActiveJokerEffects.ContainsKey(jokerCard))
            {
                foreach (var listener in jokerCard.JokerData.Listeners)
                {
                    EngineEventHandler.StopListening(listener);
                    ActiveJokerEffects[jokerCard].Remove(listener);
                }
                ActiveJokerEffects.Remove(jokerCard);
            }

            if(jokerCard.JokerData.OnJokerRemovalEffs != null && jokerCard.JokerData.OnJokerRemovalEffs.Count > 0)
            {
                foreach (var onRemove in jokerCard.JokerData.OnJokerRemovalEffs)
                {
                    onRemove();
                }
            }
        }
    }
}
