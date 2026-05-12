using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Blinds
{
    public static class BossBlindDb
    {
        public static List<string> BossBlindNames = new()
        {
            "PLAYONEHAND",
            "NODISCARD",
        };

        public static List<string> BossBlindsAlreadyUsed = new();

        public static List<string> AvailableBossBlinds => BossBlindNames.Where(x => !BossBlindsAlreadyUsed.Contains(x)).ToList();

        //NOTE!!!!!
        //ALL BOSS BLINDS need an OnJokerRemove that undoes their effect, such as adding hands to play if blind sets to one at start of round, etc.
        //This is for jokers Luchador and that one legendary that disable bosses.
        public static Dictionary<string, Func<Card, JokerCardDataBlock>> BossBlindData = new()
        {
            {
                "PLAYONEHAND",
                c =>
                {
                    var ret = new JokerCardDataBlock();
                    ret.JokerName = "PLAYONEHAND";
                    ret.DBName = "PLAYONEHAND";
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
                "NODISCARD",
                c =>
                {
                    var ret = new JokerCardDataBlock();
                    ret.JokerName = "NODISCARD";
                    ret.DBName = "NODISCARD";
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
