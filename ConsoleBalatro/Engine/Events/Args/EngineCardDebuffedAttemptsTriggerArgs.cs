using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    internal class EngineCardDebuffedAttemptsTriggerArgs : EngineEventArgs
    {
        public EngineCardDebuffedAttemptsTriggerArgs()
        {
            if (MyContext == null)
                MyContext = new() { Context = EventContextType.CardPurchased };
        }
    }
}
