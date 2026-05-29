using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleBalatro.UI.EngineUI.Controls.ControlManager;

namespace ConsoleBalatro.UI.EngineUI.Controls
{
    public class ControlOptionset
    {
        public string SchemaName;
        public Dictionary<ConsoleKey, Action<ControlContext>> AvailableActions = new();
        public LookZonesAvailable ZonesAvailable;
    }
}
