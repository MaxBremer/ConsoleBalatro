using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCardTriggerArgs : EngineEventArgs
    {
        public Card CardThatIsTriggering;
        public PlayedHandType HandCurrentlyBeingPlayed;
        public bool isScoringTrigger = false;
        public bool isInHandPostScoringTrigger = false;
    }
}
