using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineAchievementUnlockArgs : EngineEventArgs
    {
        public EngineAchievementUnlockArgs()
        {
            if (MyContext == null)
                MyContext = new();
            MyContext.Context = EventContextType.AchievementUnlocked;
        }
        public string AchievementId;
        public string AchievementName;
        public string AchievementDesc;
    }
}
