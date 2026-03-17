using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCardSoldArgs : EngineEventArgs
    {
        public Card CardBeingSold;
        public CardZone ZoneCardIsLeaving;
        public int ValueBeingSoldFor;
    }
}
