using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineTagAddedEventArgs : EngineEventArgs
    {
        public Card TagCard;
        public bool isPostAdd = false;
    }
}
