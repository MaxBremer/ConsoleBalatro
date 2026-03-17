using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCardSuitPullArgs : EngineEventArgs
    {
        public Card CardBeingPulled;
        public List<Suit> SuitsBeingReturned;
    }
}
