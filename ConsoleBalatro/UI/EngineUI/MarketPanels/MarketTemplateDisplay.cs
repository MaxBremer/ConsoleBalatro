using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
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
            var listener = new EngineEventListener() { MyAction = UpdatePrice, MyContextType = EventContextType.CardDetailsChange };
            EngineEventHandler.StartListening(listener);
        }

        public override void AdditionalCardAddAction(Card c)
        {
            base.AdditionalCardAddAction(c);
            CardDisplays[c].PriceDisplay = c.BuyCost;
            CardDisplays[c].PreDisplaySetup();
        }

        private void RefreshPriceDisplay()
        {
            foreach(var c in CardList)
            {
                CardDisplays[c].PriceDisplay = c.BuyCost;
                CardDisplays[c].PreDisplaySetup();
            }
        }

        private void UpdatePrice(EngineEventArgs args)
        {
            if(args is EngineCardDetailsChangeArgs detArgs && detArgs.CardBeingChanged != null && CardList.Contains(detArgs.CardBeingChanged))
            {
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    RefreshPriceDisplay();
                });
            }
        }
    }
}
