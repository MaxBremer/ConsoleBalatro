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
        public CardZoneBeingPlayedDisplay(CardZone zone) : base(zone, 7, 37)//TODO: CONST INT AT LEAST
        {
            xLoc = 20;
            yLoc = 6;
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnScoringHandCalced, MyContextType = EventContextType.HandPlayedCalculated });
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
