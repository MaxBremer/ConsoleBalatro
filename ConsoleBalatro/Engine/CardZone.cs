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
    public class CardZone
    {
        public string Name;
        public List<Card> Cards = new List<Card>();
        public int MaxCapacity = -1;//-1 for infinite
        public bool HasRoom => MaxCapacity == -1 || Cards.Count < MaxCapacity;
        public bool HasRoomFor(int numCards) => MaxCapacity == -1 || MaxCapacity - Cards.Count > numCards;

        public void Shuffle()
        {
            //TODO: test this. Changed from old shuffle method which was basically just doing random swaps.
            Cards = Cards.OrderBy(x => Random.Shared.Next()).ToList();
        }
        //TODO: invisible add/remove causes so many fucking problems better to just remove... not even that useful.
        public virtual bool AddCard(Card card, bool invisibleAdd = false, bool overrideSpace = false)
        {
            if (!HasRoom && !overrideSpace)
                return false;

            Cards.Add(card);
            card.MyZone = this;
            if (!invisibleAdd)
            {
                var context = new EventContext() { Context = EventContextType.CardDrawnToZone };
                var evArgs = new EngineCardDrawnToZoneArgs() { CardBeingDrawn = card, ZoneDrawnTo = this, MyContext = context };
                EngineEventHandler.TriggerEvent(evArgs);

            }

            return true;
        }

        public bool AddCards(List<Card> cardsToAdd, bool invisibleAdd = false, bool overrideSpace = false)
        {
            if (!HasRoomFor(cardsToAdd.Count) && !overrideSpace)
                return false;
            foreach (var card in cardsToAdd)
            {
                AddCard(card, invisibleAdd: invisibleAdd, overrideSpace: overrideSpace);
            }
            return true;
        }

        public void ClearCards(bool invisibleRemove = false)
        {
            var cardsToRemove = Cards.ToList();
            foreach (var card in cardsToRemove)
            {
                RemoveCard(card, invisibleRemove: invisibleRemove);
            }
        }

        public void RemoveCard(Card card, bool invisibleRemove = false)
        {
            if (Cards.Contains(card))
            {
                card.MyZone = null;
                Cards.Remove(card);
                if (!invisibleRemove)
                {
                    var context = new EventContext() { Context = EventContextType.CardDiscarded };
                    var evArgs = new EngineCardDiscardedFromZoneArgs() { CardBeingDiscarded = card, ZoneCardIsLeaving = this, MyContext = context };
                    EngineEventHandler.TriggerEvent(evArgs);
                }
            }
        }

        public void RemoveCards(List<Card> cardsToRemove, bool invisibleRemove = false)
        {
            foreach (var card in cardsToRemove)
            {
                RemoveCard(card, invisibleRemove: invisibleRemove);
            }
        }

        public void DrawTargetFrom(CardZone zone, Card target, bool invisibleAdd = false, bool ignoreSpaceLimits = false)
        {
            if ((!ignoreSpaceLimits && !HasRoom) || !zone.Cards.Contains(target))
                return;
            zone.RemoveCard(target);
            AddCard(target, invisibleAdd: invisibleAdd, overrideSpace: ignoreSpaceLimits);
        }

        public void DrawTargetsFrom(CardZone zone, List<Card> targets, bool invisibleAdd = false, bool ignoreSpaceLimits = false)
        {
            if ((!ignoreSpaceLimits && !HasRoomFor(targets.Count)) || !targets.All(x => zone.Cards.Contains(x)))
                return;
            foreach (var target in targets)
            {
                DrawTargetFrom(zone, target, invisibleAdd: invisibleAdd);
            }
        }

        //Add optional params for override space and invisible add/remove if needed in the future
        public Card DrawFromAndReturn(CardZone zone, bool ignoreSpaceLimits = false)
        {
            if ((!ignoreSpaceLimits && !HasRoom) || zone.Cards.Count == 0)
                return null;
            var target = zone.Cards.First();
            DrawTargetFrom(zone, target, ignoreSpaceLimits: ignoreSpaceLimits);
            return target;
        }

        public Card DrawTargetFromAndReturn(CardZone zone, Card target)
        {
            if (!HasRoom || !zone.Cards.Contains(target))
                return null;
            DrawTargetFrom(zone, target);
            return target;
        }

        public void DrawFrom(CardZone zone)
        {
            if (!HasRoom || zone.Cards.Count == 0)
                return;
            var target = zone.Cards.First();
            DrawTargetFrom(zone, target);
        }

        public void DrawXFrom(CardZone zone, int numDraw)
        {
            //No checks here because if no room or enough from zone, we want to draw as many as possible.
            for (int i = 0; i < numDraw; i++)
            {
                DrawFrom(zone);
            }
        }

        public void DrawUntilCapacityFrom(CardZone zone)
        {
            //No checks here because if no room or enough from zone, we want to draw as many as possible.
            while (HasRoom && zone.Cards.Count > 0)
            {
                DrawFrom(zone);
            }
        }
    }
}
