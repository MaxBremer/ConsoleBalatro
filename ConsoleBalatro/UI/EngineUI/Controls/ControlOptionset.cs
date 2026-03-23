using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI.Controls
{
    public class ControlOptionset
    {
        public Dictionary<ConsoleKey, Action<ControlContext>> AvailableActions = new();
    }
}
