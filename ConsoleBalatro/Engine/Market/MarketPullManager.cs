using ConsoleBalatro.Engine.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Market
{
    public static class MarketPullManager
    {
        public static Dictionary<BuyItemType, ItemPool> TypeToPoolMappings = new Dictionary<BuyItemType, ItemPool>()
        {
            [BuyItemType.TAROT_CARD] = ItemPool.Tarot,
            [BuyItemType.SPECTRAL_CARD] = ItemPool.Spectral,
            [BuyItemType.PLANET_CARD] = ItemPool.Planet,
            [BuyItemType.JOKER] = ItemPool.Joker,
            [BuyItemType.VOUCHER] = ItemPool.Voucher,
        };

        public static void DrawMarketItem(BuyItemType itemType, CardZone zoneToDrawTo, bool applyMarketModifiers = false, bool overrideSpaceLimits = false)
        {

        }

        
    }
}
