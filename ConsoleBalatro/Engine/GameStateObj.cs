using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public class GameStateObj
    {
        public GameState GameState;

        //Pack-opening-related fields
        public Card TargetPack = null;
        public int NumChoicesAlreadyMade = 0;

        //Used by any state that gives money (rn just post-roudn)
        public int PostRoundMoneyToGive = 0;
        public List<(string, int)> PostRoundMoneySources = new();

        public string SavedHandOrder;
        public string SavedDeckOrder;
        public string SavedDiscardOrder;
        public string SavedHiddenPlayOrder;
    }
}
