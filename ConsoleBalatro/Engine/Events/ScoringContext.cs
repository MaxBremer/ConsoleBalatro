using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events
{
    public class ScoringContext
    {
        public PlayedHandType HandBeingPlayed;
        public List<Card> PlayingCardsBeingScored = new();
        public List<Card> AllPlayingCardsSubmittedForHand = new();
    }
}
