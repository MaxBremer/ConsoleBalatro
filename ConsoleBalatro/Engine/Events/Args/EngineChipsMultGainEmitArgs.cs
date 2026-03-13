using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineChipsMultGainEmitArgs : EngineEventArgs
    {
        public Card SourceOfEmit;
        public int ChipsGainEmitted = -1;
        public double MultGainEmitted = -1;
        public double MultMultEmitted = -1;
    }
}
