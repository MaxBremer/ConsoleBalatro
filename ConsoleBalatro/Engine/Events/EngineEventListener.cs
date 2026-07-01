using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events
{
    public class EngineEventListener
    {
        public virtual void Trigger(EngineEventArgs args)
        {
            MyAction(args);
        }

        public Action<EngineEventArgs> MyAction;
        public EventContextType MyContextType = EventContextType.NONE;

        public bool RemoveAfterTriggering = false;

        /// <summary>
        /// Indicates whether this listener is tied to the game engine or not. If not it's for the UI or controls or something, so should not be cleared when the engine is reset.
        /// </summary>
        public bool NonEngineListener = false;
    }
}
