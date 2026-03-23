using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI.MarketPanels
{
    public class MarketTemplateDisplay : CardZoneTemplateDisplay
    {
        public MarketTemplateDisplay(CardZone zone, int h, int w, int xpos, int ypos, bool showCount) : base(zone, h, w, xpos, ypos, showCount)
        {
        }

        public override void AdditionalCardAddAction(Card c)
        {
            base.AdditionalCardAddAction(c);
            CardDisplays[c].PriceDisplay = c.BuyCost;
            CardDisplays[c].PreDisplaySetup();
        }
    }
}
