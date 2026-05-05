using ConsoleBalatro.Engine.Cards.Jokers;
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
                    var ret = VoucherDatablock("Grabber");
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
                "DISCARDSGAINONE",
                c =>
                {
                    var ret = new JokerCardDataBlock();
                    ret.JokerName = "Gain 1 discard";
                    ret.DBName = "DISCARDSGAINONE";
                    ret.DescriptionBuilder = _ => "+ " + ret.DataDict["INTAMOUNT"].IntData + " discards.";
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

        private static JokerCardDataBlock VoucherDatablock(string name)
        {
            var ret = new JokerCardDataBlock();
            ret.JokerName = name;
            ret.DBName = name.ToUpper();
            return ret;
        }
    }
}
