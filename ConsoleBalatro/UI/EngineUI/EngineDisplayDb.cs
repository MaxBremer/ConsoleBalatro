using ConsoleBalatro.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public static class EngineDisplayDb
    {
        public class DisplayData
        {
            public int xLoc;
            public int yLoc;
            public int height;
            public int width;
            public string Name;
        }
        public static Dictionary<EngineDisplayType, DisplayData> TemplateData = new()
        {

        };

        public static Dictionary<GameState, List<EngineDisplayType>> DisplaysShownPerState = new()//TODO: This is a good idea, but not implemented yet. Replaces a lot of repeated code in EngineDisplayGlobals.
        {
            { GameState.PlayRound, new() {EngineDisplayType.BeingPlayed, EngineDisplayType.Jokers, EngineDisplayType.Consumables, EngineDisplayType.Hand} },
        };
    }
}
