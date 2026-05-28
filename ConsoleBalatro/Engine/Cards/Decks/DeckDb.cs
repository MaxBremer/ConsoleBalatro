using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Decks
{
    public static class DeckDb
    {
        public static Dictionary<string, Func<Card, JokerCardDataBlock>> DeckData = new()
        {
            {
                "RED",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Red", "+1 discard every round");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxDiscardsPerRound += 1;
                    });

                    return ret;
                }
            },
            {
                "BLUE",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Blue", "+1 hand every round");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxHandsPerRound += 1;
                    });

                    return ret;
                }
            },
            {
                "YELLOW",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Yellow", "Start with extra $10");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.EmitMoneyGain(10, c);
                    });

                    return ret;
                }
            },
            {
                "BLACK",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Black", "+1 Joker slot, -1 hand every round");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxHandsPerRound -= 1;
                        ZoneManager.JokerZone.MaxCapacity += 1;
                    });

                    return ret;
                }
            },
            {
                "MAGIC",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Magic", "Start run with the Crystal Ball voucher and 2 copies of The Fool");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard("CRYSTAL BALL"));
                        for (int i = 0; i < 2; i++)
                            ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeTarotCard("FOOL"));
                    });

                    return ret;
                }
            },
            {
                "NEBULA",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Nebula", "Start run with the Telescope voucher, -1 consumable slot");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.ConsumableZone.MaxCapacity -= 1;
                        ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard("TELESCOPE"));
                    });

                    return ret;
                }
            },
            {
                "ABANDONED",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Abandoned", "Start run with no Face Cards in your deck.");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.DeckZone.Cards.RemoveAll(c => EngineUtils.isFace(c));
                    });

                    return ret;
                }
            },
            {
                "CHECKERED",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Checkered", "Start run with 26 Spades and 26 Hearts in your deck");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        foreach (var c in ZoneManager.DeckZone.Cards)
                        {
                            if(c.Suit == Enums.Suit.DIAMONDS)
                            {
                                c.Suit = Enums.Suit.HEARTS;
                            }else if(c.Suit == Enums.Suit.CLUBS)
                            {
                                c.Suit = Enums.Suit.SPADES;
                            }
                        }
                    });

                    return ret;
                }
            },
            {
                "ZODIAC",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Zodiac", "Start run with Tarot Merchant, Planet Merchant, and Overstock.");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard("TAROT MERCHANT"));
                        ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard("PLANET MERCHANT"));
                        ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard("OVERSTOCK"));
                    });

                    return ret;
                }
            },
            {
                "PAINTED",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Painted", "+2 hand size, -1 Joker slot");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.HandSize += 2;
                        ZoneManager.JokerZone.MaxCapacity -= 1;
                    });

                    return ret;
                }
            },
            {
                "ERRATIC",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Erratic", "All Ranks and Suits in deck are randomized");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        foreach (var c in ZoneManager.DeckZone.Cards)
                        {
                            EngineUtils.RandomizePlayingCard(c);
	                    }
                    });

                    return ret;
                }
            },
        };
    }
}
