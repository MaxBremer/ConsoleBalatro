using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Jokers
{
    public static class JokerDb
    {
        //Costs of jokers... should prob just include in the db.
        //Or maybe better to separate this stuff out, idk.
        public static Dictionary<string, int> JokerCosts = new()
        {
            {"JIMBO", 2 },
            {"GREEDY JOKER", 5 },
            {"LSTY JOKER", 5 },
            {"WRATHFUL JOKER", 5 },
            {"GLUTTONOUS JOKER", 5 },
            {"JOLLY JOKER", 3 },
            {"ZANY JOKER", 4 },
            {"MAD JOKER", 4 },
            {"CRAZY JOKER", 4 },
            {"DROLL JOKER", 4 },
            {"SLY JOKER", 3 },
            {"WILY JOKER", 4 },
            {"CLEVER JOKER", 4 },
            {"DEVIOUS JOKER", 4 },
            {"CRAFTY JOKER", 4 },
            {"HALF JOKER", 5 },
            {"STENCIL JOKER", 8 },
            {"FOUR FINGERS", 7 },
            {"MIME", 5 },
            {"CREDIT CARD", 1 },
            {"GOLDEN JOKER", 5 },
        };

        public static List<string> JokerDbNames => JokerData.Keys.ToList();

        //Define our jokers by functions that build them out of a passed card.
        public static Dictionary<string, Func<Card, JokerCardDataBlock>> JokerData = new()
        {
            //DO: POPULATE THIS
        };

        public static void MakeCardJoker(Card c, string JokerName)
        {
            var toSet = JokerData[JokerName](c);
            c.JokerData = toSet;
            c.JokerData.MyCard = c;
            if (JokerCosts.ContainsKey(JokerName))
            {
                c.BaseCost = JokerCosts[JokerName];
            }
        }

        //Generate and return a fresh Card object that is the named joker (DB NAME)
        public static Card GenerateJokerCard(string JokerName)
        {
            var c = new Card();
            MakeCardJoker(c, JokerName);
            return c;
        }
    }
}
