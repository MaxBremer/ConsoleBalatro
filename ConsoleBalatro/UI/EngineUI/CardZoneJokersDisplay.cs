using ConsoleBalatro.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public class CardZoneJokersDisplay : CardZoneDisplay
    {
        private const int NUM_X_LOC = 0;
        private const int NUM_Y_LOC = 5;
        public CardZoneJokersDisplay(CardZone zone) : base(zone, EngineDisplayConstants.JOKER_DISPLAY_HEIGHT, EngineDisplayConstants.JOKER_DISPLAY_WIDTH)
        {
            xLoc = EngineDisplayConstants.JOKER_DISPLAY_XLOC;
            yLoc = EngineDisplayConstants.JOKER_DISPLAY_YLOC;
        }

        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();

            var contentStr = CardList.Count.ToString() + " / " + MyCardZone.MaxCapacity.ToString();
            var min_width = contentStr.Length;

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
