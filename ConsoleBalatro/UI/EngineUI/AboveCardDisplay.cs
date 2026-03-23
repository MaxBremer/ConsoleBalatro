using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public class AboveCardDisplay : TextDisplayPanel
    {
        private CardDisplay _myCardDisplay;

        public bool Display = false;
        public AboveCardDisplay(CardDisplay cd) : base(new List<string>(), cd.Width, 7)
        {
            _myCardDisplay = cd;
        }

        public int OverrideXLoc = -1;
        public int OverrideYLoc = -1;

        public List<string> MyOverrideLines = new();

        public override void PreDisplaySetup()
        {
            //For now, if card isn't selected, this shouldn't display.
            //Above is no longer relevant.
            if (!Display)
            {
                Sprite = new string[0, 0];
                return;
            }
            xLoc = OverrideXLoc >= 0 ? OverrideXLoc : _myCardDisplay.GlobalX;
            yLoc = OverrideYLoc >= 0 ? OverrideYLoc : _myCardDisplay.GlobalY - 7;

            Lines.Clear();
            if(MyOverrideLines.Count > 0)
            {
                Lines.AddRange(MyOverrideLines);
            }
            else
            {
                Lines.AddRange(_myCardDisplay.MyCard.PlayingCardBasicDisplay().Split("\n"));
            }
            base.PreDisplaySetup();
        }
    }
}
