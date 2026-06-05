using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Blinds
{
    public static class BossBlindDb
    {
        public static List<string> BossBlindNames => BossBlindData.Keys.ToList();

        public static List<string> BossBlindsAlreadyUsed = new();

        public static List<string> BigBossBlinds = new()
        {
            "AMBER ACORN",
            "VERDANT LEAF",
            "VIOLET VESSEL",
            "CRIMSON HEART",
            "CERULEAN BELL",
        };

        public static Dictionary<string, int> BossBlindMinimumAntes = new()
        {
            ["THE OX"] = 6,
            ["THE HOUSE"] = 2,
            ["THE WALL"] = 2,
            ["THE WHEEL"] = 2,
            ["THE ARM"] = 2,
            ["THE FISH"] = 2,
            ["THE WATER"] = 2,
            ["THE EYE"] = 3,
            ["THE MOUTH"] = 2,
            ["THE PLANT"] = 4,
            ["THE SERPENT"] = 5,
            ["THE NEEDLE"] = 2,
            ["THE TOOTH"] = 3,
            ["THE FLINT"] = 2,
            ["THE MARK"] = 2,
        };

        public static List<string> AvailableBossBlinds 
        { 
            get 
            {
                List<string> validOpts = new List<string>();
                if(FlowHandler.CurrentAnte % 8 == 0)
                {
                    validOpts.AddRange(BigBossBlinds);
                }
                else
                {
                    validOpts.AddRange(BossBlindNames.Where(x => !BigBossBlinds.Contains(x)));
                    validOpts.RemoveAll(x => BossBlindMinimumAntes.ContainsKey(x) && BossBlindMinimumAntes[x] > FlowHandler.CurrentAnte);
                }

                return validOpts.Where(x => !BossBlindsAlreadyUsed.Contains(x)).ToList(); 
            } 
        }

        //NOTE!!!!!
        //ALL BOSS BLINDS need an OnJokerRemove that undoes their effect, such as adding hands to play if blind sets to one at start of round, etc.
        //This is for jokers Luchador and that one legendary that disable bosses.
        //This was kind of a stupid comment in retrospect... I mean regular jokers need that too lol.
        public static Dictionary<string, Func<Card, JokerCardDataBlock>> BossBlindData = new()
        {
            {
                "THE NEEDLE",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Needle");

                    ret.DescriptionBuilder = _ =>
                    {
                        var possPlural = ret.DataDict["INTAMOUNT"].IntData > 1 ? "s" : "";
                        return "Play only " + ret.DataDict["INTAMOUNT"].IntData + " hand" + possPlural + ".";
                    };
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.Listeners.Add(new Events.EngineEventListener()
                    {
                        MyContextType = Events.EventContextType.StartPlayRoundSetupOver,
                        MyAction = args =>
                        {
                            Globals.CurHandsRemaining = ret.DataDict["INTAMOUNT"].IntData;
                        }
                    });

                    return ret;
                }
            },
            {
                "THE WATER",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Water", "No discards this round.");

                    ret.Listeners.Add(new Events.EngineEventListener()
                    {
                        MyContextType = Events.EventContextType.StartPlayRoundSetupOver,
                        MyAction = args =>
                        {
                            Globals.CurDiscardsRemaining = 0;
                        }
                    });

                    return ret;
                }
            },
            {
                "THE OX",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Ox", "Playing the most played hand this run sets money to $0");

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.HandPlayedCalculated,
                        MyAction = args =>
                        {
                            if (args is EngineHandPlayArgs h)
                            {
                                var max = ScoreHandler.HandNumTimesPlayed.Values.Max();
                                if (ScoreHandler.HandNumTimesPlayed[h.HandBeingPlayed] == max)
                                {
                                    Globals.EmitMoneyLoss(Globals.Money, c, false);
                                    EngineEventHandler.TriggerEvent(new EngineEventArgs() {MyContext = new() { Context = EventContextType.BossAbilityTriggeredByHand}});
                                }
                            }
                        }
                    });

                    return ret;
                }
            },
            {
                "THE WALL",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Wall", "Extra large blind.");

                    return ret;
                }
            },
            {
                "THE HOOK",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Hook", "Discard 2 random cards in hand after each hand played.");

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.HandPlayDone,
                        MyAction = args =>
                        {
                            if (args is EngineHandPlayDoneArgs h && h.CardsHeldInHand.Count > 0)
                            {
                                if(h.CardsHeldInHand.Count <= 2)
                                    ZoneManager.DiscardZone.DrawTargetsFrom(ZoneManager.HandZone, h.CardsHeldInHand);
                                else
                                {
                                    List<Card> toDisc = h.CardsHeldInHand.OrderBy(x => Globals.randomNext(int.MaxValue)).Take(2).ToList();
                                    ZoneManager.DiscardZone.DrawTargetsFrom(ZoneManager.HandZone, toDisc);
                                }
                            }
                        }
                    });

                    return ret;
                }
            },
            {
                "THE TOOTH",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Tooth", "Lose $1 per card played.");

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.SelectedCardBeingConsideredForCalc,
                        MyAction = args =>
                        {
                            if (args is EngineCardChosenForPlayedHandArgs playArgs)
                            {
                                Globals.EmitMoneyLoss(1, playArgs.CardBeingConsidered, false);
                            }
                        }
                    });

                    return ret;
                }
            },
            {
                "THE MANACLE",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Manacle", "-1 Hand Size.");

                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.HandSize -= 1;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.HandSize += 1;
                    });

                    return ret;
                }
            },
            {
                "THE WHEEL",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Wheel", "1 in 7 cards get drawn face down during the round.");

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.CardDrawnToZone,
                        MyAction = args =>
                        {
                            if(args is EngineCardDrawnToZoneArgs drawArgs && drawArgs.ZoneDrawnTo == ZoneManager.HandZone && Globals.RollRandom(1, 7, c))
                            {
                                drawArgs.CardBeingDrawn.FaceDown = true;
                            }
                        },
                    });

                    return ret;
                }
            },
            {
                "THE PLANT",
                c => { return BuildDebuffBlind("The Plant", "Face cards are debuffed", c => EngineUtils.isFace(c)); }
            },
            {
                "THE CLUB",
                c => { return BuildSuitDebuffBlind("The Club", "All Club cards are debuffed", Suit.CLUBS); }
            },
            {
                "THE GOAD",
                c => { return BuildSuitDebuffBlind("The Goad", "All Spade cards are debuffed", Suit.SPADES); }
            },
            {
                "THE WINDOW",
                c => { return BuildSuitDebuffBlind("The Window", "All Diamond cards are debuffed", Suit.DIAMONDS); }
            },
            {
                "THE HEAD",
                c => { return BuildSuitDebuffBlind("The Head", "All Heart cards are debuffed", Suit.HEARTS); }
            },
            {
                "THE MARK",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Mark", "All face cards are drawn face down.");

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.CardDrawnToZone,
                        MyAction = args =>
                        {
                            if(args is EngineCardDrawnToZoneArgs drawArgs && drawArgs.ZoneDrawnTo == ZoneManager.HandZone && EngineUtils.isFace(drawArgs.CardBeingDrawn))
                            {
                                drawArgs.CardBeingDrawn.FaceDown = true;
                            }
                        },
                    });

                    return ret;
                }
            },
            {
                "THE HOUSE",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The House", "First hand is drawn face down.");
                    ret.DataDict.Add("FLIPFLAG", new JokerData() {MyDataType = JokerDataType.INT, IntData = 1});
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.CardDrawnToZone,
                        MyAction = args =>
                        {
                            if(args is EngineCardDrawnToZoneArgs drawArgs && drawArgs.ZoneDrawnTo == ZoneManager.HandZone && ret.DataDict["FLIPFLAG"].IntData == 1)
                            {
                                drawArgs.CardBeingDrawn.FaceDown = true;
                            }
                        },
                    });
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.DrawHandfulDone,
                        MyAction = args =>
                        {
                            ret.DataDict["FLIPFLAG"].IntData = 0;
                        },
                    });

                    return ret;
                }
            },
            {
                "THE FISH",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Fish", "Cards drawn facedown after each hand played.");
                    ret.DataDict.Add("FLIPFLAG", new JokerData() {MyDataType = JokerDataType.INT, IntData = 0});
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.CardDrawnToZone,
                        MyAction = args =>
                        {
                            if(args is EngineCardDrawnToZoneArgs drawArgs && drawArgs.ZoneDrawnTo == ZoneManager.HandZone && ret.DataDict["FLIPFLAG"].IntData == 1)
                            {
                                drawArgs.CardBeingDrawn.FaceDown = true;
                            }
                        },
                    });
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.DrawHandfulDone,
                        MyAction = args =>
                        {
                            ret.DataDict["FLIPFLAG"].IntData = 0;
                        },
                    });
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.HandPlayDone,
                        MyAction = args =>
                        {
                            ret.DataDict["FLIPFLAG"].IntData = 1;
                        },
                    });

                    return ret;
                }
            },
            {
                "THE ARM",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Arm", "Decrease level of played poker hand by 1");

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.HandPlayedCalculated,
                        MyAction = args =>
                        {
                            if(args is EngineHandPlayArgs playArgs)
                            {
                                ScoreHandler.LevelDownHand(playArgs.HandBeingPlayed);
                                EngineEventHandler.TriggerEvent(new EngineEventArgs() {MyContext = new() { Context = EventContextType.BossAbilityTriggeredByHand}});
                            }
                        },
                    });

                    return ret;
                }
            },
            {
                "THE PSYCHIC",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Psychic", "Must play 5 cards (not all cards need to score)");

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.AllScoringCardsDecided,
                        MyAction = args =>
                        {
                            if(args is EngineHandPlayArgs playArgs && playArgs.CardsSelected.Count < 5)
                            {
                                playArgs.CancelScoring = true;
                                EngineEventHandler.TriggerEvent(new EngineEventArgs() {MyContext = new() { Context = EventContextType.BossAbilityTriggeredByHand}});
                            }
                        },
                    });

                    return ret;
                }
            },
            {
                "THE MOUTH",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Mouth", "Only one hand type can be played this round");
                    ret.DataDict.Add("PLAYEDFLAG", new JokerData() {MyDataType = JokerDataType.INT, IntData = 0});
                    ret.DataDict.Add("CHOSENHAND", new JokerData() {MyDataType = JokerDataType.HANDTYPE, HandTypeData = PlayedHandType.HIGHCARD});

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.AllScoringCardsDecided,
                        MyAction = args =>
                        {
                            if(args is EngineHandPlayArgs playArgs && ret.DataDict["PLAYEDFLAG"].IntData == 1 && playArgs.HandBeingPlayed != ret.DataDict["CHOSENHAND"].HandTypeData)
                            {
                                playArgs.CancelScoring = true;
                                EngineEventHandler.TriggerEvent(new EngineEventArgs() {MyContext = new() { Context = EventContextType.BossAbilityTriggeredByHand}});
                            }
                        },
                    });

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.HandPlayDone,
                        MyAction = args =>
                        {
                            if(args is EngineHandPlayDoneArgs playArgs && ret.DataDict["PLAYEDFLAG"].IntData == 0)
                            {
                                ret.DataDict["PLAYEDFLAG"].IntData = 1;
                                ret.DataDict["CHOSENHAND"].HandTypeData = playArgs.HandTypeThatWasPlayed;
                            }
                        },
                    });

                    return ret;
                }
            },
            {
                "THE FLINT",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Flint", "Base Chips and Mult for played poker hands are halved for the entire round");

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.SettingBaseChipsMult,
                        MyAction = args =>
                        {
                            if(args is EngineSettingBaseHandScoreArgs scoreArgs)
                            {
                                scoreArgs.BaseChipAmount /= 2;
                                scoreArgs.BaseMultAmount /= 2;
                                EngineEventHandler.TriggerEvent(new EngineEventArgs() {MyContext = new() { Context = EventContextType.BossAbilityTriggeredByHand}});
                            }
                        },
                    });

                    return ret;
                }
            },
            {
                "THE SERPENT",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Serpent", "After Play or Discard, always draw 3 cards (ignores hand size)");
                    ret.DataDict.Add("FIRSTDRAWDONE", new JokerData() { MyDataType = JokerDataType.INT, IntData = 0});
                    ret.DataDict.Add("INTAMOUNT", new JokerData() { MyDataType = JokerDataType.INT, IntData = 3});

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.DrawHandfulStarted,
                        MyAction = args =>
                        {
                            if(args is EngineRedrawArgs redrawArgs && ret.DataDict["FIRSTDRAWDONE"].IntData == 1)
                            {
                                redrawArgs.ForcedRedrawAmount = ret.DataDict["INTAMOUNT"].IntData;
                            }
                        },
                    });
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.DrawHandfulDone,
                        MyAction = args =>
                        {
                            if(ret.DataDict["FIRSTDRAWDONE"].IntData == 0)
                            {
                                ret.DataDict["FIRSTDRAWDONE"].IntData = 1;
                            }
                        },
                        RemoveAfterTriggering = true,
                    });

                    return ret;
                }
            },
            {
                "THE EYE",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("The Eye", "No repeat hand types this round");

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.AllScoringCardsDecided,
                        MyAction = args =>
                        {
                            if(args is EngineHandPlayArgs playArgs && ScoreHandler.NumHandTypePlayedThisRound[playArgs.HandBeingPlayed] > 0)
                            {
                                playArgs.CancelScoring = true;
                                EngineEventHandler.TriggerEvent(new EngineEventArgs() {MyContext = new() { Context = EventContextType.BossAbilityTriggeredByHand}});
                            }
                        },
                    });

                    return ret;
                }
            },
            {
                "THE PILLAR",
                c => { return BuildDebuffBlind("The Pillar", "Cards played previously this Ante (during Small and Big Blinds) are debuffed", ShouldDebuffForPillar); }
            },
            {
                "VIOLET VESSEL",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Violet Vessel", "Very large blind.");

                    return ret;
                }
            },
            {
                "VERDANT LEAF",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Verdant Leaf", "All cards debuffed until 1 Joker sold.");

                    ret.DataDict.Add("DEBUFFFLAG", new JokerData() {MyDataType = JokerDataType.INT, IntData = 1});

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.CardDrawnToZone,
                        MyAction = args =>
                        {
                            if(args is EngineCardDrawnToZoneArgs drawArgs && drawArgs.ZoneDrawnTo == ZoneManager.HandZone && ret.DataDict["DEBUFFFLAG"].IntData == 1)
                            {
                                drawArgs.CardBeingDrawn.Debuffed = true;
                            }
                        },
                    });

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.CardSell,
                        MyAction = args =>
                        {
                            if(args is EngineCardSoldArgs sellArgs && sellArgs.CardBeingSold.isJoker)
                            {
                                ret.DataDict["DEBUFFFLAG"].IntData = 0;
                                foreach (var c in ZoneManager.HandZone.Cards)
                                {
                                    c.Debuffed = false;
                                }
                            }
                        }
                    });

                    return ret;
                }
            },
            {
                "AMBER ACORN",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Amber Acorn", "Flips and shuffles all Joker cards");
                    
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        foreach (var j in ZoneManager.JokerZone.Cards)
                        {
                            j.FaceDown = true;
	                    }
                        ZoneManager.JokerZone.Shuffle();
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        foreach (var j in ZoneManager.JokerZone.Cards)
                        {
                            j.FaceDown = false;
                        }
                    });

                    return ret;
                }
            },
            {
                "CRIMSON HEART",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Crimson Heart", "One random Joker disabled every hand");

                    ret.DataDict.Add("CARDTARGET", new JokerData() {MyDataType = JokerDataType.CARD});

                    Func<Card?, Card?> GetJoker = curTarget =>
                    {
                        var cardList = ZoneManager.JokerZone.Cards.ToList();
                        if (!cardList.Any())
                            return null;

                        if(cardList.Count == 1)
                            return cardList.Single();

                        if(curTarget != null && cardList.Contains(curTarget))
                        {
                            cardList.Remove(curTarget);
                        }

                        return cardList[Globals.randomNext(cardList.Count)];
                    };

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.StartPlayRound,
                        MyAction = _ =>
                        {
                            var target = GetJoker(null);
                            if(target != null)
                            {
                                target.Debuffed = true;
                                ret.DataDict["CARDTARGET"].CardData = target;
                            }
                        },
                    });

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.HandPlayDone,
                        MyAction = _ =>
                        {
                            if(ret.DataDict["CARDTARGET"].CardData != null)
                            {
                                ret.DataDict["CARDTARGET"].CardData.Debuffed = false;
                            }
                            var target = GetJoker(ret.DataDict["CARDTARGET"].CardData);
                            if(target != null)
                            {
                                target.Debuffed = true;
                                ret.DataDict["CARDTARGET"].CardData = target;
                            }
                        },
                    });

                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        if(ret.DataDict["CARDTARGET"].CardData != null)
                            ret.DataDict["CARDTARGET"].CardData.Debuffed = false;
                    });

                    return ret;
                }
            },
            {
                "CERULEAN BELL",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Cerulean Bell", "Forces 1 card in hand to always be selected.");

                    ret.DataDict.Add("CARDTARGET", new JokerData() {MyDataType = JokerDataType.CARD});

                    Func<Card?> GetSelected = () =>
                    {
                        var cardList = ZoneManager.HandZone.Cards.Where(x => !x.isSelected).ToList();
                        if (!cardList.Any())
                            return null;

                        if(cardList.Count == 1)
                            return cardList.Single();
                        
                        return cardList[Globals.randomNext(cardList.Count)];
                    };

                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.DrawHandfulDone,
                        MyAction = _ =>
                        {
                            var target = GetSelected();
                            if(target != null)
                            {
                                target.ToggleSelect();
                                target.ForcedSelect = true;
                            }
                        },
                    });

                    return ret;
                }
            },
        };

        //Give the passed card the data necessary to make it the named blind (DB NAME)
        public static void MakeCardBlind(Card c, string blindName)
        {
            if (!BossBlindData.ContainsKey(blindName))
                return;
            var toSet = BossBlindData[blindName](c);
            c.JokerData = toSet;
        }

        //Generate and return a fresh Card object that is the named blind (DB NAME)
        public static Card GenerateBlindCard(string blindName)
        {
            if (!BossBlindData.ContainsKey(blindName))
                throw new ArgumentException("Bad blind DB name passed: " + blindName);
            var c = new Card();
            MakeCardBlind(c, blindName);
            return c;
        }

        private static JokerCardDataBlock BuildSuitDebuffBlind(string name, string desc, Suit toDebuff) => BuildDebuffBlind(name, desc, c => c.IsSuit(toDebuff));

        private static JokerCardDataBlock BuildDebuffBlind(string name, string desc, Func<Card, bool> isValidTarget)
        {
            var ret = JokerDb.BasicDataBlock(name, desc);

            ret.Listeners.Add(new EngineEventListener()
            {
                MyContextType = EventContextType.CardDrawnToZone,
                MyAction = args =>
                {
                    if (args is EngineCardDrawnToZoneArgs drawArgs && drawArgs.ZoneDrawnTo == ZoneManager.HandZone && isValidTarget(drawArgs.CardBeingDrawn))
                    {
                        drawArgs.CardBeingDrawn.Debuffed = true;
                    }
                },
            });

            return ret;
        }

        private static bool ShouldDebuffForPillar(Card target)
        {
            return EngineEventHandler.SavedEvents.Any(args => args is EngineCardPreTriggerArgs triggerArgs
            && triggerArgs.CardAboutToTrigger == target
            && triggerArgs.numTriggersToDo > 0
            && triggerArgs.CurrentAnte == FlowHandler.CurrentAnte);
        }
    }
}
