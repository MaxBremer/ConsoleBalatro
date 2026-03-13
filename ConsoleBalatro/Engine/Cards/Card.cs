using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public JokerCardDataBlock JokerData;
        public ConsumableCardDataBlock ConsumableData;
        public int BuyCost;
        public bool isJoker;
        public bool isConsumable;

        public void SetChipsFromRank() { }
        public void DestroyCard() { }
        public void ToggleSelect() { }
    }
}
