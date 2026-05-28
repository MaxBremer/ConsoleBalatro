using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCardDetailsChangeArgs : EngineEventArgs
    {
        public Card CardBeingChanged;

        public bool isEditionChange = false;
        public Edition OldEdition;
        public Edition NewEdition;

        public bool isEnhancementChange = false;
        public Enhancement OldEnhancement;
        public Enhancement NewEnhancement;

        public bool isSuitChange = false;
        public Suit OldSuit;
        public Suit NewSuit;

        public bool isRankChange = false;
        public Rank OldRank;
        public Rank NewRank;

        public bool isFlip = false;
        public bool newFlipVal = false;

        public bool isAfter = false;
    }
}
