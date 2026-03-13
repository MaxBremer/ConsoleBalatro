using ConsoleBalatro.Engine.Cards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Jokers
{
    public class JokerData
    {
        public JokerDataType MyDataType;
        public int IntData;
        public double DoubleData;
        public bool BoolData;
        public PlayedHandType HandTypeData;
        public Rank SpecificCardRank;
        public Suit SpecificCardSuit;

        public string GetDataString()
        {
            switch(MyDataType)
            {
                case JokerDataType.INT:
                    return IntData.ToString();
                case JokerDataType.DOUBLE:
                    return DoubleData.ToString();
                case JokerDataType.BOOL:
                    return BoolData.ToString();
                case JokerDataType.HANDTYPE:
                    return HandTypeData.ToString();
                case JokerDataType.SPECIFICCARD:
                    return SpecificCardRank.ToString() + " OF " + SpecificCardSuit.ToString();
                case JokerDataType.SUIT:
                    return SpecificCardSuit.ToString();
                case JokerDataType.RANK:
                    return SpecificCardRank.ToString();
                default:
                    return "Unknown Data Type";
            }
        }

        public JokerData MakeCopy() {  
            return new JokerData()
            {
                MyDataType = MyDataType,
                IntData = IntData,
                DoubleData = DoubleData,
                BoolData = BoolData,
                HandTypeData = HandTypeData,
                SpecificCardRank = SpecificCardRank,
                SpecificCardSuit = SpecificCardSuit
            };
        }
    }
}
