using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineSettingBaseHandScoreArgs : EngineEventArgs
    {
        public int BaseChipAmount { get; set; }
        public double BaseMultAmount { get; set; }
    }
}
