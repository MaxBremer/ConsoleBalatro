using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineHandLevelChangeArgs : EngineEventArgs
    {
        public PlayedHandType HandTypeLevelling { get; set; }
        public bool isLevelUp { get; set; }

        public int oldChipAmount { get; set; }
        public int newChipAmount { get; set; }
        public int chipChangeAmount { get; set; }

        public double oldMultAmount { get; set; }
        public double newMultAmount { get; set; }
        public double multChangeAmount { get; set; }

        public int oldLevel { get; set; }
        public int newLevel { get; set; }
    }
}
