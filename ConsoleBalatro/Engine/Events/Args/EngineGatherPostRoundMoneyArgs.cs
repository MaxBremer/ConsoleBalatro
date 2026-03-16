using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineGatherPostRoundMoneyArgs : EngineEventArgs
    {
        public int TotalAmount = 0;
        public List<(JokerCardDataBlock, int)> JokersContributed = new();
    }
}
