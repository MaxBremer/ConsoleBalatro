using ConsoleBalatro.Engine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Tags
{
    public class TagDataBlock
    {
        public static int IDCount = 1;
        public TagDataBlock()
        {
            MyTagID = IDCount;
            IDCount++;
        }
        public int MyTagID;
        public TagType MyType;
        public bool ImmuneToDouble = false;

        public List<EventContextType> EventTypesTrigger = new();
        public Func<EngineEventArgs, Card, bool> DoTrigger = (_, _) => true; //Defaults to always true
        public Action<EngineEventArgs> Activate = _ => { };
        public Action<EngineEventArgs?> OnAddAction = null;
    }
}
