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
    public class BlindDisplayEntity : TextDisplayPanel
    {
        private static Dictionary<BlindType, string> BLIND_NAMES = new()
        {
            { BlindType.SMALL, "Small Blind" },
            { BlindType.BIG, "Big Blind" },
            { BlindType.BOSS, "BOSS Blind" },
        };

        public BlindDisplayEntity(BlindType myBlind, int xloc, int yloc) : base(new List<string>(), EngineDisplayConstants.BLINDDISPLAY_HEIGHT, EngineDisplayConstants.BLINDDISPLAY_WIDTH)
        {
            xLoc = xloc;
            yLoc = yloc;
            MyBlind = myBlind;
            ResetTextLines();
            var list = new EngineEventListener();
            list.MyContextType = EventContextType.BlindChange;
            list.MyAction = BlindChangeDone;
            list.NonEngineListener = true;
            EngineEventHandler.StartListening(list);
        }

        public BlindType MyBlind { get; set; }

        private List<string> GetMyTextLines()
        {
            var ret = new List<string>();

            ret.Add("");
            ret.Add("");
            ret.Add("");

            ret.Add(BLIND_NAMES[MyBlind]);
            if(MyBlind == BlindType.BOSS && !string.IsNullOrEmpty(FlowHandler.CurrentBossBlind))
            {
                ret.Add(FlowHandler.CurrentBossBlind);
            }
            ret.Add("");
            ret.Add("Chips:");
            ret.Add(Globals.FormatChipCount(FlowHandler.GetChipsForBlindType(MyBlind)));

            if(MyBlind != BlindType.BOSS)
            {
                ret.Add("");
                ret.Add("");
                ret.Add("");
                ret.Add("TAG:");
                var tagStr = MyBlind == BlindType.SMALL ? FlowHandler.CurSmallBlindTag.ToString() : FlowHandler.CurBigBlindTag.ToString();
                ret.Add(tagStr.Length > 11 ? tagStr.Substring(0, 10) : tagStr);
            }

            return ret;
        }

        private void ResetTextLines()
        {
            _textLines.Clear();
            _textLines.AddRange(GetMyTextLines());
        }

        private void ResetCapChars()
        {
            if(FlowHandler.CurrentSelectedBlind == MyBlind)
            {
                TopChar = "^";
            }
            else
            {
                TopChar = "-";
            }
        }

        private void BlindChangeDone(EngineEventArgs args)
        {
            if(args is EngineBlindChangeEventArgs)
            {
                PreDisplaySetup();
                EngineDisplayGlobals.CacheRedraw();
            }
        }

        public override void PreDisplaySetup()
        {
            ResetTextLines();
            ResetCapChars();
            base.PreDisplaySetup();
        }
    }
}
