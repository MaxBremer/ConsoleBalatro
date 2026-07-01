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
    public class CardZoneHandDisplay : CardZoneDisplay
    {
        //TWO SLOTS FOR raise-for-selection, 7 extra to make room for info box above the card
        public const int CARD_Y_OFFSET_POS = 2 + 7;

        public CardZoneHandDisplay(CardZone zone) : base(zone, CardDisplay.CARD_HEIGHT + CARD_Y_OFFSET_POS + 1, Interface.Display_Width)
        {
            yLoc = Interface.Display_Height - (CardDisplay.CARD_HEIGHT + CARD_Y_OFFSET_POS + 1);
            xLoc = EngineDisplayConstants.HAND_DISPLAY_XLOC;

            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnSelectAction, MyContextType = EventContextType.CardSelect, NonEngineListener = true });
        }

        public Dictionary<Card, bool> CardIsSelectedVisually = new();

        public Dictionary<Card, AboveCardDisplay> CardInfoBoxes = new();

        public override void AdditionalCardAddAction(Card c)
        {
            base.AdditionalCardAddAction(c);
            CardIsSelectedVisually.Add(c, false);
        }
        public override void AdditionalCardRemoveAction(Card c)
        {
            base.AdditionalCardRemoveAction(c);
            CardIsSelectedVisually.Remove(c);
        }

        public override int GetCardYPos(Card c, int listInd)
        {
            return CardIsSelectedVisually[c] ? CARD_Y_OFFSET_POS - 2 : CARD_Y_OFFSET_POS;
        }

        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();

            var desiredWidth = CardDisplays.Keys.Count * (CardDisplay.CARD_WIDTH + 1);
            if(desiredWidth != Sprite.GetLength(1))
            {
                Sprite = new string[CardDisplay.CARD_HEIGHT + CARD_Y_OFFSET_POS + 1, desiredWidth];
            }
            //prefill entity with clear tile indicator
            FillWithClear();
            int curCount = 0;
            foreach (var c in CardList)
            {
                InsertOtherEntity(CardDisplays[c].xLoc, CardDisplays[c].yLoc, CardDisplays[c]);

                curCount++;
            }
            ApplyDisplayChar();
        }

        private void OnSelectAction(EngineEventArgs args)
        {
            if(args.MyContext.Context == EventContextType.CardSelect && args is EngineCardSelectedArgs selArgs && ZoneManager.HandZone.Cards.Contains(selArgs.TargetedCard))
            {
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    CardIsSelectedVisually[selArgs.TargetedCard] = selArgs.isNowSelected;
                    UpdateCardPosition(selArgs.TargetedCard);
                }, 150);
            }
        }
    }
}
