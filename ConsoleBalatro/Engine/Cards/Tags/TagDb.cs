using ConsoleBalatro.Engine.Cards.Consumables;
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
            {TagType.INVESTMENT, c =>
            {
                var jokerDataBlock = PrepBlockForTag("Investment Tag");
                var tagDataBlock = new TagDataBlock();
                tagDataBlock.EventTypesTrigger.Add(EventContextType.GatherPostRoundMoney);
                Func<EngineEventArgs, Card, bool> doTriggerFunc = (args, ct) =>
                {
                    return args is EngineGatherPostRoundMoneyArgs moneyArgs && FlowHandler.CurrentSelectedBlind == BlindType.BOSS;
                };
                tagDataBlock.DoTrigger = doTriggerFunc;
                Action<EngineEventArgs> activation = args =>
                {
                    if(args is EngineGatherPostRoundMoneyArgs moneyArgs)
                        moneyArgs.JokersContributed.Add((jokerDataBlock, 25));//TODO: Data dict val???
                };
                tagDataBlock.Activate = activation;

                jokerDataBlock.TagData = tagDataBlock;
                return jokerDataBlock;
            } },
            {TagType.DOUBLE_TAG, c =>
            {
                var jokerDataBlock = PrepBlockForTag("Double Tag");
                var tagDataBlock = new TagDataBlock();
                tagDataBlock.EventTypesTrigger.Add(EventContextType.TagAdded);
                Func<EngineEventArgs, Card, bool> doTriggerFunc = (args, ct) =>
                {
                    return args is EngineTagAddedEventArgs tagArgs && tagArgs.isPostAdd;
                };
                tagDataBlock.DoTrigger = doTriggerFunc;
                Action<EngineEventArgs> activation = args =>
                {
                    if(args is EngineTagAddedEventArgs tagArgs)
                        OnTagAdd(tagArgs.TagCard.MakeCopy());
                };
                tagDataBlock.Activate = activation;

                jokerDataBlock.TagData = tagDataBlock;
                return jokerDataBlock;
            } },
            {TagType.MEGA_ARCANA, c =>
            {
                var jokerDataBlock = PrepBlockForTag("Mega Arcana Tag");
                var tagDataBlock = new TagDataBlock();
                tagDataBlock.EventTypesTrigger.Add(EventContextType.PostGameStatePop);
                tagDataBlock.EventTypesTrigger.Add(EventContextType.PostGameStatePush);
                Func<EngineEventArgs, Card, bool> doTriggerFunc = (args, ct) =>
                {
                    return args is EngineGameStateChangeArgs stateArgs && stateArgs.NewState != null && stateArgs.NewState.GameState != GameState.SelectingPackOption && !stateArgs.StateChangeIsInterrupted; ;
                };
                tagDataBlock.DoTrigger = doTriggerFunc;
                Action<EngineEventArgs> activation = args =>
                {
                    if(args is EngineGameStateChangeArgs stateArgs)
                    {
                        stateArgs.StateChangeIsInterrupted = true;
                        Card targPack = ConsumableManager.MakePack(Enums.PackType.MEGA_TAROT);
                        PackActions.OpenPack(targPack);
                    }
                };
                tagDataBlock.Activate = activation;

                jokerDataBlock.TagData = tagDataBlock;
                return jokerDataBlock;
            } },
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
            if (type == TagType.NONE)
                return;
            var c = BuildTagOfType(type);
            OnTagAdd(c);
        }

        public static void OnTagAdd(Card TagCard, CardZone fromZone = null)
        {
            //First just check that the card passed is a tag.
            if (!TagCard.isTag)
                return;

            //Build the activation function.
            //TODO: Make immediate activation a flag?
            //Then this func is just called on add? No need for a separate "on add func"?
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

            //Trigger the pre-addition tagAdd event.
            EventContext evContext = new() { Context = EventContextType.TagAdded };
            EngineEventHandler.TriggerEvent(new EngineTagAddedEventArgs()
            {
                isPostAdd = false,
                TagCard = TagCard,
                MyContext = evContext,
            });

            //Now, draw the card to the tag zone, either from another zone or from nothing.
            if (fromZone == null)
            {
                ZoneManager.TagZone.AddCard(TagCard);
            }
            else
            {
                ZoneManager.TagZone.DrawTargetFrom(fromZone, TagCard);
            }

            //Add all the tags listeners
            var listenerList = new List<EngineEventListener>();
            foreach (var evType in TagCard.JokerData.TagData.EventTypesTrigger)
            {
                var listener = new EngineEventListener() { MyAction = ActivateFunc, MyContextType = evType };
                EngineEventHandler.StartListening(listener);
                listenerList.Add(listener);
            }
            //track the listeners for stopping post-activation.
            TagListeners.Add(TagCard.JokerData.TagData.MyTagID, listenerList);

            //If there is an on add action, activate it now. We're doing the add.
            var onAddAct = TagCard.JokerData.TagData.OnAddAction;
            if(onAddAct != null)
            {
                onAddAct(null);//TODO: args needed????
                //After an on-add activation, destroy this tag.
                //NOTE: This means tags can only have ONE activation, whether triggered via listeners or on-add.
                //This makes sense gameplay-wise (tags only trigger once when they "pop"), but is slightly counter-intuitive code-wise.
                ZoneManager.PreDestructionZone.DrawTargetFrom(ZoneManager.TagZone, TagCard);
                OnTagRemove(TagCard);
                ZoneManager.DestroyCard(TagCard, ZoneManager.PreDestructionZone);
            }

            //Finally, trigger the post-add tagAdd event.
            EventContext evContextPostAdd = new() { Context = EventContextType.TagAdded };
            EngineEventHandler.TriggerEvent(new EngineTagAddedEventArgs()
            {
                isPostAdd = true,
                TagCard = TagCard,
                MyContext = evContextPostAdd,
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
