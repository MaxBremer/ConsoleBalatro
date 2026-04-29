using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events
{
    public static class EngineEventHandler
    {
        private static List<EventContextType> TypesToSave = new List<EventContextType>()
        {
            EventContextType.ConsumableUsed,
            EventContextType.HandPlayDone,
            EventContextType.HandDiscardDone,
            EventContextType.BlindSkip,
        };

        public static List<EngineEventArgs> SavedEvents = new();

        public static List<EngineEventListener> GeneralListeners = new();
        public static Dictionary<EventContextType, List<EngineEventListener>> SpecificListeners = new();
        public static List<EngineEventListener> ToBeAdded = new();
        public static List<EngineEventListener> ToBeRemoved = new();
        public static int CallDepth = 0;

        public static void ResetSavedEvents()
        {
            SavedEvents.Clear();
        }

        public static EngineEventArgs LastSavedOfType(EventContextType evType)
        {
            return SavedEvents.LastOrDefault(x => x.MyContext.Context == evType);
        }

        public static EngineEventArgs LastSavedOfTypeConditional(EventContextType evType, Func<EngineEventArgs, bool> condition)
        {
            return SavedEvents.LastOrDefault(x => x.MyContext.Context == evType && condition(x));
        }

        public static int CountOfSaved(EventContextType evType)
        {
            return SavedEvents.Count(x => x.MyContext.Context == evType);
        }

        public static void StartListening(EngineEventListener listener)
        {
            if (CallDepth == 0)
            {
                //TODO: General listener set still exists for legacy stuff.
                //In perfect world for performance, everyone is a specific listener.
                if (listener.MyContextType == EventContextType.NONE)
                {
                    GeneralListeners.Add(listener);
                }
                else
                {
                    if (!SpecificListeners.ContainsKey(listener.MyContextType))
                    {
                        SpecificListeners[listener.MyContextType] = new List<EngineEventListener>();
                    }
                    SpecificListeners[listener.MyContextType].Add(listener);
                }
            }
            else
            {
                ToBeAdded.Add(listener);
            }
        }

        public static void StopListening(EngineEventListener listener)
        {
            if (CallDepth == 0)
            {
                if (listener.MyContextType == EventContextType.NONE && GeneralListeners.Contains(listener))
                {
                    GeneralListeners.Remove(listener);
                }
                if (SpecificListeners.ContainsKey(listener.MyContextType) && SpecificListeners[listener.MyContextType].Contains(listener))
                {
                    SpecificListeners[listener.MyContextType].Remove(listener);
                }
            }
            else
            {
                ToBeRemoved.Add(listener);
            }
        }

        public static void TriggerEvent(EngineEventArgs args)
        {
            if (TypesToSave.Contains(args.MyContext.Context))
            {
                SavedEvents.Add(args);
            }
            foreach (var list in GeneralListeners)
            {
                DoTrigger(list, args);
            }
            if (SpecificListeners.ContainsKey(args.MyContext.Context))
            {
                foreach (var listener in SpecificListeners[args.MyContext.Context])
                {
                    DoTrigger(listener, args);
                }
            }

            if (CallDepth == 0)
            {
                foreach (var toAdd in ToBeAdded)
                {
                    StartListening(toAdd);
                }
                ToBeAdded.Clear();
                foreach (var toRemove in ToBeRemoved)
                {
                    StopListening(toRemove);
                }
                ToBeRemoved.Clear();
            }
        }

        private static void DoTrigger(EngineEventListener listener, EngineEventArgs args)
        {
            CallDepth++;
            listener.Trigger(args);
            CallDepth--;
        }
    }
}
