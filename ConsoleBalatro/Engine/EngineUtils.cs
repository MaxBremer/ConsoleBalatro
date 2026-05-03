using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public static class EngineUtils
    {
        public const string BasicDeckString = "AS,2S,3S,4S,5S,6S,7S,8S,9S,1S,JS,QS,KS,AH,2H,3H,4H,5H,6H,7H,8H,9H,1H,JH,QH,KH,AC,2C,3C,4C,5C,6C,7C,8C,9C,1C,JC,QC,KC,AD,2D,3D,4D,5D,6D,7D,8D,9D,1D,JD,QD,KD";

        public static int LenStraight = 5;
        public static int LenFlush = 5;
        public static int SkipStrength = 0;

        private static Dictionary<PlayedHandType, List<PlayedHandType>> OtherHandsInsideDict = new()
        {
            { PlayedHandType.FIVEOFAKIND, new() {PlayedHandType.PAIR, PlayedHandType.THREEOFAKIND, PlayedHandType.FOUROFAKIND, PlayedHandType.TWOPAIR, PlayedHandType.FULLHOUSE} },
            { PlayedHandType.STRAIGHTFLUSH, new() {PlayedHandType.STRAIGHT, PlayedHandType.FLUSH} },
            { PlayedHandType.FLUSHFIVE, new() {PlayedHandType.PAIR, PlayedHandType.THREEOFAKIND, PlayedHandType.FOUROFAKIND, PlayedHandType.TWOPAIR, PlayedHandType.FULLHOUSE, PlayedHandType.FIVEOFAKIND, PlayedHandType.FLUSH} },
            { PlayedHandType.FLUSHHOUSE, new() {PlayedHandType.FLUSHHOUSE, PlayedHandType.FULLHOUSE, PlayedHandType.PAIR, PlayedHandType.THREEOFAKIND, PlayedHandType.TWOPAIR} },
            { PlayedHandType.FOUROFAKIND, new() {PlayedHandType.PAIR, PlayedHandType.THREEOFAKIND, PlayedHandType.TWOPAIR} },
            { PlayedHandType.FULLHOUSE, new() {PlayedHandType.PAIR, PlayedHandType.THREEOFAKIND, PlayedHandType.TWOPAIR} },
            { PlayedHandType.TWOPAIR, new() {PlayedHandType.PAIR} },
            { PlayedHandType.THREEOFAKIND, new() {PlayedHandType.PAIR} },
        };

        public static Dictionary<Edition, int> EditionCostIncreases = new()
        {
            { Edition.BASE, 0 },
            { Edition.FOIL, 2 },
            { Edition.HOLOGRAPHIC, 3 },
            { Edition.POLYCHROME, 5 },
            { Edition.NEGATIVE, 5 },
        };

        public static List<Rank> StandardOrderAceLow = new()
        {
            Rank.ACE,
            Rank.TWO,
            Rank.THREE,
            Rank.FOUR,
            Rank.FIVE,
            Rank.SIX,
            Rank.SEVEN,
            Rank.EIGHT,
            Rank.NINE,
            Rank.TEN,
            Rank.JACK,
            Rank.QUEEN,
            Rank.KING,
        };

        public static List<Rank> StandardOrderAceHigh = new()
        {
            Rank.TWO,
            Rank.THREE,
            Rank.FOUR,
            Rank.FIVE,
            Rank.SIX,
            Rank.SEVEN,
            Rank.EIGHT,
            Rank.NINE,
            Rank.TEN,
            Rank.JACK,
            Rank.QUEEN,
            Rank.KING,
            Rank.ACE,
        };

        public static Dictionary<Rank, int> RankBaseChipAmounts = new()
        {
            { Rank.NONE, 0 },
            { Rank.TWO, 2 },
            { Rank.THREE, 3 },
            { Rank.FOUR, 4 },
            { Rank.FIVE, 5 },
            { Rank.SIX, 6 },
            { Rank.SEVEN, 7 },
            { Rank.EIGHT, 8 },
            { Rank.NINE, 9 },
            { Rank.TEN, 10 },
            { Rank.JACK, 10 },
            { Rank.QUEEN, 10 },
            { Rank.KING, 10 },
            { Rank.ACE, 11 },
        };

        public static List<PlayedHandType> StandardHandOrder = new()
        {
            PlayedHandType.HIGHCARD,
            PlayedHandType.PAIR,
            PlayedHandType.TWOPAIR,
            PlayedHandType.THREEOFAKIND,
            PlayedHandType.STRAIGHT,
            PlayedHandType.FLUSH,
            PlayedHandType.FULLHOUSE,
            PlayedHandType.FOUROFAKIND,
            PlayedHandType.STRAIGHTFLUSH,
            PlayedHandType.FIVEOFAKIND,
            PlayedHandType.FLUSHHOUSE,
            PlayedHandType.FLUSHFIVE,
        };

        public static Dictionary<string, List<Rank>> RankGroups = new Dictionary<string, List<Rank>>()
        {
            { "NUMBERED", new() { Rank.TWO, Rank.THREE, Rank.FOUR, Rank.FIVE, Rank.SIX, Rank.SEVEN, Rank.EIGHT, Rank.NINE, Rank.TEN } },
            { "FACE", new() { Rank.JACK, Rank.QUEEN, Rank.KING } },
            { "ACE", new() { Rank.ACE } },
        };

        public static Dictionary<Edition, string> EditionDescriptors = new Dictionary<Edition, string>()
        {
            { Edition.NEGATIVE, "(+1 joker slot)" },
            { Edition.FOIL, "(+50 Chips)" },
            { Edition.HOLOGRAPHIC, "(+10 Mult)" },
            { Edition.POLYCHROME, "(X1.5 Mult)" },
        };

        public static bool isFace(Card c) => RankGroups["FACE"].Contains(c.Rank);

        public static (PlayedHandType, List<Card>) BestHandFromCards(List<Card> cards)
        {
            var ret = PlayedHandType.HIGHCARD;
            var retCards = new List<Card>();
            if(cards.Count <= 0)
            {
                return (ret, retCards);
            }
            retCards.Add(cards.OrderByDescending(x => x.Rank).First());
            bool hasStraight = false;
            bool hasFlush = false;
            bool hasFullHouse = false;

            var rankGroups = GroupByRank(cards);
            if(rankGroups.ContainsKey(Rank.NONE))
                rankGroups.Remove(Rank.NONE);
            var rankGroupLens = rankGroups.Select(x => (x.Key, x.Value.Count)).ToList();
            var rankGroupMax = rankGroupLens.Select(x => x.Count).Max();
            var suitGroups = GroupBySuit(cards);
            var suitGroupLens = suitGroups.Select(x => (x.Key, x.Value.Count)).ToList();
            var suitGroupMax = suitGroupLens.Select(x => x.Count).Max();

            if(rankGroupMax == 2)
            {
                ret = PlayedHandType.PAIR;
                retCards.Clear();
                var targetRank = rankGroupLens.Where(x => x.Count == 2).First().Key;
                retCards.AddRange(rankGroups[targetRank]);
            }

            if(rankGroupLens.Where(x => x.Count == 2).Count() >= 2)
            {
                ret = PlayedHandType.TWOPAIR;
                retCards.Clear();
                var rankGroupLensTarget = rankGroupLens.Where(x => x.Count == 2).ToList();
                retCards.AddRange(rankGroups[rankGroupLensTarget[0].Key]);
                retCards.AddRange(rankGroups[rankGroupLensTarget[1].Key]);
            }

            if (rankGroupMax == 3)
            {
                ret = PlayedHandType.THREEOFAKIND;
                retCards.Clear();
                var targetRank = rankGroupLens.Where(x => x.Count == 3).First().Key;
                retCards.AddRange(rankGroups[targetRank]);
            }

            var hasStraightPair = FullContainsStraightCheck(cards, SkipStrength);
            if (hasStraightPair.Item1)
            {
                ret = PlayedHandType.STRAIGHT;
                retCards.Clear();
                retCards.AddRange(hasStraightPair.Item2);
                hasStraight = true;
            }

            if(suitGroupMax >= LenFlush)
            {
                var targetSuit = suitGroupLens.Where(x => x.Count >= LenFlush).First().Key;
                retCards.Clear();
                retCards.AddRange(suitGroups[targetSuit]);
                ret = PlayedHandType.FLUSH;
                hasFlush = true;
            }

            if(rankGroupMax == 3 && rankGroupLens.Where(x => x.Count >= 2).Count() >= 2)
            {
                ret = PlayedHandType.FULLHOUSE;
                hasFullHouse = true;
                retCards.Clear();
                var orderedRankGroups = rankGroupLens.OrderByDescending(x => x.Count).Take(2).ToList();

                retCards.AddRange(rankGroups[orderedRankGroups[0].Key]);
                if (orderedRankGroups[0].Count == 3 && orderedRankGroups[1].Count == 3)
                {
                    retCards.AddRange(rankGroups[orderedRankGroups[1].Key].Take(2));
                }
                else
                {
                    retCards.AddRange(rankGroups[orderedRankGroups[1].Key]);
                }
            }

            if(rankGroupMax == 4)
            {
                ret = PlayedHandType.FOUROFAKIND;
                retCards.Clear();
                var targetRank = rankGroupLens.First(x => x.Count == 4).Key;
                retCards.AddRange(rankGroups[targetRank]);
            }

            if(hasStraight && hasFlush)
            {
                ret = PlayedHandType.STRAIGHTFLUSH;
                retCards.Clear();
                var targetSuit = suitGroupLens.Where(x => x.Count >= LenFlush).OrderByDescending(x => x.Count).First().Key;
                retCards.AddRange(hasStraightPair.Item2);
            }

            if(rankGroupMax >= 5)
            {
                ret = PlayedHandType.FIVEOFAKIND;
                retCards.Clear();
                var targetRank = rankGroupLens.Where(x => x.Count >= 5).OrderByDescending(x => x.Count).First().Key;
                retCards.AddRange(rankGroups[targetRank]);
            }

            if(hasFlush && hasFullHouse)
            {
                ret = PlayedHandType.FLUSHHOUSE;
                var targetSuit = suitGroupLens.Where(x => x.Count >= LenFlush).First().Key;
                retCards.Clear();
                retCards.AddRange(suitGroups[targetSuit]);
            }

            if(hasFlush && rankGroupMax >= 5)
            {
                ret = PlayedHandType.FLUSHFIVE;
                retCards.Clear();
                var targetRank = rankGroupLens.Where(x => x.Count >= 5).OrderByDescending(x => x.Count).First().Key;
                retCards.AddRange(rankGroups[targetRank]);
            }

            return (ret, retCards);
        }

        public static bool HandContainsOtherHand(PlayedHandType HandThatMightContain, PlayedHandType HandLookedFor)
        {
            return OtherHandsInside(HandThatMightContain).Contains(HandLookedFor);
        }

        public static List<PlayedHandType> OtherHandsInside(PlayedHandType hand)
        {
            var ret = new List<PlayedHandType>();

            ret.Add(hand);
            if(hand != PlayedHandType.HIGHCARD)
            {
                ret.Add(PlayedHandType.HIGHCARD);
            }
            if (OtherHandsInsideDict.ContainsKey(hand))
            {
                ret.AddRange(OtherHandsInsideDict[hand]);
            }

            return ret;
        }

        public static int NumCardsSelectedInHand => ZoneManager.CardsSelectedInHand.Count;
        
        public static (bool, List<Card>) FullContainsStraightCheck(List<Card> cards, int skipStrength = 0)
        {
            var checkOne = ContainsStraight(cards, StandardOrderAceHigh, skipStrength);
            var checkTwo = ContainsStraight(cards, StandardOrderAceLow, skipStrength);
            if (checkOne.Item1)
            {
                return checkOne;
            }else if (checkTwo.Item1)
            {
                return checkTwo;
            }
            else
            {
                return (false, new List<Card>());
            }
        }

        public static (bool, List<Card>) ContainsStraight(List<Card> cards, List<Rank> RankOrder, int skipStrength = 0)
        {
            if(LenStraight <= 1 && cards.Count > 0)
            {
                return (true, new List<Card>() { cards[0] });
            }

            var rankIndexMap = new Dictionary<Rank, int>();
            for(int i = 0; i < RankOrder.Count; i++)
            {
                if (!rankIndexMap.ContainsKey(RankOrder[i]))
                {
                    rankIndexMap[RankOrder[i]] = i;
                }
            }
            var cardsByRank = cards.Where(card => rankIndexMap.ContainsKey(card.Rank)).GroupBy(card => card.Rank).ToDictionary(g => g.Key, g => g.ToList());

            var cardIndices = cardsByRank.Keys.Select(rank => rankIndexMap[rank]).ToHashSet();

            for(int start = 0; start < RankOrder.Count; start++)
            {
                var sequence = new List<Rank>();
                int lastIndex = start;

                if (!cardIndices.Contains(lastIndex))
                {
                    continue;
                }
                else
                {
                    sequence.Add(RankOrder[lastIndex]);
                }

                for(int i = start + 1; i < RankOrder.Count && sequence.Count < LenStraight; i++)
                {
                    int gap = i - lastIndex;
                    if(gap - 1 > skipStrength)
                    {
                        break;
                    }
                    if (cardIndices.Contains(i))
                    {
                        sequence.Add(RankOrder[i]);
                        lastIndex = i;
                    }
                }

                if(sequence.Count >= LenStraight)
                {
                    var straightCards = new List<Card>();
                    var usedRanks = new HashSet<Rank>(sequence);

                    foreach(var rank in sequence)
                    {
                        if(cardsByRank.TryGetValue(rank, out var cardsOfThisRank))
                        {
                            var chosen = cardsOfThisRank.FirstOrDefault(c => !straightCards.Contains(c));
                            if(chosen != null)
                            {
                                straightCards.Add(chosen);
                            }
                        }
                    }

                    return (true, straightCards);
                }
            }

            return (false, new List<Card>());
        }

        public static void RandomizePlayingCard(Card c, List<Rank> validRanks = null, List<Suit> validSuits = null)
        {
            var ranksToUse = validRanks ?? Enum.GetValues(typeof(Rank)).Cast<Rank>().Where(r => r != Rank.NONE).ToList();
            var suitsToUse = validSuits ?? Enum.GetValues(typeof(Suit)).Cast<Suit>().Where(s => s != Suit.NONE).ToList();
            c.Rank = ranksToUse[Random.Shared.Next(ranksToUse.Count)];
            c.Suit = suitsToUse[Random.Shared.Next(suitsToUse.Count)];
        }

        private static Dictionary<Rank, List<Card>> GroupByRank(List<Card> cards)
        {
            return cards
            .GroupBy(c => c.Rank)
            .ToDictionary(g => g.Key, g => g.ToList());
        }

        private static Dictionary<Suit, List<Card>> GroupBySuit(List<Card> cards)
        {
            return cards
            .GroupBy(c => c.Suit)
            .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
