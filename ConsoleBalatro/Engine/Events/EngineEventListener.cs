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
    }
}
