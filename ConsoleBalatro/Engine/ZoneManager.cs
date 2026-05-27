using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Vouchers;
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
    public static class ZoneManager
    {
        //Constantly visible zones
        public static JokerZone JokerZone;
        public static CardZone ConsumableZone;
        public static CardZone TagZone;

        //Visible zones during play round
        public static CardZone HandZone;
        public static CardZone CurrentlyBeingPlayedZone;

        //Visible zones during shop round
        public static CardZone MainMarketZone;
        public static CardZone PackMarketZone;
        public static CardZone VoucherMarketZone;

        //Visible zones for pack opening
        public static CardZone PackOptionZone;
        //Use HandZone for cards to use the options on, thus will be affected by hand size limits and such
        //TODO: Maybe set up separate hand-like zone for pack opening?

        public static List<Card> CardsSelectedInHand => HandZone.Cards.Where(x => x.isSelected).ToList();
        public static List<Card> JokersSelectedInZone => JokerZone.Cards.Where(x => x.isSelected).ToList();
        public static List<Card> ConsumablesSelectedInZone => ConsumableZone.Cards.Where(x => x.isSelected).ToList();
        public static List<Card> AllCardsSelected
        {
            get
            {
                List<Card> overall = new();
                overall.AddRange(CardsSelectedInHand);
                overall.AddRange(JokersSelectedInZone);
                overall.AddRange(ConsumablesSelectedInZone);
                return overall;
            }
        }

        public static int HandSize
        {
            get
            {
                return HandZone.MaxCapacity;
            }
            set
            {
                HandZone.MaxCapacity = value;
            }
        }

        public static int MarketSize
        {
            get
            {
                return MainMarketZone.MaxCapacity;
            }
            set
            {
                MainMarketZone.MaxCapacity = value;
            }
        }

        //Invisible zones
        public static CardZone DiscardZone; //Where cards go when discarded during a blind.
        public static CardZone PreDestructionZone; //Special zone for cards about to be destroyed, but needed for the moment. Currently used for tags only.
        public static DestroyCardZone DestructionZone; //Where destroyed cards go, deletes them.
        public static CardZone HiddenPlayZone; //Where cards go when played during a round.
        public static CardZone DeckZone; //Zone that holds the deck in its current state (not necessarily full list during a play round)

        public static CardZone ActiveVoucherZone; //Zone that holds currently active vouchers.
        public static CardZone CurrentlyActivatingConsumable; //Zone holding any consumable currently being activated.

        public static CardZone HiddenBlindAttributeZone; //Used for hidden "Jokers" used to implement effects of Boss blinds.

        public static void InitializeMainGameZones()
        {
            DeckZone = MakeBasicDeck();

            JokerZone = MakeJokerZone(); //TODO: Settings/varying joker space.
            ConsumableZone = MakeZone("Consumable", 2); //TODO: Settings/varying consumable space.
            TagZone = MakeZone("Tags");
            ActiveVoucherZone = new VoucherZone();

            DestructionZone = new DestroyCardZone();

            PreDestructionZone = MakeZone("PreDestroy");

            CurrentlyBeingPlayedZone = MakeZone("CurrentlyBeingPlayed");
            HiddenBlindAttributeZone = MakeJokerZone(1); //Has all the same attributes of jokers, blinds are effectively hidden jokers.
            HiddenBlindAttributeZone.Name = "Blinds";

            InitializePlayRoundZones();
            InitializeMarketRoundZones();
        }

        public static void InitializePlayRoundZones()
        {
            //TODO: Hand size change changes this zones size.
            HandZone = MakeHand(Globals.BaseHandSize);
            HandSize = Globals.BaseHandSize; //Yes, this is redundant with the above line. Live with it. It doesn't hurt anything.
            DiscardZone = MakeZone("Discard");
            HiddenPlayZone = MakeZone("Played");
        }

        public static void ClosePlayRound()
        {
            DeckZone.DrawUntilCapacityFrom(HandZone);
            DeckZone.DrawUntilCapacityFrom(HiddenPlayZone);
            DeckZone.DrawUntilCapacityFrom(DiscardZone);
            ShuffleDeck();
        }

        public static void ClosePackSelection()
        {
            foreach (Card card in PackOptionZone.Cards.ToList())
            {
                MarketOptionsManager.ReturnMarketItemFromZone(card, PackOptionZone);
            }

            DeckZone.DrawUntilCapacityFrom(HandZone);
            ShuffleDeck();
        }

        public static void InitializeMarketRoundZones()
        {
            MainMarketZone = MakeZone("MainMarket", Globals.BaseMainMarketCount);
            PackMarketZone = MakeZone("PacksMarket", Globals.BasePackMarketCount);
            VoucherMarketZone = MakeZone("VouchersMarket", Globals.BaseVoucherMarketCount);

            PackOptionZone = MakeZone("PackOptions");

            CurrentlyActivatingConsumable = MakeZone("CurrentConsumable");
        }

        public static void ShuffleDeck()
        {
            DeckZone.Shuffle();
        }

        public static void DrawHandful()
        {
            var redrawStart = new EngineRedrawArgs() { MyContext = new() { Context = EventContextType.DrawHandfulStarted} };
            EngineEventHandler.TriggerEvent(redrawStart);
            if (redrawStart.ForcedRedrawAmount < 0)
                HandZone.DrawUntilCapacityFrom(DeckZone);
            else
                HandZone.DrawXFrom(DeckZone, redrawStart.ForcedRedrawAmount, ignoreSpaceLimits: true);
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.DrawHandfulDone } });
        }

        public static void DrawXCardsToHand(int x)
        {
            HandZone.DrawXFrom(DeckZone, x);
        }

        public static void ClearOutPlayZone()
        {
            var cList = CurrentlyBeingPlayedZone.Cards.ToList();
            foreach (var c in cList)
            {
                HiddenPlayZone.DrawTargetFrom(CurrentlyBeingPlayedZone, c);
                c.ForcedSelect = false;
                c.isSelected = false;
            }
        }

        public static void DiscardSelectedFromHand()
        {
            var selList = CardsSelectedInHand;
            foreach (var c in selList)
            {
                //Trigger event
                var args = new EngineCardDiscardedFromHandArgs
                {
                    CardBeingDiscarded = c,
                    MyContext = new EventContext() { Context = EventContextType.CardDiscardedFromHand }
                };
                EngineEventHandler.TriggerEvent(args);

                DiscardZone.DrawTargetFrom(HandZone, c);
                c.ForcedSelect = false;
                c.ToggleSelect();
            }
        }

        public static CardZone MakeBasicDeck()
        {
            var ret = MakeZone("Deck");
            ret.AddCards(CardFactory.CardListFromDefString(EngineUtils.BasicDeckString, ","));

            return ret;
        }

        public static CardZone MakeHand(int handSize)
        {
            return MakeZone("Hand", handSize);
        }

        public static JokerZone MakeJokerZone(int numSlots = 5)
        {
            return new JokerZone(numSlots);
        }

        public static CardZone MakeZone(string name, int capacity = -1)
        {
            var ret = new CardZone();
            ret.Name = name;
            ret.MaxCapacity = capacity;

            return ret;
        }

        public static void SortZoneByRank(CardZone zone)
        {
            zone.Cards = zone.Cards.OrderByDescending(x => x.Rank).ThenBy(y => y.Suit).ToList();
        }

        public static void SortZoneBySuit(CardZone zone)
        {
            zone.Cards = zone.Cards.OrderByDescending(x => x.Suit).ThenBy(y => y.Rank).ToList();
        }

        public static void DestroyCard(Card c, CardZone fromZone)
        {
            if(c.IsDestructible)
                DestructionZone.DrawTargetFrom(fromZone, c);
        }

        public static void DestroyCard(Card c)
        {
            DestroyCard(c, c.MyZone);
        }

        public static void DeleteCard(Card c)
        {
            if (c.MyZone != null)
                c.MyZone.RemoveCard(c);
        }
        public static List<Card> GetFullDeckCards()
        {
            var ret = new List<Card>();
            ret.AddRange(DeckZone.Cards);
            ret.AddRange(HandZone.Cards);
            ret.AddRange(DiscardZone.Cards);
            ret.AddRange(HiddenPlayZone.Cards);
            ret.AddRange(CurrentlyBeingPlayedZone.Cards);
            return ret.Distinct().ToList();
        }
        public static List<Card> GetFullDeckPlayingCards()
        {
            return GetFullDeckCards().Where(x => !x.isJoker && !x.isVoucher && !x.isConsumable && !x.isTag && !x.isPack).ToList();
        }

    }
}
