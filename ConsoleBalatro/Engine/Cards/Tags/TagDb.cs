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
                jdb.DataDict.Add("INTAMOUNT", new JokerData() {MyDataType = JokerDataType.INT, IntData = 25});
                jdb.DescriptionBuilder = _ => "Gain $" + jdb.DataDict["INTAMOUNT"].IntData + " after defeating the next Boss Blind.";
                jdb.TagData.EventTypesTrigger.Add(EventContextType.GatherPostRoundMoney);
                jdb.TagData.DoTrigger = (args, _) =>
                {
                    return args is EngineGatherPostRoundMoneyArgs && FlowHandler.CurrentSelectedBlind == BlindType.BOSS;
                };
                jdb.TagData.Activate = args =>
                {
                    if(args is EngineGatherPostRoundMoneyArgs moneyArgs)
                        moneyArgs.JokersContributed.Add((jdb, jdb.DataDict["INTAMOUNT"].IntData));
                };

                return jdb;
            } },
            {TagType.DOUBLE_TAG, BuildDoubleTag },
            {TagType.ECONOMY, c => {
                var jdb = PrepBlockForTag("Economy Tag", c);
                jdb.DescriptionBuilder = _ => "Doubles your money (max. $40).";
                jdb.TagData.OnAddAction = _ =>
                {
                    if(Globals.Money > 0)
                        Globals.EmitMoneyGain(Math.Min(Globals.Money, 40), jdb.MyCard);//TODO: Maximum is datadict val?
                };
                return jdb;
            } },
            {TagType.NEGATIVE, c => BuildEditionShopTag("Negative Tag", Edition.NEGATIVE, c) },
            {TagType.HOLO, c => BuildEditionShopTag("Holo Tag", Edition.HOLOGRAPHIC, c) },
            {TagType.FOIL, c => BuildEditionShopTag("Foil Tag", Edition.FOIL, c) },
            {TagType.POLYCHROME, c => BuildEditionShopTag("Polychrome Tag", Edition.POLYCHROME, c) },
            {TagType.MEGA_JOKER, c => BuildMegaPackTag("Buffoon Tag", PackType.MEGA_JOKER, c) },
            {TagType.MEGA_ARCANA, c => BuildMegaPackTag("Charm Tag", PackType.MEGA_TAROT, c) },
            {TagType.MEGA_PLANET, c => BuildMegaPackTag("Meteor Tag", PackType.MEGA_PLANET, c) },
            {TagType.MEGA_STANDARD, c => BuildMegaPackTag("Standard Tag", PackType.MEGA_STANDARD, c) },
            {TagType.SPECTRAL, c => BuildMegaPackTag("Ethereal Tag", PackType.BASIC_SPECTRAL, c) },
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
            {TagType.HANDY, c => BuildImmediateTag("Handy Tag", c, _ => Globals.EmitMoneyGain(EngineEventHandler.CountOfSaved(EventContextType.HandPlayDone), c), hardDesc: "Gain $1 for each hand played this run.")},//TODO: Datadict val for amt per hand?
            {TagType.GARBAGE, c => BuildImmediateTag("Garbage Tag", c, _ => Globals.EmitMoneyGain(EngineEventHandler.CountOfSaved(EventContextType.HandDiscardDone), c), hardDesc: "Gain $1 for each unused discard this run.")},//TODO: See above.
            {TagType.ORBITAL, c => {
                var ret = PrepBlockForTag("Orbital Tag", c);
                var upgradable = Enum.GetValues(typeof(PlayedHandType)).Cast<PlayedHandType>().ToList();
                var targetUpgrade = upgradable[Globals.randomNext(upgradable.Count)];
                ret.DescriptionBuilder = _ => "Upgrades " + targetUpgrade.ToString() + " by three levels.";
                ret.TagData.OnAddAction = _ =>
                {
                    for (int i = 0; i < 3; i++)
                        ScoreHandler.LevelUpHand(targetUpgrade);
                };

                return ret;
            } },
            {TagType.VOUCHER, c => 
            {
                var jdb = PrepBlockForTag("Voucher Tag", c);
                jdb.TagData.EventTypesTrigger.Add(EventContextType.MarketSetupDone);
                jdb.DescriptionBuilder = _ => "Adds a Voucher to the next Shop";
                jdb.TagData.Activate = _ =>
                {
                    MarketOptionsManager.DrawMarketItem(BuyItemType.VOUCHER, ZoneManager.VoucherMarketZone, overrideSpaceLimits: true);
                };
                return jdb;
            } },
            {TagType.BOSS_REROLL, c => BuildImmediateTag("Boss Tag", c, _ => FlowHandler.RerollBossBlind(), hardDesc: "Re-rolls the next Boss Blind.")},
            {TagType.COUPON, c =>
            {
                var jdb = PrepBlockForTag("Coupon Tag", c);
                jdb.TagData.EventTypesTrigger.Add(EventContextType.MarketSetupDone);
                jdb.DescriptionBuilder = _ => "In the next shop, initial Jokers, Consumables, Cards and Booster Packs are free ($0).";
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
                var jdb = PrepBlockForTag("D6 Tag", c);
                jdb.DescriptionBuilder = _ => "In the next Shop, Rerolls start at $0.";
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
                jdb.DataDict.Add("INTAMOUNT", new JokerData() {MyDataType = JokerDataType.INT, IntData = 3 });
                jdb.DescriptionBuilder = _ => "+" + jdb.DataDict["INTAMOUNT"].IntData + " Hand Size for the next round only.";
                jdb.TagData.Activate = args =>
                {
                    if(args is EnginePlayRoundSetupArgs playArgs)
                    {
                        playArgs.TempHandSizeBonus += jdb.DataDict["INTAMOUNT"].IntData;
                    }
                };
                return jdb;
            } },
            {TagType.SPEED, c => BuildImmediateTag("Speed Tag", c, _ => Globals.EmitMoneyGain(EngineEventHandler.CountOfSaved(EventContextType.BlindSkip) * 5, c), hardDesc: "Gives $5 for each Blind you've skipped this run.")},//TODO: Datadict val?
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

        public static JokerCardDataBlock BuildDoubleTag(Card c)
        {
            var jokerDataBlock = PrepBlockForTag("Double Tag", c);
            jokerDataBlock.DataDict.Add("ACTIVATED", new JokerData() { MyDataType = JokerDataType.BOOL, BoolData = false });
            jokerDataBlock.DescriptionBuilder = _ => "Gives a copy of the next Tag selected (excluding Double Tags).";
            var tagDataBlock = new TagDataBlock();
            tagDataBlock.ImmuneToDouble = true;
            tagDataBlock.EventTypesTrigger.Add(EventContextType.TagAdded);
            tagDataBlock.DoTrigger = (args, ct) => args is EngineTagAddedEventArgs tagArgs
                && tagArgs.isPostAdd
                && !tagArgs.TagCard.JokerData.TagData.ImmuneToDouble
                && ct.JokerData.DataDict.ContainsKey("ACTIVATED")
                && !ct.JokerData.DataDict["ACTIVATED"].BoolData;
            
            tagDataBlock.Activate = args =>
            {
                if(args is EngineTagAddedEventArgs tagArgs)
                {
                    jokerDataBlock.DataDict["ACTIVATED"].BoolData = true;
                    OnTagAdd(tagArgs.TagCard.MakeCopy());
                }
            };
            jokerDataBlock.TagData = tagDataBlock;
            return jokerDataBlock;
        }

        public static JokerCardDataBlock BuildImmediateTag(string name, Card c, Action<EngineEventArgs> OnAddAction, string hardDesc = "")
        {
            var jokerDataBlock = PrepBlockForTag(name, c);
            jokerDataBlock.TagData.OnAddAction = OnAddAction;
            if (!string.IsNullOrEmpty(hardDesc))
            {
                jokerDataBlock.DescriptionBuilder = _ => hardDesc;
            }
            return jokerDataBlock;
        }

        private static JokerCardDataBlock BuildEditionShopTag(string name, Edition edition, Card c)
        {
            var jokerDataBlock = PrepBlockForTag(name, c);
            jokerDataBlock.TagData.EventTypesTrigger.Add(EventContextType.StartMarket);
            jokerDataBlock.TagData.DoTrigger = (_, _) => ZoneManager.MainMarketZone.HasRoom;

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
            jokerDataBlock.DescriptionBuilder = _ => "The next base edition Joker you find in a Shop becomes " + edition.ToString() + " " + EngineUtils.EditionDescriptors[edition] + " and free.";
            return jokerDataBlock;
        }

        private static JokerCardDataBlock BuildRarityShopTag(string name, JokerRarity rarity, Card c)
        {
            var jokerDataBlock = PrepBlockForTag(name, c);
            jokerDataBlock.TagData.EventTypesTrigger.Add(EventContextType.StartMarket);
            jokerDataBlock.TagData.DoTrigger = (_, _) => ZoneManager.MainMarketZone.HasRoom;
            jokerDataBlock.TagData.Activate = _ =>
            {
                var toAdd = MarketOptionsManager.PullRandomJokerFromPool(rarity);
                if (toAdd == null)
                    return;
                toAdd.BuyCostOverride = 0;
                MarketOptionsManager.DrawTargetMarketItem(BuyItemType.JOKER, ZoneManager.MainMarketZone, toAdd);
            };
            jokerDataBlock.DescriptionBuilder = _ => "The next shop will have a free " + rarity.ToString() + " Joker.";
            return jokerDataBlock;
        }

        private static JokerCardDataBlock BuildMegaPackTag(string name, PackType packType, Card c) { 
            var ret = BuildImmediateTag(name, c, _ => EnqueuePackCard(packType));
            ret.DescriptionBuilder = _ => "Immediately open a free " + ConsumableManager.PackBasicNums[packType].PackName + ".";
            return ret;
        }
        

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
                    EngineEventHandler.TriggerEvent(new EngineTagTriggeredArgs()
                    {
                        TagThatTriggered = TagCard,
                        MyContext = new() { Context = EventContextType.TagActivatedViaListener},
                    });
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
                EngineEventHandler.TriggerEvent(new EngineTagTriggeredArgs()
                {
                    TagThatTriggered = TagCard,
                    MyContext = new() { Context = EventContextType.TagActivatedInstantly },
                });
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
