using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineSettingBaseHandScoreArgs : EngineEventArgs
    {
        public BigInteger BaseChipAmount { get; set; }
        public double BaseMultAmount { get; set; }
    }
}
