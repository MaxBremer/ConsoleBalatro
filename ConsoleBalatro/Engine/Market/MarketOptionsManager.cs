using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Market
{
    public static class MarketOptionsManager
    {
        private static List<PlayedHandType> DefaultPlanetsAvailable = new()
        {
            PlayedHandType.HIGHCARD,
            PlayedHandType.PAIR,
            PlayedHandType.TWOPAIR,
            PlayedHandType.THREEOFAKIND,
            PlayedHandType.FOUROFAKIND,
            PlayedHandType.FLUSH,
            PlayedHandType.FULLHOUSE,
            PlayedHandType.STRAIGHT,
            PlayedHandType.STRAIGHTFLUSH,
        };
        public static Dictionary<BuyItemType, int> MainMarketWeights = new()
        {
            {BuyItemType.JOKER, 40 },
            {BuyItemType.TAROT_CARD, 5 },
            {BuyItemType.PLANET_CARD, 5 },
        };

        //NOTE: All chances are odds in 1000 (to do decimal odds with ints, for some reason, its how balatro does it)
        //We index first on item type in case later items have diff edition odds.
        public static Dictionary<BuyItemType, Dictionary<Edition, int>> RandomEditionOdds = new()
        {
            {BuyItemType.JOKER, new Dictionary<Edition, int>()
            {
                { Edition.FOIL, 20 },
                { Edition.HOLOGRAPHIC, 14 },
                { Edition.POLYCHROME, 3 },
                { Edition.NEGATIVE, 3 },
            } },
        };

        public static int MainMarketWeightsTotal => MainMarketWeights.Values.Sum();

        public static Dictionary<BuyItemType, CardZone> MarketPoolsToDrawFrom = new();

        public static void InitializeMarketPools()
        {
            //DO
        }

        public static void ShufflePools()
        {
            foreach (var pool in MarketPoolsToDrawFrom.Values)
            {
                pool.Shuffle();
            }
        }

        public static void AddToVoucherPool(string DBNameToAdd)
        {
            //DO
        }

        public static void AttemptToRemoveFromVoucherPool(string DBNameToRemove)
        {
            //DO
        }

        //returns whether it successfully returned the item.
        public static bool ReturnMarketItemFromZone(Card cardToReturn, CardZone zoneFrom)
        {
            //DO
            return false;
        }

        public static void DrawMarketItemOfTypeToConsumableZone(BuyItemType itemType)
        {
            DrawMarketItem(itemType, ZoneManager.ConsumableZone);
        }

        public static void DrawMarketItem(BuyItemType itemType, CardZone zoneToDrawTo, bool applyMarketModifiers = false)
        {
            //DO
        }

        public static void DrawMarketItems(List<BuyItemType> itemTypes, CardZone zoneToDrawTo)
        {
            foreach (var type in itemTypes)
            {
                DrawMarketItem(type, zoneToDrawTo);
            }
        }

        public static void DrawItemsByMainMarketOdds(int numItems, CardZone zoneToDrawTo, bool applyMarketModifiers = false)
        {
            //Simply draws based on market odds. Other tweaks (edition mainly) happen elsewhere (in DrawMarketItem).
            for(int i = 0; i < numItems; i++)
            {
                var roll = Random.Shared.Next(MainMarketWeightsTotal);
                BuyItemType chosenType = BuyItemType.NONE;
                foreach(var typeOpt in MainMarketWeights)
                {
                    if(roll < typeOpt.Value)
                    {
                        chosenType = typeOpt.Key;
                        break;
                    }
                    else
                    {
                        roll -= typeOpt.Value;
                    }
                }
                if(chosenType == BuyItemType.NONE)
                {
                    //TEMPORARILY: default to Joker
                    chosenType = BuyItemType.JOKER;
                }
                DrawMarketItem(chosenType, zoneToDrawTo, applyMarketModifiers);
            }
        }

        public static void DrawNumMarketItems(BuyItemType itemType, int itemNum, CardZone drawTo)
        {
            var passList = new List<BuyItemType>();
            for (int i = 0; i < itemNum; i++)
            {
                passList.Add(itemType);
            }
            DrawMarketItems(passList, drawTo);
        }
    }

        public enum BuyItemType
    {
        NONE,
        PLAYING_CARD,
        TAROT_CARD,
        PLANET_CARD,
        SPECTRAL_CARD,
        JOKER,
        VOUCHER,
    }
}
