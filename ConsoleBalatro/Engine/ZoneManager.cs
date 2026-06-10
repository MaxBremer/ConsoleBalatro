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
                List<Card> overall = [.. CardsSelectedInHand, .. JokersSelectedInZone, .. ConsumablesSelectedInZone];
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
        public static DestroyCardZone DestructionZone; //Where destroyed cards go, deletes them. Don't put anything here u want to see ever again.
        public static CardZone HiddenPlayZone; //Where cards go when played during a round.
        public static CardZone DeckZone; //Zone that holds the deck in its current state (not necessarily full list during a play round)

        public static CardZone ActiveVoucherZone; //Zone that holds currently active vouchers.
        public static CardZone CurrentlyActivatingConsumable; //Zone holding any consumable currently being activated.

        public static CardZone HiddenBlindAttributeZone; //Used for hidden "Jokers" used to implement effects of Boss blinds.
        //NOTE: The following should only be used for PERMANENT effects, staying across the entire run after being added.
        public static CardZone OtherHiddenJokerZone; //Used for other hidden effects; deck abilities, stake effects, challenge effects, etc.

        /// <summary>
        /// Initialize all CardZones needed for a run.
        /// </summary>
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
            OtherHiddenJokerZone = MakeJokerZone(-1);
            OtherHiddenJokerZone.Name = "Permanent Effects";

            InitializePlayRoundZones();
            InitializeMarketRoundZones();
        }

        private static void InitializePlayRoundZones()
        {
            //XTOXDOX: Hand size change changes this zones size.
            //Ignore above; BASE hand size is only what it starts at. 
            //HandSize is a field in both Globals and ZoneManager that does directly pull/set the MaxCapacity of handzone.
            HandZone = MakeZone("Hand", Globals.BaseHandSize);
            HandSize = Globals.BaseHandSize; //Yes, this is redundant with the above line. Live with it. It doesn't hurt anything.
            DiscardZone = MakeZone("Discard");
            HiddenPlayZone = MakeZone("Played");
        }

        /// <summary>
        /// Zone-related actions taken when a Play round is closed.
        /// </summary>
        public static void ClosePlayRound()
        {
            //Return all cards played, discarded, or currently in hand to the deck.
            DeckZone.DrawUntilCapacityFrom(HandZone);
            DeckZone.DrawUntilCapacityFrom(HiddenPlayZone);
            DeckZone.DrawUntilCapacityFrom(DiscardZone);
            ShuffleDeck();
        }

        /// <summary>
        /// Zone-related actions taken when the pack selection round is closed.
        /// </summary>
        public static void ClosePackSelection()
        {
            foreach (Card card in PackOptionZone.Cards.ToList())
            {
                MarketOptionsManager.ReturnMarketItemFromZone(card, PackOptionZone);
            }

            DeckZone.DrawUntilCapacityFrom(HandZone);
            ShuffleDeck();
        }

        private static void InitializeMarketRoundZones()
        {
            MainMarketZone = MakeZone("MainMarket", Globals.BaseMainMarketCount);
            PackMarketZone = MakeZone("PacksMarket", Globals.BasePackMarketCount);
            VoucherMarketZone = MakeZone("VouchersMarket", Globals.BaseVoucherMarketCount);

            PackOptionZone = MakeZone("PackOptions");

            CurrentlyActivatingConsumable = MakeZone("CurrentConsumable");
        }

        /// <summary>
        /// Shuffle the DeckZone... it's pretty intuitive from the name.
        /// </summary>
        public static void ShuffleDeck()
        {
            DeckZone.Shuffle();
        }

        /// <summary>
        /// Attempt to redraw the players hand, as in at the start of round, after a discard, or after a play.
        /// </summary>
        public static void DrawHandful()
        {
            var redrawStart = new EngineRedrawArgs() { MyContext = new() { Context = EventContextType.DrawHandfulStarted} };
            EngineEventHandler.TriggerEvent(redrawStart);
            //Listeners (just Serpent for now) can set a "Forced redraw amount" which means that on this drawHandful attempt instead of redrawing to full,
            //we will draw that set amount to hand, ignoring MaxCapacity entirely.
            if (redrawStart.ForcedRedrawAmount < 0)
                HandZone.DrawUntilCapacityFrom(DeckZone);
            else
                HandZone.DrawXFrom(DeckZone, redrawStart.ForcedRedrawAmount, ignoreSpaceLimits: true);
            EngineEventHandler.TriggerEvent(new EngineEventArgs() { MyContext = new() { Context = EventContextType.DrawHandfulDone } });
        }

        /// <summary>
        /// Helper func to empty the CurrentlyBeingPlayedZone once a hand play/scoring is done.
        /// </summary>
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
                //c.ToggleSelect();
                c.isSelected = false;
            }
        }

        public static CardZone MakeBasicDeck()
        {
            var ret = MakeZone("Deck");
            ret.AddCards(CardFactory.CardListFromDefString(EngineUtils.BasicDeckString, ","));

            return ret;
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
            zone.Cards = zone.Cards.OrderByDescending(x => x.Rank).ThenByDescending(y => y.Suit).ToList();
        }

        public static void SortZoneBySuit(CardZone zone)
        {
            zone.Cards = zone.Cards.OrderByDescending(x => x.Suit).ThenByDescending(y => y.Rank).ToList();
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

        public static void AddHiddenEffect(Card c)
        {
            OtherHiddenJokerZone.AddCard(c);
        }

        /// <summary>
        /// Return the current "Full deck list"; that is, all cards in the current runs main deck, regardless of play status in this round.
        /// Used in jokers/effects that care about the full deck.
        /// </summary>
        /// <returns>A list of all cards in the runs full deck.</returns>
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

        /// <summary>
        /// A version of GetFullDeckCards that intentionally filters out non-playing cards.
        /// In case a consumable/joker/whatever ended up in the deck... somehow.
        /// </summary>
        /// <returns>A list of all PLAYING cards in the runs full deck.</returns>
        public static List<Card> GetFullDeckPlayingCards()
        {
            return GetFullDeckCards().Where(x => !x.isJoker && !x.isVoucher && !x.isConsumable && !x.isTag && !x.isPack).ToList();
        }

    }
}
