using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineHandDiscChangeArgs : EngineEventArgs
    {
        public bool isHand;
        public int oldVal;
        public int newVal;
    }
}
