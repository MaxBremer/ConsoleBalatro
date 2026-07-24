using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineVoucherRedeemedArgs : EngineEventArgs
    {
        public EngineVoucherRedeemedArgs()
        {
            if (MyContext == null)
                MyContext = new() { Context = EventContextType.VoucherRedeemed };
        }

        public Card BeingRedeemed { get; set; }
    }
}
