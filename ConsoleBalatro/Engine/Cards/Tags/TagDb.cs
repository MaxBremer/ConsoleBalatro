using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;

namespace ConsoleBalatro.Engine.Cards.Tags
{
    public static class TagDb
    {
        private static Dictionary<int, List<EngineEventListener>> TagListeners = new();
        private static Queue<PackType> QueuedPackOpens = new();
        private static bool PackQueueListenerInitialized = false;

        public static Dictionary<TagType, Func<Card, JokerCardDataBlock>> TagBuilders = new()
        {
            {TagType.NEGATIVE, _ => BuildEditionShopTag("Negative Tag", Edition.NEGATIVE) },
            {TagType.HOLO, _ => BuildEditionShopTag("Holo Tag", Edition.HOLOGRAPHIC) },
            {TagType.FOIL, _ => BuildEditionShopTag("Foil Tag", Edition.FOIL) },
            {TagType.POLYCHROME, _ => BuildEditionShopTag("Polychrome Tag", Edition.POLYCHROME) },
            {TagType.MEGA_JOKER, _ => BuildMegaPackTag("Mega Joker Tag", PackType.MEGA_JOKER) },
            {TagType.MEGA_ARCANA, _ => BuildMegaPackTag("Mega Arcana Tag", PackType.MEGA_TAROT) },
            {TagType.MEGA_PLANET, _ => BuildMegaPackTag("Mega Planet Tag", PackType.MEGA_PLANET) },
            {TagType.MEGA_STANDARD, _ => BuildMegaPackTag("Mega Standard Tag", PackType.MEGA_STANDARD) },
            {TagType.SPECTRAL, _ => BuildMegaPackTag("Spectral Tag", PackType.MEGA_SPECTRAL) },
            {TagType.TOP_UP, _ => BuildImmediateTag("Top-Up Tag", _ =>
                {
                    for (int i = 0; i < 2 && ZoneManager.JokerZone.HasRoom; i++)
                    {
                        var commonJoker = BuildRandomJokerForShop(JokerRarity.COMMON);
                        if (commonJoker == null)
                            break;
                        ZoneManager.JokerZone.AddCard(commonJoker, invisibleAdd: false);
                    }
                }) },
            {TagType.DOUBLE_MONEY, _ => BuildImmediateTag("Double Money Tag", _ =>
                {
                    if (Globals.Money > 0)
                    {
                        Globals.EmitMoneyGain(Math.Min(Globals.Money, 40), null);
                    }
                }) },
            {TagType.INVESTMENT, _ => BuildPostRoundMoneyTag("Investment Tag", 25) },
            {TagType.UNCOMMON, _ => BuildRarityShopTag("Uncommon Tag", JokerRarity.UNCOMMON) },
            {TagType.RARE, _ => BuildRarityShopTag("Rare Tag", JokerRarity.RARE) },
            {TagType.HANDY, _ => BuildImmediateTag("Handy Tag", _ => Globals.EmitMoneyGain(FlowHandler.HandsPlayedThisRun, null)) },
            {TagType.GARBAGE, _ => BuildImmediateTag("Garbage Tag", _ => Globals.EmitMoneyGain(FlowHandler.DiscardActionsThisRun, null)) },
            {TagType.ORBITAL, _ => BuildImmediateTag("Orbital Tag", _ =>
                {
                    var upgradable = Enum.GetValues(typeof(PlayedHandType)).Cast<PlayedHandType>().Where(x => x != PlayedHandType.NONE).ToList();
                    if (upgradable.Count == 0)
                        return;
                    var chosen = upgradable[Random.Shared.Next(upgradable.Count)];
                    for (int i = 0; i < 3; i++)
                        ScoreHandler.LevelUpHand(chosen);
                }) },
            {TagType.VOUCHER, _ => BuildImmediateTag("Voucher Tag", _ =>
                {
                    if (ZoneManager.ActiveVoucherZone.HasRoom)
                        MarketOptionsManager.DrawMarketItem(BuyItemType.VOUCHER, ZoneManager.ActiveVoucherZone);
                }) },
            {TagType.BOSS_REROLL, _ => BuildImmediateTag("Boss Reroll Tag", _ => FlowHandler.RerollBossBlind()) },
            {TagType.COUPON, _ => BuildImmediateTag("Coupon Tag", _ => FlowHandler.NextShopIsCouponed = true) },
            {TagType.DOUBLE_TAG, _ => BuildDoubleTag() },
            {TagType.JUGGLE, _ => BuildImmediateTag("Juggle Tag", _ => FlowHandler.NextRoundGetsJuggleHandSize = true) },
            {TagType.REROLLS, _ => BuildImmediateTag("Reroll Tag", _ => FlowHandler.NextShopRerollsStartFree = true) },
            {TagType.SPEED, _ => BuildImmediateTag("Speed Tag", _ => Globals.EmitMoneyGain(FlowHandler.BlindsSkippedThisRun * 5, null)) },
        };

