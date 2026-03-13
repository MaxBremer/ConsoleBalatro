using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCardPreTriggerArgs : EngineEventArgs
    {
        public Card CardAboutToTrigger;
        public int numTriggersToDo = 1;
        public bool isInHandPostScoringTrigger = false;
    }
}
