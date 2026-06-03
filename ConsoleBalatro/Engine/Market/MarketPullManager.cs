using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Vouchers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
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
            [BuyItemType.PLAYING_CARD] = ItemPool.PlayingCard,
        };

        public static Dictionary<BuyItemType, int> MainMarketWeights = new()
        {
            {BuyItemType.JOKER, 40 },
            {BuyItemType.TAROT_CARD, 5 },
            {BuyItemType.PLANET_CARD, 5 },
        };

        public static void FillMainMarket()
        {
            var batch = new ContentRollBatchContext();
            while (ZoneManager.MainMarketZone.HasRoomIgnoreNegative)
            {
                var type = ChooseMarketRollType();
                DrawMarketItem(type, ZoneManager.MainMarketZone, batchContext: batch);
            }
        }

        public static BuyItemType ChooseMarketRollType()
        {
            var args = new EngineMarketTypeBeingChosenArgs
            {
                WeightsBeingRolled = MainMarketWeights.ToDictionary(),
                MyContext = new()
                {
                    Context = EventContextType.MarketTypeBeingChosen,
                }
            };
            EngineEventHandler.TriggerEvent(args);
            return ChooseRollItemByOdds(args.WeightsBeingRolled);
        }

        public static BuyItemType ChooseRollItemByOdds(Dictionary<BuyItemType, int> weights)
        {
            var maxRoll = weights.Values.Sum();
            var roll = Globals.ChooseRandomInclusive(0, maxRoll);

            foreach (var kvp in weights)
            {
                if (roll <= kvp.Value)
                    return kvp.Key;
                roll -= kvp.Value;
            }
            return BuyItemType.NONE;
        }

        public static void DrawNumMarketItems(BuyItemType itemType, int num, CardZone drawTo, bool applyMarketModifiers = false, bool overrideSpaceLimits = false, GenerationSource source = GenerationSource.Shop, ContentRollBatchContext batchContext = null)
        {
            for (int i = 0; i < num; i++)
            {
                DrawMarketItem(itemType, drawTo, applyMarketModifiers: applyMarketModifiers, overrideSpaceLimits: overrideSpaceLimits, source: source, batchContext: batchContext);
            }
        }

        public static void DrawMarketItem(BuyItemType itemType, CardZone zoneToDrawTo, bool applyMarketModifiers = false, bool overrideSpaceLimits = false, GenerationSource source = GenerationSource.Shop, ContentRollBatchContext batchContext = null)
        {
            //structure:
            //roll for option
            //returns RollableDefinition
            //Build ID of def 

            var ret = PickMarketCard(itemType, source, batchContext);

            //ADD MARKET MODIFIERS

            zoneToDrawTo.AddCard(ret, overrideSpace: overrideSpaceLimits);
        }

        public static Card PickMarketCard(BuyItemType itemType, GenerationSource source = GenerationSource.Shop, ContentRollBatchContext batchContext = null, JokerRarity? forcedRarity = null)
        {
            var poolType = TypeToPoolMappings[itemType];
            var req = new ContentRollRequest()
            {
                Pool = poolType,
                Source = source,
                Batch = batchContext,
                ForcedRarity = forcedRarity,
            };
            var resp = PoolManager.RollSingle(req);
            Card ret = null;
            switch (resp.Pool)
            {
                case ItemPool.Planet:
                    ret = ConsumableManager.MakePlanetCard(ConsumableManager.PlanetsToHandType[resp.Id]);
                    break;
                case ItemPool.Joker:
                    ret = new Card();
                    JokerDb.MakeCardJoker(ret, resp.Id);
                    break;
                case ItemPool.Tarot:
                    ret = ConsumableManager.MakeTarotCard(resp.Id);
                    break;
                case ItemPool.Spectral:
                    ret = ConsumableManager.MakeSpectralCard(resp.Id);
                    break;
                case ItemPool.Pack:
                    ret = ConsumableManager.MakePack(ConsumableManager.PackBasicNums.First(x => x.Value.ID == resp.Id).Key);
                    break;
                case ItemPool.Voucher:
                    ret = new Card();
                    VoucherDb.MakeCardVoucher(ret, resp.Id);
                    break;
                case ItemPool.PlayingCard:
                    ret = CardFactory.PlayingCardFromDefString(resp.Id);
                    break;
                default:
                    ret = new Card();
                    break;
            }

            return ret;
        }
    }
}
