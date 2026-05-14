using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineRandomRollArgs : EngineEventArgs
    {
        public Card CardThatIsRolling;
        public bool? OverrideResult = null;
        public int Numerator;
        public int Denominator;
    }
}