        private static JokerCardDataBlock BuildImmediateTag(string name, Action<EngineEventArgs> onAddAction)
        {
            var jokerDataBlock = PrepBlockForTag(name);
            var tagDataBlock = new TagDataBlock();
            tagDataBlock.OnAddAction = onAddAction;
            tagDataBlock.Activate = _ => { };
            jokerDataBlock.TagData = tagDataBlock;
            return jokerDataBlock;
        }

        private static JokerCardDataBlock BuildPostRoundMoneyTag(string name, int amount)
        {
            var jokerDataBlock = PrepBlockForTag(name);
            var tagDataBlock = new TagDataBlock();
            tagDataBlock.EventTypesTrigger.Add(EventContextType.GatherPostRoundMoney);
            tagDataBlock.DoTrigger = (args, _) => args is EngineGatherPostRoundMoneyArgs && FlowHandler.CurrentSelectedBlind == BlindType.BOSS;
            tagDataBlock.Activate = args =>
            {
                if (args is EngineGatherPostRoundMoneyArgs moneyArgs)
                    moneyArgs.JokersContributed.Add((jokerDataBlock, amount));
            };
            jokerDataBlock.TagData = tagDataBlock;
            return jokerDataBlock;
        }

        private static JokerCardDataBlock BuildDoubleTag()
        {
            var jokerDataBlock = PrepBlockForTag("Double Tag");
            jokerDataBlock.DataDict.Add("ACTIVATED", new JokerData() { MyDataType = JokerDataType.BOOL, BoolData = false });
            var tagDataBlock = new TagDataBlock();
            tagDataBlock.EventTypesTrigger.Add(EventContextType.TagAdded);
            tagDataBlock.DoTrigger = (args, ct) => args is EngineTagAddedEventArgs tagArgs
                && tagArgs.isPostAdd
                && tagArgs.TagCard.JokerData.TagData.MyType != TagType.DOUBLE_TAG
                && ct.JokerData.DataDict.ContainsKey("ACTIVATED")
                && !ct.JokerData.DataDict["ACTIVATED"].BoolData;
            tagDataBlock.Activate = args =>
            {
                if (args is EngineTagAddedEventArgs tagArgs)
                {
                    jokerDataBlock.DataDict["ACTIVATED"].BoolData = true;
                    OnTagAdd(tagArgs.TagCard.MakeCopy());
                }
            };
            jokerDataBlock.TagData = tagDataBlock;
            return jokerDataBlock;
        }

        private static JokerCardDataBlock BuildMegaPackTag(string name, PackType packType)
            => BuildImmediateTag(name, _ => EnqueuePackOpen(packType));

        private static void EnqueuePackOpen(PackType packType)
        {
            EnsurePackQueueListener();
            QueuedPackOpens.Enqueue(packType);
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
            if (QueuedPackOpens.Count == 0)
                return;
            if (Globals.GameStateStack == null || Globals.GameStateStack.Count == 0)
                return;
            if (Globals.CurrentGameState == GameState.SelectingPackOption)
                return;

            var nextPack = QueuedPackOpens.Dequeue();
            PackActions.OpenPack(ConsumableManager.MakePack(nextPack));
        }

