using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineCollectionItemAddArgs : EngineEventArgs
    {
        public EngineCollectionItemAddArgs() 
        {
            if (MyContext == null)
                MyContext = new();
            MyContext.Context = EventContextType.CollectionItemAdded;
        }

        public string ItemDbName { get; set; }
    }
}
