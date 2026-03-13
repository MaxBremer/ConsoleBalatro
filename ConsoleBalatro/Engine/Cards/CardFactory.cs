using ConsoleBalatro.Engine.Cards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards
{
    public static class CardFactory
    {
        public static Dictionary<string, Rank> StringToRank = new()
        {
            {"A", Rank.ACE },
            {"2", Rank.TWO },
            {"3", Rank.THREE },
            {"4", Rank.FOUR },
            {"5", Rank.FIVE },
            {"6", Rank.SIX },
            {"7", Rank.SEVEN },
            {"8", Rank.EIGHT },
            {"9", Rank.NINE },
            {"1", Rank.TEN },
            {"J", Rank.JACK },
            {"Q", Rank.QUEEN },
            {"K", Rank.KING },
        };

        public static Dictionary<string, Suit> StringToSuit = new()
        {
            {"S", Suit.SPADES },
            {"C", Suit.CLUBS },
            {"D", Suit.DIAMONDS },
            {"H", Suit.HEARTS },
        };

        public static Dictionary<Suit, string> SuitToString = StringToSuit.ToDictionary(x => x.Value, x => x.Key);
        public static Dictionary<Rank, string> RankToString = StringToRank.ToDictionary(x => x.Value, x => x.Key);

        public static Card PlayingCardFromDefString(string def)
        {
            var args = def.Split(":");
            var basics = args[0].ToUpper();
            return PlayingCardFromRankSuit(StringToRank[basics[0].ToString().ToUpper()], StringToSuit[basics[1].ToString().ToUpper()]);
        }

        public static Card PlayingCardFromRankSuit(Rank r, Suit s)
        {
            var ret = new Card()
            {
                Rank = r,
                Suit = s
            };
            ret.SetChipsFromRank();
            return ret;
        }

        public static List<Card> CardListFromDefString(string def, string splitter)
        {
            return def.Split(splitter).Select(x => PlayingCardFromDefString(x)).ToList();
        }
    }
}
