using ConsoleBalatro.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public class CardZoneConsumableDisplay : CardZoneDisplay
    {
        private const int NUM_X_LOC = 0;
        private const int NUM_Y_LOC = 5;
        private int min_width = 0;
        public CardZoneConsumableDisplay(CardZone zone) : base(zone, 5 + 1, 30)
        {
            xLoc = 70;
            yLoc = 0;
        }
        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();

            var contentStr = CardList.Count.ToString() + " / " + MyCardZone.MaxCapacity.ToString();
            min_width = contentStr.Length;

            var desiredWidth = Math.Max(CardList.Count * (CardDisplay.CARD_WIDTH + 1), min_width);
            if(desiredWidth != Width)
            {
                Sprite = new string[7, desiredWidth];
            }

            FillWithClear();

            foreach (var c in CardList)
            {
                InsertOtherEntity(CardDisplays[c].xLoc, CardDisplays[c].yLoc, CardDisplays[c]);
            }
            InsertOtherStringDirect(NUM_X_LOC, NUM_Y_LOC, contentStr);
            ApplyDisplayChar();
        }
    }
}
