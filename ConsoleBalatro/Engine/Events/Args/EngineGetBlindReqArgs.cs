using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineGetBlindReqArgs : EngineEventArgs
    {
        public int ChipRequirementAmount { get; set; }
    }
}
