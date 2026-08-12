using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineGameStateChangeArgs : EngineEventArgs
    {
        //TODO: This is the stupidest most poorly-designed args of all time.
        //Like why are they separated? What was I thinking? idk...
        public bool isPush => NewStateBeingPushed != null;
        public bool isPop => OldStateToBePopped != null;
        public bool isReplace => isPush && isPop;
        public GameStateObj? NewStateBeingPushed = null;
        public GameStateObj? OldStatePushedOver = null;
        public GameStateObj? OldStateToBePopped = null;
        public GameStateObj? NewStateRevealedByPop = null;

        public bool StateChangeIsInterrupted = false; //More stupid. Don't do this. Jackass

        public bool isAfterStateChange = false; //See this here is how to do it dumbass.

        //See? All that work just to undo it all here. Why, Max? Why?
        public GameStateObj? OldState => isPop ? OldStateToBePopped : OldStatePushedOver;
        public GameStateObj? NewState => isPush ? NewStateBeingPushed : NewStateRevealedByPop;
    }
}
