using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EnginePreFinalGainArgs : EngineEventArgs
    {
        public int FinalChips { get; set; }
        public double FinalMult { get; set; }
    }
}
