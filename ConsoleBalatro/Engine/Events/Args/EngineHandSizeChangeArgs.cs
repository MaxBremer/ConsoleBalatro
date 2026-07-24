using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineHandSizeChangeArgs : EngineEventArgs
    {
        public EngineHandSizeChangeArgs()
        {
            if (MyContext == null)
                MyContext = new() { Context = EventContextType.HandSizeChanged };
        }

        public int OldHandSize { get; set; }
        public int NewHandSize { get; set; }
    }
}
