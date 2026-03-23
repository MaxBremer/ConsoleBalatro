using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI.Animation
{
    public class EngineActionAnimation
    {
        public List<AnimationFrame> FrameActions = new();
        public int GlobalFrameDelay = 1000;
        public void PerformAnimatedAction(AnimationArgs args, bool clearActionsAfter)
        {
            if (FrameActions.Count == 0)
                return;
            var lastAct = FrameActions.Last();
            foreach(var act in FrameActions)
            {
                //For now, no frame args are passed.
                //Later, pass args if needed.
                act.MyAction(null);
                EngineDisplayGlobals.Redraw();
                if(act != lastAct && act.MyFrameDelay != 0)
                {
                    var curDelay = act.MyFrameDelay == -1 ? GlobalFrameDelay : act.MyFrameDelay;
                    Thread.Sleep(curDelay);
                }
            }

            if (clearActionsAfter)
            {
                FrameActions.Clear();
            }
        }
    }
}
