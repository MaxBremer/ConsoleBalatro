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
        public bool isScoringPreTrigger => !isInHandPostScoringTrigger;

        //Yeah yeah this is kinda stupid to include here for like one case.
        //suck my nards.
        public int CurrentAnte;
    }
}
