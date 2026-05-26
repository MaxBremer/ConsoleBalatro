using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineSelectionTogglingArgs : EngineEventArgs
    {
        public Card CardThatIsToggling { get; set; }
        public bool OldSelectionState { get; set; }
        public bool NewSelectionState => !OldSelectionState;
        public bool CancelToggling { get; set; } = false;
    }
}
