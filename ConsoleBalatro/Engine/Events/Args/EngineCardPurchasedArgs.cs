using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCardPurchasedArgs : EngineEventArgs
    {
        public EngineCardPurchasedArgs()
        {
            if (MyContext == null)
                MyContext = new() { Context = EventContextType.CardPurchased };
        }
        public Card BeingPurchased { get; set; }
        public CardZone? ZoneGoingTo { get; set; }
        public int AmountPaid { get; set; }
    }
}
