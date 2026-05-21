using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineMarketTypeBeingChosenArgs : EngineEventArgs
    {
        public Dictionary<BuyItemType, int> WeightsBeingRolled { get; set; }
    }
}
