using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineHandPlayDoneArgs : EngineEventArgs
    {
        public PlayedHandType HandTypeThatWasPlayed { get; set; }
        public List<Card> CardsInPlayedHand { get; set; }
        public List<Card> CardsHeldInHand { get; set; }
        public int CurrentTotalChips { get; set; }
        public int RequiredChipsForBlind { get; set; }
        public bool PreventGameOverAndWinBlind { get; set; } = false;

    }
}
