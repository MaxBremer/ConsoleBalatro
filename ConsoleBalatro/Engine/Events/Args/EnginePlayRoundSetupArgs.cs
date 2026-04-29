using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EnginePlayRoundSetupArgs : EngineEventArgs
    {
        public int TempHandSizeBonus = 0;

        //Returns whether any changes at all happen.
        public bool AnyBuffsApplied()
        {
            return TempHandSizeBonus != 0;
        }
    }
}
