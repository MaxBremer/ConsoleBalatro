using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Consumables
{
    public static class ConsumableManager
    {
        public class PackData
        {
            public int BasePackPrice;
            public int NumOptionsPresented;
            public int NumCanBeTaken;
            public int ChanceToAppear; //This is out of 10,000 (for percentages).
            public BuyItemType RelevantBuyItemType = BuyItemType.NONE;
            public string PackName;
            public string TopLine;
            public string BottomLine;
            public string GetItemTypeString()
            {
                if (ItemTypeString.ContainsKey(RelevantBuyItemType))
                {
                    return ItemTypeString[RelevantBuyItemType];
                }
                return "";
            }
        }

        public static Dictionary<BuyItemType, string> ItemTypeString = new()
        {
            {BuyItemType.JOKER, "Jokers to obtain." },
            {BuyItemType.PLAYING_CARD, "Playing Cards to add to your deck." },
            {BuyItemType.TAROT_CARD, "Tarot Cards to use immediately." },
            {BuyItemType.PLANET_CARD, "Planet Cards to use immediately." },
            {BuyItemType.SPECTRAL_CARD, "Spectral Cards to use immediately." },
        };

        public static Dictionary<PackType, PackData> PackBasicNums = new()
        {
            //DO: POPULATE THIS
        };

        public static int PackTotalOdds => PackBasicNums.Values.Select(x => x.ChanceToAppear).Sum();

        public static List<string> SpectralNames => SpectralConsumablesDb.Keys.ToList();
        public static Dictionary<string, Func<Card, ConsumableCardDataBlock>> SpectralConsumablesDb = new()
        {
            {"TEST", c =>
            {
                var ret = new ConsumableCardDataBlock();
                ret.ConsumableName = "Test";
                ret.DBName = "TEST";

                return ret;
            } },
        };

        public static List<string> TarotNames => TarotConsumableDb.Keys.ToList();
        public static Dictionary<string, Func<Card, ConsumableCardDataBlock>> TarotConsumableDb = new()
        {
            //DO: POPULATE THIS
        };

        //Creating a planet card is a bit simpler, just take its name and hand-type and you're good.
        public static Dictionary<PlayedHandType, string> PlanetCardNames = new()
        {
            {PlayedHandType.HIGHCARD, "Pluto" },
            {PlayedHandType.PAIR, "Mercury" },
            {PlayedHandType.TWOPAIR, "Uranus" },
            {PlayedHandType.THREEOFAKIND, "Venus" },
            {PlayedHandType.FOUROFAKIND, "Mars" },
            {PlayedHandType.FLUSH, "Jupiter" },
            {PlayedHandType.FULLHOUSE, "Earth" },
            {PlayedHandType.STRAIGHT, "Saturn" },
            {PlayedHandType.STRAIGHTFLUSH, "Neptune" },
            {PlayedHandType.FIVEOFAKIND, "Planet X" },
            {PlayedHandType.FLUSHFIVE, "Eris" },
            {PlayedHandType.FLUSHHOUSE, "Ceres" },
        };

        //Use a consumable card, given the card and the zone it's from.
        //TODO: shouldn't the cards MyZone field suffice?
        //I think it's cause I'm not consistently setting MyZone well. Def should fix that. TODO.
        public static void UseConsumable(Card c, CardZone zoneFrom)
        {
            if(!(ZoneManager.ConsumableZone.Cards.Contains(c) || ZoneManager.PackOptionZone.Cards.Contains(c) || ZoneManager.MainMarketZone.Cards.Contains(c)))
            {
                //ERROR: card not in consumable, pack options, OR main market zone (buy and use).
                //TODO: Maybe remove this? Like the modded thing where playing cards can be turned into consumables.
                return;
            }
            ZoneManager.CurrentlyActivatingConsumable.DrawTargetFrom(zoneFrom, c);
            if (!c.isConsumable)
            {
                //ERROR: card not a consumable.
                return;
            }
            //TODO: Any args to pass here?
            if (!c.ConsumableData.IsActivatable(null))
            {
                //ERROR: Not activatable;
                return;
            }

            //TODO: Again, args to pass?
            c.ConsumableData.Use(null);

            var args = new EngineConsumableUseArgs();
            args.MyContext = new Events.EventContext() { Context = Events.EventContextType.ConsumableUsed };
            args.ConsumableName = c.ConsumableData.ConsumableName;
            args.ConsumableDBName = c.ConsumableData.DBName;
            args.HandOfItemUsed = c.ConsumableData.PlanetHandType;
            args.TypeUsed = c.ConsumableData.Type;
            args.BuyItemUsed = c.ConsumableData.BuyType;
            EngineEventHandler.TriggerEvent(args);

            MarketOptionsManager.ReturnMarketItemFromZone(c, ZoneManager.CurrentlyActivatingConsumable);
        }

        public static Card MakeTarotCard(string TarotCardDbName)
        {
            if (!TarotConsumableDb.ContainsKey(TarotCardDbName))
            {
                return null;
            }
            var c = new Card();
            MakeCardTarotCard(TarotCardDbName, c);
            return c;
        }

        public static Card MakeSpectralCard(string SpectralCardDbName)
        {
            if (!SpectralNames.Contains(SpectralCardDbName))
            {
                return null;
            }
            var c = new Card();
            MakeCardSpectralCard(SpectralCardDbName, c);
            return c;
        }

        //Give the passed card the data necessary to transform it into the named tarot card (DB NAME)
        public static void MakeCardTarotCard(string TarotCardDbName, Card target)
        {
            if (!TarotConsumableDb.ContainsKey(TarotCardDbName))
            {
                return;
            }
            var retDataBlock = TarotConsumableDb[TarotCardDbName](target);
            retDataBlock.Type = ConsumableType.TAROT;
            retDataBlock.BuyType = BuyItemType.TAROT_CARD;
            retDataBlock.MyCard = target;
            target.ConsumableData = retDataBlock;
            target.BaseCost = 3;//default tarot cost.
        }

        //Give the passed card the data necessary to transform it into the named spectral card (DB NAME)
        public static void MakeCardSpectralCard(string SpectralCardDbName, Card target)
        {
            if (!SpectralConsumablesDb.ContainsKey(SpectralCardDbName))
            {
                return;
            }
            var retDataBlock = SpectralConsumablesDb[SpectralCardDbName](target);
            retDataBlock.Type = ConsumableType.SPECTRAL;
            retDataBlock.BuyType = BuyItemType.SPECTRAL_CARD;
            retDataBlock.MyCard = target;
            target.ConsumableData = retDataBlock;
            target.BaseCost = 4;//default spectral cost.
        }

        //Generate and return a card that is a planet card for the passed hand type.
        public static Card MakePlanetCard(PlayedHandType hand)
        {
            var ret = new Card();
            MakeCardPlanetCard(hand, ret);
            return ret;
        }

        //Generate and return a card that is a pack of the passed type
        public static Card MakePack(PackType packType)
        {
            var ret = new Card
            {
                MyPackType = packType
            };
            ret.BaseCost = PackBasicNums[ret.MyPackType].BasePackPrice;
            return ret;
        }

        public static Card MakePackByOdds()
        {
            PackType chosenPackType = PackType.BASIC_JOKER;
            var roll = Random.Shared.Next(PackTotalOdds);
            foreach (var kv in PackBasicNums)
            {
                if(roll < kv.Value.ChanceToAppear)
                {
                    chosenPackType = kv.Key;
                    break;
                }
                else
                {
                    roll -= kv.Value.ChanceToAppear;
                }
            }
            return MakePack(chosenPackType);
        }

        //Give the passed card the data necessary to transform it into planet card for the passed hand type.
        public static void MakeCardPlanetCard(PlayedHandType hand, Card target)
        {
            var dataBlock = new ConsumableCardDataBlock
            {
                ConsumableName = PlanetCardNames[hand],
                Type = ConsumableType.PLANET,
                BuyType = BuyItemType.PLANET_CARD,
                PlanetHandType = hand,
            };
            dataBlock.DataDict.Add("HANDTYPE", new Jokers.JokerData() { MyDataType = Jokers.JokerDataType.HANDTYPE, HandTypeData = hand });
            dataBlock.DescriptionBuilder = _ => "Level up hand " + dataBlock.DataDict["HANDTYPE"].HandTypeData.ToString();
            dataBlock.Use = _ =>
            {
                ScoreHandler.LevelUpHand(dataBlock.DataDict["HANDTYPE"].HandTypeData);
            };
            dataBlock.MyCard = target;
            target.ConsumableData = dataBlock;
            target.BaseCost = 3; //default planet card cost.
        }

        private static bool EvaluateCardForFool(EngineEventArgs args) => args is EngineConsumableUseArgs conArgs && (conArgs.TypeUsed == ConsumableType.TAROT || conArgs.TypeUsed == ConsumableType.PLANET);
    }
}
