using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Pools;
using ConsoleBalatro.Engine.Pools.Rollables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCardRollGeneratedArgs : EngineEventArgs
    {
        public EngineCardRollGeneratedArgs() 
        {
            if (MyContext == null)
                MyContext = new();
            MyContext.Context = EventContextType.RolledCardGenerated;
        }

        public ContentRollRequest RollRequest { get; set; }

        public RollableDefinition RollMade { get; set; }

        public Card FinalCardRolled { get; set; }
    }
}
