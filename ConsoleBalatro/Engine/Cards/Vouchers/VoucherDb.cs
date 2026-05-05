using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Vouchers
{
    public static class VoucherDb
    {
        public static Dictionary<string, string> VoucherDependants = new();

        public static Dictionary<string, Func<Card, JokerCardDataBlock>> VoucherData = new()
        {
            {
                "GRABBER",
                c =>
                {
                    var ret = VoucherDatablock("Grabber", nextVoucher: "NACHO TONG");
                    ret.DescriptionBuilder = _ => "Permanently gain +" + ret.DataDict["INTAMOUNT"].IntData + " hand per round.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxHandsPerRound += ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.MaxHandsPerRound -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "NACHO TONG",
                c =>
                {
                    var ret = VoucherDatablock("Nacho Tong", prevVoucher: "GRABBER");
                    ret.DescriptionBuilder = _ => "Permanently gain an additional +" + ret.DataDict["INTAMOUNT"].IntData + " hand per round.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxHandsPerRound += ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.MaxHandsPerRound -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "WASTEFUL",
                c =>
                {
                    var ret = VoucherDatablock("Wasteful", nextVoucher: "RECYCLOMANCY");
                    ret.DescriptionBuilder = _ => "Permanently gain +" + ret.DataDict["INTAMOUNT"].IntData + " discard each round.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxDiscardsPerRound += ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.MaxDiscardsPerRound -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "RECYCLOMANCY",
                c =>
                {
                    var ret = VoucherDatablock("Recyclomancy", prevVoucher: "WASTEFUL");
                    ret.DescriptionBuilder = _ => "Permanently gain an additional +" + ret.DataDict["INTAMOUNT"].IntData + " discard each round.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxDiscardsPerRound += ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.MaxDiscardsPerRound -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "SEED MONEY",
                c =>
                {
                    var ret = VoucherDatablock("Seed Money", nextVoucher: "MONEY TREE");
                    ret.DescriptionBuilder = _ => "Raise the cap on interest earned in each round to $10";
                    //NOTE: TECHNICALLY THE DESCRIPTION LIES.
                    //We don't set it to 10, we Increase it by 5.
                    //This makes it easier/adds clarity in the case of removing it, i.e. how to reset it, as aura effects are otherwise big tough.
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 5, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.CurMaxInterest += ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.CurMaxInterest -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "MONEY TREE",
                c =>
                {
                    var ret = VoucherDatablock("Money Tree", prevVoucher: "SEED MONEY");
                    ret.DescriptionBuilder = _ => "Raise the cap on interest earned in each round to $20";
                    //NOTE: TECHNICALLY THE DESCRIPTION LIES. See above voucher.
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 10, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.CurMaxInterest += ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.CurMaxInterest -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "BLANK",
                c =>
                {
                    var ret = VoucherDatablock("Blank", nextVoucher: "ANTIMATTER");
                    ret.DescriptionBuilder = _ => "Does nothing?";

                    return ret;
                }
            },
            {
                "ANTIMATTER",
                c =>
                {
                    var ret = VoucherDatablock("Antimatter", prevVoucher: "BLANK");
                    ret.DescriptionBuilder = _ => "+ " + ret.DataDict["INTAMOUNT"].IntData + " Joker slot.";

                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.JokerZone.MaxCapacity += ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        ZoneManager.JokerZone.MaxCapacity -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "HIEROGLYPH",
                c =>
                {
                    var ret = VoucherDatablock("Hieroglyph", nextVoucher: "PETROGLYPH");
                    ret.DescriptionBuilder = _ => "-" + ret.DataDict["INTAMOUNT"].IntData + " Ante, -" + ret.DataDict["INTAMOUNT"].IntData + " hand each round.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxHandsPerRound -= ret.DataDict["INTAMOUNT"].IntData;
                        FlowHandler.CurrentAnte -= ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.MaxHandsPerRound += ret.DataDict["INTAMOUNT"].IntData;
                        FlowHandler.CurrentAnte += ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "PETROGLYPH",
                c =>
                {
                    var ret = VoucherDatablock("Petroglyph", prevVoucher: "HIEROGLYPH");
                    ret.DescriptionBuilder = _ => "-" + ret.DataDict["INTAMOUNT"].IntData + " Ante, -" + ret.DataDict["INTAMOUNT"].IntData + " discard each round.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxDiscardsPerRound -= ret.DataDict["INTAMOUNT"].IntData;
                        FlowHandler.CurrentAnte -= ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.MaxDiscardsPerRound += ret.DataDict["INTAMOUNT"].IntData;
                        FlowHandler.CurrentAnte += ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "PAINT BRUSH",
                c =>
                {
                    var ret = VoucherDatablock("Paint Brush", nextVoucher: "PALETTE");
                    ret.DescriptionBuilder = _ => "+" + ret.DataDict["INTAMOUNT"].IntData + " Hand Size.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.HandSize += ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.HandSize -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "PALETTE",
                c =>
                {
                    var ret = VoucherDatablock("Palette", prevVoucher: "PAINT BRUSH");
                    ret.DescriptionBuilder = _ => "+" + ret.DataDict["INTAMOUNT"].IntData + " Hand Size again.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.HandSize += ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.HandSize -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "OVERSTOCK",
                c =>
                {
                    var ret = VoucherDatablock("Overstock", nextVoucher: "OVERSTOCK PLUS");
                    ret.DescriptionBuilder = _ => "+" + ret.DataDict["INTAMOUNT"].IntData + " card slot available in shop.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.MainMarketZone.MaxCapacity += ret.DataDict["INTAMOUNT"].IntData;
                        MarketGeneralManager.FillMainMarket();
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        ZoneManager.MainMarketZone.MaxCapacity -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "OVERSTOCK PLUS",
                c =>
                {
                    var ret = VoucherDatablock("Overstock Plus", prevVoucher: "OVERSTOCK");
                    ret.DescriptionBuilder = _ => "+" + ret.DataDict["INTAMOUNT"].IntData + " card slot available in shop.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 1, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.MainMarketZone.MaxCapacity += ret.DataDict["INTAMOUNT"].IntData;
                        MarketGeneralManager.FillMainMarket();
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        ZoneManager.MainMarketZone.MaxCapacity -= ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "REROLL SURPLUS",
                c =>
                {
                    var ret = VoucherDatablock("Reroll Surplus", nextVoucher: "REROLL GLUT");
                    ret.DescriptionBuilder = _ => "Rerolls cost $" + ret.DataDict["INTAMOUNT"].IntData + " less.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 2, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.BaseRerollCost -= ret.DataDict["INTAMOUNT"].IntData;
                        Globals.CurrentRerollCost -= ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.BaseRerollCost += ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "REROLL GLUT",
                c =>
                {
                    var ret = VoucherDatablock("Reroll Glut", prevVoucher: "REROLL SURPLUS");
                    ret.DescriptionBuilder = _ => "Rerolls cost an additional $" + ret.DataDict["INTAMOUNT"].IntData + " less.";
                    ret.DataDict.Add("INTAMOUNT", new JokerData() {IntData = 2, MyDataType = JokerDataType.INT});
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.BaseRerollCost -= ret.DataDict["INTAMOUNT"].IntData;
                        Globals.CurrentRerollCost -= ret.DataDict["INTAMOUNT"].IntData;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.BaseRerollCost += ret.DataDict["INTAMOUNT"].IntData;
                    });

                    return ret;
                }
            },
            {
                "CLEARANCE SALE",
                c =>
                {
                    var ret = VoucherDatablock("Clearance Sale", nextVoucher: "LIQUIDATION");
                    ret.DescriptionBuilder = _ => "All cards and packs in the shop are 25% off.";
                    //NOTE: AGAIN TECHNICALLY DESCRIPTION IS A LIE. SEE SEED MONEY COMMENT FOR DETAILS, SAME KINDA THING.
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.DiscountMultiplier -= 0.25;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.DiscountMultiplier += 0.25;
                    });

                    return ret;
                }
            },
            {
                "LIQUIDATION",
                c =>
                {
                    var ret = VoucherDatablock("Liquidation", prevVoucher: "CLEARANCE SALE");
                    ret.DescriptionBuilder = _ => "All cards and packs in the shop are 50% off.";
                    //NOTE: AGAIN TECHNICALLY DESCRIPTION IS A LIE. SEE ABOVE
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.DiscountMultiplier -= 0.25;
                    });
                    ret.OnJokerRemovalEffs.Add(() =>
                    {
                        Globals.DiscountMultiplier += 0.25;
                    });

                    return ret;
                }
            },
        };

        public static List<string> VoucherDBNames => VoucherData.Keys.ToList();

        //Give the passed card the data necessary to make it the named Voucher (DB NAME)
        public static void MakeCardVoucher(Card c, string VoucherName)
        {
            var toSet = VoucherData[VoucherName](c);
            c.JokerData = toSet;
            c.JokerData.MyCard = c;
            c.JokerData.isVoucher = true;
            c.JokerData.isJoker = false;
            c.BaseCost = 10; //Voucher all base cost is 10? I think?
        }

        //Generate and return a fresh Card object that is the named Voucher (DB NAME)
        public static Card GenerateVoucherCard(string VoucherName)
        {
            var c = new Card();
            MakeCardVoucher(c, VoucherName);
            return c;
        }

        private static JokerCardDataBlock VoucherDatablock(string name, string nextVoucher = "", string prevVoucher = "")
        {
            var ret = new JokerCardDataBlock();
            ret.JokerName = name;
            ret.DBName = name.ToUpper();
            if(!string.IsNullOrEmpty(nextVoucher))
                ret.SuccessorVoucherDBName = nextVoucher;
            if (!string.IsNullOrEmpty(prevVoucher))
                ret.PredecessorVoucherDBName = prevVoucher;
            else
                ret.voucherIsBase = true;
            return ret;
        }
    }
}
