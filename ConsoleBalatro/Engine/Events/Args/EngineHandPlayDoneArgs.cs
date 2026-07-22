using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineHandPlayDoneArgs : EngineEventArgs
    {
        public PlayedHandType HandTypeThatWasPlayed { get; set; }
        public List<Card> CardsInPlayedHand { get; set; }
        public List<Card> CardsHeldInHand { get; set; }
        public BigInteger CurrentTotalChips { get; set; }
        public BigInteger RequiredChipsForBlind { get; set; }
        public bool PreventGameOverAndWinBlind { get; set; } = false;

    }
}
