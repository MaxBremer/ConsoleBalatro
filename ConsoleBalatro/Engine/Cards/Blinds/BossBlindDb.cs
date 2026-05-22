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

        public static List<string> AvailableBossBlinds => BossBlindNames.Where(x => !BossBlindsAlreadyUsed.Contains(x)).ToList();

        //NOTE!!!!!
        //ALL BOSS BLINDS need an OnJokerRemove that undoes their effect, such as adding hands to play if blind sets to one at start of round, etc.
        //This is for jokers Luchador and that one legendary that disable bosses.
        public static Dictionary<string, Func<Card, JokerCardDataBlock>> BossBlindData = new()
        {
            {
                "THE NEEDLE",
                c =>
                {
                    var ret = new JokerCardDataBlock();
                    ret.JokerName = "The Needle";
                    ret.DBName = "THE NEEDLE";
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
                    var ret = new JokerCardDataBlock();
                    ret.JokerName = "The Water";
                    ret.DBName = "THE WATER";
                    ret.DescriptionBuilder = _ => "No discards this round.";
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
                    var ret = new JokerCardDataBlock();
                    ret.JokerName = "The Ox";
                    ret.DBName = "THE OX";
                    ret.DescriptionBuilder = _ => "Playing the most played hand this run sets money to $0";
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
                    var ret = new JokerCardDataBlock();
                    ret.JokerName = "The Wall";
                    ret.DBName = "THE WALL";
                    ret.DescriptionBuilder = _ => "Extra large blind.";

                    return ret;
                }
            },
            {
                "VIOLET VESSEL",
                c =>
                {
                    var ret = new JokerCardDataBlock();
                    ret.JokerName = "Violet Vessel";
                    ret.DBName = "VIOLET VESSEL";
                    ret.DescriptionBuilder = _ => "Very large blind.";

                    return ret;
                }
            },
            {
                "THE HOOK",
                c =>
                {
                    var ret = new JokerCardDataBlock();
                    ret.JokerName = "The Hook";
                    ret.DBName = "THE HOOK";
                    ret.DescriptionBuilder = _ => "Discard 2 random cards in hand after each hand played.";
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
    }
}
