using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCardSelectedArgs : EngineEventArgs
    {
        public Card TargetedCard;
        public bool isNowSelected;
        public bool wasPreviouslySelected;//... are you stupid?? Can... can you not just invert prev bool for this?
        //apparently not cause I'm also tracking whether it gets set to the same value as before... idk when that will be useful, but maybe.
    }
}
