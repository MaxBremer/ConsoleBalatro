using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineNewAnteArgs : EngineEventArgs
    {
        public int OldAnteVal;
        public int NewAnteVal;
    }
}
