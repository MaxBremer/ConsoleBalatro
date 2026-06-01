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
        public List<(JokerCardDataBlock, int)> JokersContributed = new();
        public List<(string, int)> ExistingSources = new();
    }
}
