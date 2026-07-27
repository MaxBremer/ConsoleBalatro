using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public static class UIStateManager
    {
        public static Stack<GameStateObj> UIGameStateTracker;

        public static void InitializeUIStateManager()
        {
            UIGameStateTracker = new Stack<GameStateObj>();
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnGameStatePop, MyContextType = EventContextType.GameStatePop, NonEngineListener = true });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnGameStatePush, MyContextType = EventContextType.GameStatePush, NonEngineListener = true });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnGameStateReplace, MyContextType = EventContextType.GameStateReplace, NonEngineListener = true });
        }

        //TODO: These exist so that, if needed, I can animate this.
        public static void OnGameStatePop(EngineEventArgs args)
        {
            if(args is EngineGameStateChangeArgs gsArgs && gsArgs.isPop)
            {
                if (UIGameStateTracker.Count > 0)
                    UIGameStateTracker.Pop();
            }
        }

        public static void OnGameStatePush(EngineEventArgs args)
        {
            if(args is EngineGameStateChangeArgs gsArgs && gsArgs.isPush)
            {
                UIGameStateTracker.Push(gsArgs.NewStateBeingPushed);
            }
        }

        public static void OnGameStateReplace(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs gsArgs && gsArgs.isReplace)
            {
                if (UIGameStateTracker.Count > 0)
                    UIGameStateTracker.Pop();
                UIGameStateTracker.Push(gsArgs.NewStateBeingPushed);
            }
        }
    }
}
