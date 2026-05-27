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
            Cards = Cards.OrderBy(x => Globals.randomNext(Int32.MaxValue)).ToList();
        }
        //TODO: invisible add/remove causes so many fucking problems better to just remove... not even that useful.
        public virtual bool AddCard(Card card, bool invisibleAdd = false, bool overrideSpace = false, CardZone zoneDrawnFrom = null)
        {
            if (!HasRoom && !overrideSpace)
                return false;

            Cards.Add(card);
            card.MyZone = this;
            if (!invisibleAdd)
            {
                var context = new EventContext() { Context = EventContextType.CardDrawnToZone };
                var evArgs = new EngineCardDrawnToZoneArgs() { CardBeingDrawn = card, ZoneDrawnTo = this, ZoneDrawnFrom = zoneDrawnFrom, MyContext = context };
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
            AddCard(target, invisibleAdd: invisibleAdd, overrideSpace: ignoreSpaceLimits, zoneDrawnFrom: zone);
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

        public void DrawFrom(CardZone zone, bool ignoreSpaceLimits = false)
        {
            if (zone.Cards.Count == 0 || (!HasRoom && !ignoreSpaceLimits))
                return;
            var target = zone.Cards.First();
            DrawTargetFrom(zone, target, ignoreSpaceLimits: ignoreSpaceLimits);
        }

        public void DrawXFrom(CardZone zone, int numDraw, bool ignoreSpaceLimits = false)
        {
            //No checks here because if no room or enough from zone, we want to draw as many as possible.
            for (int i = 0; i < numDraw; i++)
            {
                DrawFrom(zone, ignoreSpaceLimits: ignoreSpaceLimits);
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

        public void SwapCardPositions(int ind1, int ind2)
        {
            if (ind1 < 0 || ind2 < 0 || ind1 >= Cards.Count || ind2 >= Cards.Count) return;

            SwapCardPositions(Cards[ind1], Cards[ind2]);
        }

        public void SwapCardPositions(Card c1, Card c2)
        {
            if(!Cards.Contains(c1)  || !Cards.Contains(c2) || c1 == c2) return;
            var firstInd = Cards.IndexOf(c1);
            var secondInd = Cards.IndexOf(c2);

            var args = new EngineCardPositionsSwappingArgs()
            {
                Card1 = c1,
                Card2 = c2,
                Card1OldIndex = firstInd,
                Card2OldIndex = secondInd,
                ZoneOfSwap = this,
                MyContext = new EventContext() { Context = EventContextType.CardPositionsSwapping }
            };
            EngineEventHandler.TriggerEvent(args);

            Cards[secondInd] = c1;
            Cards[firstInd] = c2;

            var nargs = new EngineCardPositionsSwappingArgs()
            {
                Card1 = c1,
                Card2 = c2,
                Card1OldIndex = firstInd,
                Card2OldIndex = secondInd,
                ZoneOfSwap = this,
                MyContext = new EventContext() { Context = EventContextType.CardPositionsSwapDone }
            };
            EngineEventHandler.TriggerEvent(nargs);
        }
    }
}
