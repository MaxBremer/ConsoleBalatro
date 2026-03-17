using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Tags
{
    public static class TagDb
    {
        private static Stack<PackTagQueueEntry> PackTagsWaiting;

        private static Dictionary<int, List<EngineEventListener>> TagListeners = new();

        //PUT DATA DICTIONARY HERE
        public static Dictionary<TagType, Func<Card, JokerCardDataBlock>> TagBuilders = new()
        {
            //DO: POPULATE
        };

        public static JokerCardDataBlock PrepBlockForTag(string name)
        {
            return new JokerCardDataBlock()
            {
                isJoker = false,
                isTag = true,
                JokerName = name,
            };
        }

        public static void MakeCardTagOfType(Card c, TagType type)
        {
            var jokerBlock = TagBuilders[type](c);
            jokerBlock.TagData.MyType = type;
            c.JokerData = jokerBlock;
        }

        public static Card BuildTagOfType(TagType type)
        {
            var c = new Card();
            MakeCardTagOfType(c, type);
            return c;
        }

        public static void AddTagOfType(TagType type)
        {
            var c = BuildTagOfType(type);
            OnTagAdd(c);
        }

        public static void OnTagAdd(Card TagCard, CardZone fromZone = null)
        {
            if (!TagCard.isTag)
                return;
            if(fromZone == null)
            {
                ZoneManager.TagZone.AddCard(TagCard);
            }
            else
            {
                ZoneManager.TagZone.DrawTargetFrom(fromZone, TagCard);
            }
            Action<EngineEventArgs> ActivateFunc = arg =>
            {
                var data = TagCard.JokerData.TagData;
                if(data.DoTrigger(arg, TagCard))
                {
                    ZoneManager.PreDestructionZone.DrawTargetFrom(ZoneManager.TagZone, TagCard);
                    data.Activate(arg);
                    OnTagRemove(TagCard);
                    ZoneManager.DestroyCard(TagCard, ZoneManager.PreDestructionZone);
                }
            };

            var listenerList = new List<EngineEventListener>();
            foreach (var evType in TagCard.JokerData.TagData.EventTypesTrigger)
            {
                var listener = new EngineEventListener() { MyAction = ActivateFunc, MyContextType = evType };
                EngineEventHandler.StartListening(listener);
                listenerList.Add(listener);
            }
            TagListeners.Add(TagCard.JokerData.TagData.MyTagID, listenerList);

            var onAddAct = TagCard.JokerData.TagData.OnAddAction;
            if(onAddAct != null)
            {
                onAddAct(null);//TODO: args needed????
            }

            EventContext evContext = new() { Context = EventContextType.TagAdded };
            EngineEventHandler.TriggerEvent(new EngineTagAddedEventArgs()
            {
                isPostAdd = true,
                TagCard = TagCard,
                MyContext = evContext,
            });
        }

        public static void OnTagRemove(Card TagCard)
        {
            if (!TagCard.isTag)
                return;
            var targetId = TagCard.JokerData.TagData.MyTagID;
            foreach (var list in TagListeners[targetId])
            {
                EngineEventHandler.StopListening(list);
            }
            TagListeners[targetId].Clear();
            TagListeners.Remove(targetId);
        }
    }
}
