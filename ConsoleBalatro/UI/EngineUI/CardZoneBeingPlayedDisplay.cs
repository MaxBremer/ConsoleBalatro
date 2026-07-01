using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public class CardZoneBeingPlayedDisplay : CardZoneDisplay
    {
        public CardZoneBeingPlayedDisplay(CardZone zone) : base(zone, EngineDisplayConstants.PLAYED_DISPLAY_HEIGHT, EngineDisplayConstants.PLAYED_DISPLAY_WIDTH)
        {
            xLoc = EngineDisplayConstants.PLAYED_DISPLAY_XLOC;
            yLoc = EngineDisplayConstants.PLAYED_DISPLAY_YLOC;
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnScoringHandCalced, MyContextType = EventContextType.HandPlayedCalculated, NonEngineListener = true });
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

            foreach (var c in CardList)
            {
                InsertOtherEntity(CardDisplays[c].xLoc, CardDisplays[c].yLoc, CardDisplays[c]);
            }

            ApplyDisplayChar();
        }

        private void OnScoringHandCalced(EngineEventArgs args)
        {
            if(args.MyContext.Context == EventContextType.HandPlayedCalculated && args is EngineHandPlayArgs handArgs)
            {
                foreach (var c in handArgs.CardsInScoringHand)
                {
                    EngineDisplayGlobals.CacheAnimationAction(_ =>
                    {
                        EngineDisplayGlobals.GlobalCardDisplays[c].SetDisplaySelectLevel(2);
                    }, 50);
                }
            }
        }
    }
}
