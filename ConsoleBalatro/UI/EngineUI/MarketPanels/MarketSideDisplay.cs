using ConsoleBalatro.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI.MarketPanels
{
    public class MarketSideDisplay : TextDisplayPanel
    {
        public MarketSideDisplay() : base(new List<string>(), EngineDisplayConstants.MARKETSIDEDISPLAY_WIDTH_MIN, EngineDisplayConstants.MARKETSIDEDISPLAY_HEIGHT_MIN)
        {
            ResetTextLines();
        }

        public bool IsBlindSide { get; set; }

        //Options: MARKET, BLIND, PACK
        public string SideImDisplaying { get; set; } = "MARKET";

        private List<string> GetMyTextLines()
        {
            var ret = new List<string>
            {
                "",
                "",
                "MONEY",
                EngineDisplayGlobals.DisplayMoney.ToString() + "$"
            };

            if(SideImDisplaying == "MARKET")
            {
                //Market-only displays.
                ret.Add("");
                ret.Add("REROLL (" + Globals.CurrentRerollCost + ")");//TODO: Don't think I need a val shell for this? No animation? But maybe in future.
                ret.Add("[R]");

                ret.Add("");
                ret.Add("END MARKET");
                ret.Add("[E]");
            }else if(SideImDisplaying == "BLIND")
            {
                //Blind-only displays.
                ret.Add("");
                ret.Add("SKIP BLIND");
                ret.Add("[S]");

                ret.Add("");
                ret.Add("START BLIND");
                ret.Add("[B]");
            }
            else
            {
                //Pack-only displays
                ret.Add("");
                ret.Add("SKIP PACK");
                ret.Add("[S]");
            }

            return ret;
        }

        private void ResetTextLines()
        {
            _textLines.Clear();
            _textLines.AddRange(GetMyTextLines());
        }

        public override void PreDisplaySetup()
        {
            ResetTextLines();
            base.PreDisplaySetup();
        }
    }
}
