using ConsoleBalatro.Engine.Cards.Tags;
using ConsoleBalatro.Engine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Jokers
{
    public class JokerCardDataBlock
    {
        public string JokerName;
        public string DBName;
        public Card MyCard;
        public bool isJoker = true;
        public JokerRarity Rarity = JokerRarity.COMMON;
        public Func<EventContext, string> DescriptionBuilder;
        public List<EngineEventListener> Listeners = new();
        public Dictionary<string, JokerData> DataDict = new();

        public JokerCardDataBlock HiddenCopiedData = null;

        public List<Action> OnJokerGainEffs = new();
        public List<Action> OnJokerRemovalEffs = new();

        //Voucher fields
        public bool isVoucher = false;
        public bool voucherIsBase = true;
        public string SuccessorVoucherDBName = "";
        public string PredecessorVoucherDBName = "";

        //Tag fields
        public bool isTag = false;
        public TagDataBlock TagData;

        public void CopyDataDictTo(JokerCardDataBlock target, bool clearFirst = true)
        {
            if (clearFirst)
                target.DataDict.Clear();
            foreach (var k in DataDict.Keys)
            {
                target.DataDict.Add(k, DataDict[k].MakeCopy());
            }
        }
    }
}
