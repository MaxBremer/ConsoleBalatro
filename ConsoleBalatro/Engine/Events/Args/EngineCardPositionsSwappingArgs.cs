using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCardPositionsSwappingArgs : EngineEventArgs
    {
        public Card Card1 { get; set; }
        public int Card1OldIndex { get; set; }
        public int Card1NewIndex => Card2OldIndex;

        public Card Card2 { get; set; }
        public int Card2OldIndex { get; set; }
        public int Card2NewIndex => Card1OldIndex;

        public CardZone ZoneOfSwap { get; set; }
    }
}
