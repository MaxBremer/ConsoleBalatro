using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCardPriceGetArgs : EngineEventArgs
    {
        public EngineCardPriceGetArgs()
        {
            if (MyContext == null)
                MyContext = new EventContext() { Context = EventContextType.CardPriceGet };
        }

        public int PriceToReturn;
        public Card CardWhichPriceGetIsFor;
    }
}
