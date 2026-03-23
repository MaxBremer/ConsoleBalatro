using ConsoleBalatro.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI.MarketPanels
{
    public class CardZoneTemplateDisplay : CardZoneDisplay
    {
        private bool show_count;
        public CardZoneTemplateDisplay(CardZone zone, int h, int w, int xpos, int ypos, bool showCount) : base(zone, h, w)
        {
            show_count = showCount;
            xLoc = xpos;
            yLoc = ypos;
        }

        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();

            var desiredWidth = CardList.Count * (CardDisplay.CARD_WIDTH + 1);
            if(desiredWidth != Width)
            {
                Sprite = new string[7, desiredWidth];
            }

            FillWithClear();

            foreach(var c in CardList)
            {
                InsertOtherEntity(CardDisplays[c].xLoc, CardDisplays[c].yLoc, CardDisplays[c]);
            }

            ApplyDisplayChar();
        }
    }
}
