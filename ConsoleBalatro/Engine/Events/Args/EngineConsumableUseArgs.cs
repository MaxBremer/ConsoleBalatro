using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineConsumableUseArgs : EngineEventArgs
    {
        public ConsumableType TypeUsed;
        public BuyItemType BuyItemUsed;
        public PlayedHandType HandOfItemUsed;
        public string ConsumableName;
        public string ConsumableDBName;
    }
}
