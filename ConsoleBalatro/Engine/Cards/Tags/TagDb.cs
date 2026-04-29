using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Tags
{
    public static class TagDb
    {
        private static Queue<PackType> QueuedPackType = new();
        private static bool PackQueueListenerInitialized = false;

        private static Dictionary<int, List<EngineEventListener>> TagListeners = new();

        //PUT DATA DICTIONARY HERE
        public static Dictionary<TagType, Func<Card, JokerCardDataBlock>> TagBuilders = new()
        {
            {TagType.INVESTMENT, c =>
            {
                var jdb = PrepBlockForTag("Investment Tag", c);
                jdb.TagData.EventTypesTrigger.Add(EventContextType.GatherPostRoundMoney);
                jdb.TagData.DoTrigger = (args, ct) =>
                {
                    return args is EngineGatherPostRoundMoneyArgs moneyArgs && FlowHandler.CurrentSelectedBlind == BlindType.BOSS;
                };
                jdb.TagData.Activate = args =>
                {
                    if(args is EngineGatherPostRoundMoneyArgs moneyArgs)
                        moneyArgs.JokersContributed.Add((jdb, 25));//TODO: Data dict val???
                };

                return jdb;
            } },
            {TagType.DOUBLE_TAG, c =>
            {
                var jdb = PrepBlockForTag("Double Tag", c);
                jdb.TagData.EventTypesTrigger.Add(EventContextType.TagAdded);
                jdb.TagData.DoTrigger = (args, ct) =>
                {
                    return args is EngineTagAddedEventArgs tagArgs && tagArgs.isPostAdd;
                };
                jdb.TagData.Activate = args =>
                {
                    if(args is EngineTagAddedEventArgs tagArgs)
                        OnTagAdd(tagArgs.TagCard.MakeCopy());
                };

                return jdb;
            } },
            {TagType.MEGA_ARCANA, c =>
            {
                var jdb = PrepBlockForTag("Mega Arcana Tag", c);
                jdb.TagData.EventTypesTrigger.Add(EventContextType.PostGameStatePop);
                jdb.TagData.EventTypesTrigger.Add(EventContextType.PostGameStatePush);
                jdb.TagData.DoTrigger = (args, ct) =>
                {
                    return args is EngineGameStateChangeArgs stateArgs && stateArgs.NewState != null && stateArgs.NewState.GameState != GameState.SelectingPackOption && !stateArgs.StateChangeIsInterrupted; ;
                };

                jdb.TagData.Activate = args =>
                {
                    if(args is EngineGameStateChangeArgs stateArgs)
                    {
                        stateArgs.StateChangeIsInterrupted = true;
                        Card targPack = ConsumableManager.MakePack(Enums.PackType.MEGA_TAROT);
                        PackActions.OpenPack(targPack);
                    }
                };

                return jdb;
            } },
            {TagType.DOUBLE_MONEY, c => {
                var jdb = PrepBlockForTag("Double Money Tag", c);
                jdb.TagData.OnAddAction = _ =>
                {
                    if(Globals.Money > 0)
                        Globals.EmitMoneyGain(Math.Min(Globals.Money, 20), jdb.MyCard);
                };
                return jdb;
            } },
            {TagType.NEGATIVE, c => BuildEditionShopTag("Negative Tag", Edition.NEGATIVE, c) },
            {TagType.HOLO, c => BuildEditionShopTag("Holo Tag", Edition.HOLOGRAPHIC, c) },
            {TagType.FOIL, c => BuildEditionShopTag("Foil Tag", Edition.FOIL, c) },
            {TagType.POLYCHROME, c => BuildEditionShopTag("Polychrome Tag", Edition.POLYCHROME, c) },
            {TagType.MEGA_JOKER, c => BuildMegaPackTag("Mega Joker Tag", PackType.MEGA_JOKER, c) },
            {TagType.MEGA_ARCANA, c => BuildMegaPackTag("Mega Arcana Tag", PackType.MEGA_TAROT, c) },
            {TagType.MEGA_PLANET, c => BuildMegaPackTag("Mega Planet Tag", PackType.MEGA_PLANET, c) },
            {TagType.MEGA_STANDARD, c => BuildMegaPackTag("Mega Standard Tag", PackType.MEGA_STANDARD, c) },
            {TagType.SPECTRAL, c => BuildMegaPackTag("Spectral Tag", PackType.MEGA_SPECTRAL, c) },
            {TagType.TOP_UP, c => BuildImmediateTag("Top-Up Tag", c, _ =>
            {
                for (int i = 0; i < 2 && ZoneManager.JokerZone.HasRoom; i++)
                    {
                        var commonJoker = MarketOptionsManager.PullRandomJokerFromPool(JokerRarity.COMMON, removeFromPool: true);
                        if (commonJoker == null)
                            break;
                        ZoneManager.JokerZone.AddCard(commonJoker, invisibleAdd: false);
                    }
            }) },
            {TagType.UNCOMMON, c => BuildRarityShopTag("Uncommon Tag", JokerRarity.UNCOMMON, c) },
            {TagType.RARE, c => BuildRarityShopTag("Rare Tag", JokerRarity.RARE, c) },
            {TagType.HANDY, c => BuildImmediateTag("Handy Tag", c, _ => Globals.EmitMoneyGain(EngineEventHandler.CountOfSaved(EventContextType.HandPlayDone), c))},
            {TagType.GARBAGE, c => BuildImmediateTag("Garbage Tag", c, _ => Globals.EmitMoneyGain(EngineEventHandler.CountOfSaved(EventContextType.HandPlayDone), c))},
            {TagType.ORBITAL, c => BuildImmediateTag("Orbital Tag", c, _ =>
            {
                var upgradable = Enum.GetValues(typeof(PlayedHandType)).Cast<PlayedHandType>().ToList();
                if (upgradable.Count == 0)
                    return;
                var chosen = upgradable[Random.Shared.Next(upgradable.Count)];
                for (int i = 0; i < 3; i++)
                    ScoreHandler.LevelUpHand(chosen);
            }) },
            {TagType.VOUCHER, c => 
            {
                var jdb = PrepBlockForTag("Voucher Tag", c);
                jdb.TagData.EventTypesTrigger.Add(EventContextType.MarketSetupDone);
                jdb.TagData.Activate = _ =>
                {
                    MarketOptionsManager.DrawMarketItem(BuyItemType.VOUCHER, ZoneManager.VoucherMarketZone, overrideSpaceLimits: true);
                };
                return jdb;
            } },
            {TagType.BOSS_REROLL, c => BuildImmediateTag("Boss Reroll Tag", c, _ => FlowHandler.RerollBossBlind())},
            {TagType.COUPON, c =>
            {
                var jdb = PrepBlockForTag("Coupon Tag", c);
                jdb.TagData.EventTypesTrigger.Add(EventContextType.MarketSetupDone);
                jdb.TagData.DoTrigger = (_, _) =>
                {
                    var allTargets = new List<Card>();
                    allTargets.AddRange(ZoneManager.MainMarketZone.Cards);
                    allTargets.AddRange(ZoneManager.PackMarketZone.Cards);

                    return allTargets.Any(x => x.BuyCost != 0);
                };
                jdb.TagData.Activate = _ =>
                {
                    MarketGeneralManager.MakeMarketFree();
                };
                return jdb;
            } },
            {TagType.REROLLS, c =>
            {
                var jdb = PrepBlockForTag("Rerolls Tag", c);
                jdb.TagData.EventTypesTrigger.Add(EventContextType.MarketSetupDone);
                jdb.TagData.DoTrigger = (_, _) =>
                {
                    return Globals.CurrentRerollCost != 0;
                };
                jdb.TagData.Activate = _ =>
                {
                    Globals.CurrentRerollCost = 0;
                };
                return jdb;
            } },
            {TagType.JUGGLE, c =>
            {
                var jdb = PrepBlockForTag("Juggle Tag", c);
                jdb.TagData.EventTypesTrigger.Add(EventContextType.StartPlayRound);
                jdb.TagData.Activate = args =>
                {
                    if(args is EnginePlayRoundSetupArgs playArgs)
                    {
                        playArgs.TempHandSizeBonus += 3;
                    }
                };
                return jdb;
            } },
            {TagType.SPEED, c => BuildImmediateTag("Speed Tag", c, _ => Globals.EmitMoneyGain(EngineEventHandler.CountOfSaved(EventContextType.BlindSkip) * 5, c))},
        };

        public static JokerCardDataBlock PrepBlockForTag(string name, Card c)
        {
            return new JokerCardDataBlock()
            {
                isJoker = false,
                isTag = true,
                JokerName = name,
                MyCard = c,
                TagData = new TagDataBlock(),
            };
        }

        public static JokerCardDataBlock BuildImmediateTag(string name, Card c, Action<EngineEventArgs> OnAddAction)
        {
            var jokerDataBlock = PrepBlockForTag(name, c);
            jokerDataBlock.TagData.OnAddAction = OnAddAction;
            return jokerDataBlock;
        }

        private static JokerCardDataBlock BuildEditionShopTag(string name, Edition edition, Card c)
        {
            var jokerDataBlock = PrepBlockForTag(name, c);
            jokerDataBlock.TagData.EventTypesTrigger.Add(EventContextType.StartMarket);

            jokerDataBlock.TagData.Activate = _ =>
            {
                //TODO: TECHNICALLY THIS IS NOT CORRECT.
                //CURRENTLY DOES: AT START OF MARKET, BEFORE POPULATION, DRAW A JOKER OF CORRECT REQUIREMENTS.
                //SHOULD DO: WHEN A JOKER IS NEXT DRAWN TO MARKET, SET ITS EDITION AND OVERRIDE COST.
                var toAdd = MarketOptionsManager.PullRandomJokerFromPool(null);
                if (toAdd == null)
                    return;
                toAdd.SetEditionOfficial(edition);
                toAdd.BuyCostOverride = 0;
                MarketOptionsManager.DrawTargetMarketItem(BuyItemType.JOKER, ZoneManager.MainMarketZone, toAdd);
            };
            return jokerDataBlock;
        }

        private static JokerCardDataBlock BuildRarityShopTag(string name, JokerRarity rarity, Card c)
        {
            var jokerDataBlock = PrepBlockForTag(name, c);
            jokerDataBlock.TagData.EventTypesTrigger.Add(EventContextType.StartMarket);
            jokerDataBlock.TagData.Activate = _ =>
            {
                var toAdd = MarketOptionsManager.PullRandomJokerFromPool(rarity);
                if (toAdd == null)
                    return;
                toAdd.BuyCostOverride = 0;
                MarketOptionsManager.DrawTargetMarketItem(BuyItemType.JOKER, ZoneManager.MainMarketZone, toAdd);
            };
            return jokerDataBlock;
        }

        private static JokerCardDataBlock BuildMegaPackTag(string name, PackType packType, Card c) => BuildImmediateTag(name, c, _ => EnqueuePackCard(packType));
        

        private static void EnqueuePackCard(PackType packType)
        {
            EnsurePackQueueListener();
            QueuedPackType.Enqueue(packType);
            TryOpenQueuedPack();
        }

        private static void EnsurePackQueueListener()
        {
            if (PackQueueListenerInitialized)
                return;
            EngineEventHandler.StartListening(new EngineEventListener()
            {
                MyContextType = EventContextType.PostGameStatePop,
                MyAction = _ => TryOpenQueuedPack(),
            });
            PackQueueListenerInitialized = true;
        }

        private static void TryOpenQueuedPack()
        {
            if(QueuedPackType.Count == 0 ||
                Globals.GameStateStack == null ||
                Globals.GameStateStack.Count == 0 ||
                Globals.CurrentGameState == GameState.SelectingPackOption)
            {
                return;
            }

            var nextPack = QueuedPackType.Dequeue();
            PackActions.OpenPack(ConsumableManager.MakePack(nextPack));
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
