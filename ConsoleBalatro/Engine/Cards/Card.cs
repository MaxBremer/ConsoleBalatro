using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards
{
    public class Card
    {
        public CardZone MyZone;
        public bool isSelected = false;

        public Rank Rank;
        public Suit Suit;
        public Edition Edition;
        public Seal Seal;
        public Enhancement Enhancement;
        public JokerCardDataBlock JokerData;
        public ConsumableCardDataBlock ConsumableData;
        public int BuyCost;
        public int SellCost;
        public bool isJoker;
        public bool isPack;
        public bool isTag;
        public bool isConsumable;
        public int ID;
        public int BaseCost;
        public PackType MyPackType;
        public bool isVoucher;

        public void SetChipsFromRank() { }
        public void DestroyCard() { }
        public void ToggleSelect() { }
        public void ClearExtras() { }

        public void TriggerScoring(ScoringContext context) { }
        public void TriggerInHandDuringScoring(ScoringContext context) { }
        public Card MakeCopy() => null;
    }
}
