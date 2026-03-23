using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public abstract class CardZoneDisplay : DisplayEntity
    {
        private EngineEventListener MyZoneChangeListener;
        public CardZoneDisplay(CardZone zone, int h, int w) : base(h, w)
        {
            MyCardZone = zone;
            MyZoneChangeListener = new EngineEventListener() { MyAction = OnCardZoneChangeAction };
            EngineEventHandler.StartListening(MyZoneChangeListener);
        }

        public CardZone MyCardZone;

        public List<Card> CardList = new();
        public Dictionary<Card, CardDisplay> CardDisplays = new();

        public string DisplayBeneath { get; set; } = "";

        public int DisplayXLoc => Sprite.GetLength(1) / 2;

        public virtual void Shutdown()
        {
            EngineEventHandler.StopListening(MyZoneChangeListener);
        }

        public virtual void MatchToZone()
        {
            if (MyCardZone == null)
                return;

            foreach (var c in MyCardZone.Cards.Where(x => !CardList.Contains(x)))
            {
                AddCard(c);
            }
        }

        public virtual void AddCard(Card c)
        {
            CardList.Add(c);
            var cd = new CardDisplay(c);
            cd.PreDisplaySetup();
            cd.AddListener();
            CardDisplays.Add(c, cd);
            EngineDisplayGlobals.GlobalCardDisplays.Add(c, cd);
            AdditionalCardAddAction(c);
            SetCardPositions();
        }

        public virtual void RemoveCard(Card c)
        {
            CardList.Remove(c);
            var cd = CardDisplays[c];
            cd.RemoveListener();
            CardDisplays.Remove(c);
            EngineDisplayGlobals.GlobalCardDisplays.Remove(c);
            AdditionalCardRemoveAction(c);
            SetCardPositions();
        }

        public virtual void AdditionalCardAddAction(Card c) { }
        public virtual void AdditionalCardRemoveAction(Card c) { }

        public virtual void SetCardPositions()
        {
            int curCount = 0;
            foreach (var c in CardList)
            {
                UpdateCardPosition(c, curCount);
                curCount++;
            }
        }

        public virtual void ResetFromZoneList()
        {
            CardList.Clear();
            CardList.AddRange(MyCardZone.Cards);
            SetCardPositions();
        }

        public virtual void UpdateCardPosition(Card c, int ind = -1)
        {
            int posInList = ind == -1 ? CardList.IndexOf(c) : ind;
            int ypos = GetCardYPos(c, posInList);
            int xpos = GetCardXPos(c, posInList);
            CardDisplays[c].xLoc = xpos;
            CardDisplays[c].yLoc = ypos;
            CardDisplays[c].GlobalX = xpos + xLoc;
            CardDisplays[c].GlobalY = ypos + yLoc;
        }

        public virtual int GetCardXPos(Card c, int listInd)
        {
            return listInd * (CardDisplay.CARD_WIDTH + 1);
        }
        public virtual int GetCardYPos(Card c, int listInd)
        {
            return 0;
        }

        protected void ApplyDisplayChar()
        {
            if(Sprite.GetLength(0) > 0 && Sprite.GetLength(1) > 0 && !string.IsNullOrEmpty(DisplayBeneath))
            {
                Sprite[Sprite.GetLength(0) - 1, DisplayXLoc] = DisplayBeneath;
            }
        }

        protected void OnCardZoneChangeAction(EngineEventArgs args)
        {
            if(args is EngineCardDrawnToZoneArgs cDArgs && args.MyContext.Context == EventContextType.CardDrawnToZone && cDArgs.ZoneDrawnTo == MyCardZone)
            {
                /*AddCard(cDArgs.CardBeingDrawn);
                EngineDisplayGlobals.Redraw();*/
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    AddCard(cDArgs.CardBeingDrawn);
                }, 150);
            }
            else if(args is EngineCardDiscardedFromZoneArgs cDiscArgs && args.MyContext.Context == EventContextType.CardDiscarded && cDiscArgs.ZoneCardIsLeaving == MyCardZone)
            {
                /*RemoveCard(cDiscArgs.CardBeingDiscarded);
                EngineDisplayGlobals.Redraw();*/
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    RemoveCard(cDiscArgs.CardBeingDiscarded);
                }, 150);
            }
        }
    }
}
