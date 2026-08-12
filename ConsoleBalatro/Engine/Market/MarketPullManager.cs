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
        private static Dictionary<BuyItemType, ItemPool> TypeToPoolMappings = new Dictionary<BuyItemType, ItemPool>()
        {
            [BuyItemType.TAROT_CARD] = ItemPool.Tarot,
            [BuyItemType.SPECTRAL_CARD] = ItemPool.Spectral,
            [BuyItemType.PLANET_CARD] = ItemPool.Planet,
            [BuyItemType.JOKER] = ItemPool.Joker,
            [BuyItemType.VOUCHER] = ItemPool.Voucher,
            [BuyItemType.PLAYING_CARD] = ItemPool.PlayingCard,
        };

        /// <summary>
        /// The default, main market roll weights of different types.
        /// </summary>
        public static Dictionary<BuyItemType, int> MainMarketWeights = new()
        {
            {BuyItemType.JOKER, 40 },
            {BuyItemType.TAROT_CARD, 5 },
            {BuyItemType.PLANET_CARD, 5 },
        };

        /// <summary>
        /// Fill the main market zone with rolled items.
        /// </summary>
        public static void FillMainMarket()
        {
            var batch = new ContentRollBatchContext();
            while (ZoneManager.MainMarketZone?.HasRoomIgnoreNegative ?? false)
            {
                var type = ChooseMarketRollType();
                DrawMarketItem(type, ZoneManager.MainMarketZone, batchContext: batch);
            }
        }

        /// <summary>
        /// Chooses an item type for an individual roll in main market.
        /// </summary>
        /// <returns>The chosen BuyItemType.</returns>
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

        /// <summary>
        /// Chooses a buy item to roll given a set of weights.
        /// </summary>
        /// <param name="weights">The weights to use for the roll</param>
        /// <returns>The rolled item.</returns>
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

        //TODO: CLEANUP. A lot of this is redundant, i.e. market modifiers would no longer be applied here.
        /// <summary>
        /// Draw a passed number of rolled market items to a given zone.
        /// </summary>
        /// <param name="itemType">The type of item(s) to draw</param>
        /// <param name="num">The number of item to draw</param>
        /// <param name="drawTo">The zone to draw the items to</param>
        /// <param name="applyMarketModifiers">Whether to apply "modifiers" to rolled items (such as enhancement, seal etc)</param>
        /// <param name="overrideSpaceLimits">Whether to ignore space limits on the zone to draw to.</param>
        /// <param name="source">The source of the generation request (market roll, pack opening etc)</param>
        /// <param name="batchContext">The context for this batch of rolls.</param>
        public static void DrawNumMarketItems(BuyItemType itemType, int num, CardZone drawTo, bool applyMarketModifiers = false, bool overrideSpaceLimits = false, GenerationSource source = GenerationSource.Shop, ContentRollBatchContext? batchContext = null)
        {
            for (int i = 0; i < num; i++)
            {
                DrawMarketItem(itemType, drawTo, applyMarketModifiers: applyMarketModifiers, overrideSpaceLimits: overrideSpaceLimits, source: source, batchContext: batchContext);
            }
        }

        /// <summary>
        /// Draw a rolled market item to a given zone.
        /// </summary>
        /// <param name="itemType">The type of item to draw</param>
        /// <param name="zoneToDrawTo">The zone to draw the items to</param>
        /// <param name="applyMarketModifiers">Whether to apply "modifiers" to the rolled item (such as enhancement, seal etc)</param>
        /// <param name="overrideSpaceLimits">Whether to ignore space limits on the zone to draw to.</param>
        /// <param name="source">The source of the generation request (market roll, pack opening etc)</param>
        /// <param name="batchContext">The context for this batch of rolls.</param>
        public static void DrawMarketItem(BuyItemType itemType, CardZone zoneToDrawTo, bool applyMarketModifiers = false, bool overrideSpaceLimits = false, GenerationSource source = GenerationSource.Shop, ContentRollBatchContext? batchContext = null)
        {
            //structure:
            //roll for option
            //returns RollableDefinition
            //Build ID of def 

            var ret = PickMarketCard(itemType, source, batchContext);

            //ADD MARKET MODIFIERS

            zoneToDrawTo.AddCard(ret, overrideSpace: overrideSpaceLimits);
        }

        /// <summary>
        /// Return a rolled card based on passed specifications.
        /// </summary>
        /// <param name="itemType">The type of item to roll.</param>
        /// <param name="source">The source of this roll request, i.e. market, pack etc.</param>
        /// <param name="batchContext">The batch context for this particular roll.</param>
        /// <param name="forcedRarity">The optional forced rarity override, forces a certain rarity of item to be returned.</param>
        /// <returns>A card rolled based on passed specifications.</returns>
        public static Card PickMarketCard(BuyItemType itemType, GenerationSource source = GenerationSource.Shop, ContentRollBatchContext? batchContext = null, JokerRarity? forcedRarity = null)
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
            Card ret = new Card();
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
                    break;
            }

            var args = new EngineCardRollGeneratedArgs
            {
                FinalCardRolled = ret,
                RollMade = resp,
                RollRequest = req
            };
            EngineEventHandler.TriggerEvent(args);

            return ret;
        }
    }
}
