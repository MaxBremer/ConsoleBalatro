using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineDiscardAvailabilityCheckArgs : EngineEventArgs
    {
        public EngineDiscardAvailabilityCheckArgs()
        {
            if (MyContext == null)
                MyContext = new() { Context = EventContextType.DiscardAvailabilityCheck };
        }
        public bool OriginalIntendedValue { get; set; }
        public bool CurrentValue { get; set; }
    }
}
