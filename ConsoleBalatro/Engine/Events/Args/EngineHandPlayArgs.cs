using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineHandPlayArgs : EngineEventArgs
    {
        public bool PreHandTypeCalculation = false;
        public PlayedHandType HandBeingPlayed;
        public List<Card> CardsSelected = new();
        public List<Card> CardsInScoringHand = new();

        public int BaseChipsForCalc;
        public double BaseMultForCalc;

        public bool CancelScoring = false;
    }
}
