using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI.Animation
{
    public class AnimationFrame
    {
        public Action<AnimationFrameArgs> MyAction;
        public int MyFrameDelay = -1;
    }
}
