using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public static class DataManager
    {
        public const string CARD_ORDER_DIVIDER = "|";
        public const string CARD_LIST_DIVIDER = "&";

        public static Dictionary<int, Card> CardsByID = new();

        public static Stack<string> SavedPlayOrderings = new();

        public static void TrackCard(Card c)
        {
            CardsByID.Add(c.ID, c);
        }

        public static void UnTrackCard(Card c)
        {
            CardsByID.Remove(c.ID);
        }

        public static Card GetCardFromList(List<Card> cards, int idToGet) => cards.Where(car => car.ID == idToGet).Count() == 1 ? cards.Where(x => x.ID == idToGet).First() : null;

        public static string OrderStringFromCards(List<Card> cards) => string.Join(CARD_ORDER_DIVIDER, cards.Select(c => c.ID));

        public static string FullSaveFromZoneOrders(List<string> zoneOrders) => string.Join(CARD_LIST_DIVIDER, zoneOrders);

        public static List<int> OrderListFromString(string orderString) => orderString
            .Split(new[] { CARD_ORDER_DIVIDER }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

        public static void ReorderCards(List<Card> cards, string orderString)
        {
            // Parse the ordering string into a list of IDs
            var orderedIds = OrderListFromString(orderString);

            // Validate: check that the IDs match exactly
            var cardIds = cards.Select(c => c.ID).ToHashSet();
            var orderIds = orderedIds.ToHashSet();

            if (!cardIds.SetEquals(orderIds))
                return; // IDs don't match, exit without modifying.

            // Create a lookup from ID to Card
            var cardLookup = cards.ToDictionary(c => c.ID);

            // Reorder in place
            for (int i = 0; i < orderedIds.Count; i++)
            {
                cards[i] = cardLookup[orderedIds[i]];
            }
        }

        public static void SaveCurrentOrder()
        {
            //TODO: Replace this with a "true" play round save, so that it can save things like Boss Blind ability.
            //For now not seeded (this whole dang class is not needed until modded content, ie opening a shop in the middle of a blind then returning to that blind)
            //ORDER: Hand, Deck, discard, hiddenPlay
            List<string> allOrderings = new();
            if(ZoneManager.HandZone.Cards.Count > 0)
                allOrderings.Add(OrderStringFromCards(ZoneManager.HandZone.Cards));
            if (ZoneManager.DeckZone.Cards.Count > 0)
                allOrderings.Add(OrderStringFromCards(ZoneManager.DeckZone.Cards));
            if (ZoneManager.DiscardZone.Cards.Count > 0)
                allOrderings.Add(OrderStringFromCards(ZoneManager.DiscardZone.Cards));
            if (ZoneManager.HiddenPlayZone.Cards.Count > 0)
                allOrderings.Add(OrderStringFromCards(ZoneManager.HiddenPlayZone.Cards));

            //TODO: Take these strings and save them.
        }

        public static string PossiblyGetOrderFor(CardZone zone)
        {
            if(zone == null || zone.Cards.Count == 0)
            {
                return "";
            }

            return OrderStringFromCards(zone.Cards);
        }
    }
}