        private static JokerCardDataBlock BuildRarityShopTag(string name, JokerRarity rarity)
        {
            var jokerDataBlock = PrepBlockForTag(name);
            var tagDataBlock = new TagDataBlock();
            tagDataBlock.EventTypesTrigger.Add(EventContextType.StartMarket);
            tagDataBlock.DoTrigger = (args, _) => args.MyContext != null && args.MyContext.Context == EventContextType.StartMarket;
            tagDataBlock.Activate = _ =>
            {
                var toAdd = BuildRandomJokerForShop(rarity);
                if (toAdd != null)
                    AddCardToMainMarket(toAdd, forceFree: true);
            };
            jokerDataBlock.TagData = tagDataBlock;
            return jokerDataBlock;
        }

        private static JokerCardDataBlock BuildEditionShopTag(string name, Edition edition)
        {
            var jokerDataBlock = PrepBlockForTag(name);
            var tagDataBlock = new TagDataBlock();
            tagDataBlock.EventTypesTrigger.Add(EventContextType.StartMarket);
            tagDataBlock.DoTrigger = (args, _) => args.MyContext != null && args.MyContext.Context == EventContextType.StartMarket;
            tagDataBlock.Activate = _ =>
            {
                var toAdd = BuildRandomJokerForShop(null);
                if (toAdd == null)
                    return;
                toAdd.Edition = edition;
                AddCardToMainMarket(toAdd, forceFree: true);
            };
            jokerDataBlock.TagData = tagDataBlock;
            return jokerDataBlock;
        }

        private static void AddCardToMainMarket(Card c, bool forceFree = false)
        {
            if (c == null)
                return;
            if (forceFree)
            {
                c.BuyCostOverride = 0;
            }
            if (ZoneManager.MainMarketZone.HasRoom)
            {
                ZoneManager.MainMarketZone.AddCard(c, invisibleAdd: false);
                return;
            }
            var idx = Random.Shared.Next(ZoneManager.MainMarketZone.Cards.Count);
            var kicked = ZoneManager.MainMarketZone.Cards[idx];
            MarketOptionsManager.ReturnMarketItemFromZone(kicked, ZoneManager.MainMarketZone);
            ZoneManager.MainMarketZone.AddCard(c, invisibleAdd: false);
        }

        private static Card BuildRandomJokerForShop(JokerRarity? rarity)
        {
            var pool = MarketOptionsManager.MarketPoolsToDrawFrom[BuyItemType.JOKER];
            var valid = pool.Cards.Where(x => x.isJoker && (!rarity.HasValue || x.JokerData.Rarity == rarity.Value)).ToList();
            if (valid.Count == 0)
                return null;
            var chosen = valid[Random.Shared.Next(valid.Count)];
            var ret = new Card();
            chosen.TurnIntoCopyOfMe(ret);
            return ret;
        }

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

            EventContext evContext = new() { Context = EventContextType.TagAdded };
            EngineEventHandler.TriggerEvent(new EngineTagAddedEventArgs()
            {
                isPostAdd = true,
                TagCard = TagCard,
                MyContext = evContext,
            });

            var onAddAct = TagCard.JokerData.TagData.OnAddAction;
            if(onAddAct != null)
            {
                onAddAct(null);
                if(ZoneManager.TagZone.Cards.Contains(TagCard))
                {
                    ZoneManager.PreDestructionZone.DrawTargetFrom(ZoneManager.TagZone, TagCard);
                }
                OnTagRemove(TagCard);
                ZoneManager.DestroyCard(TagCard, ZoneManager.PreDestructionZone);
                return;
            }
        }

        public static void OnTagRemove(Card TagCard)
        {
            if (!TagCard.isTag)
                return;
            var targetId = TagCard.JokerData.TagData.MyTagID;
            if (!TagListeners.ContainsKey(targetId))
                return;
            foreach (var list in TagListeners[targetId])
            {
                EngineEventHandler.StopListening(list);
            }
            TagListeners[targetId].Clear();
            TagListeners.Remove(targetId);
        }
    }
}
