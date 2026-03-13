using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public class DestroyCardZone : CardZone
    {
        public DestroyCardZone() 
        {
            Name = "Destroy";
            MaxCapacity = -1;
        }
        public override bool AddCard(Card card, bool invisibleAdd = false, bool overrideSpace = false)
        {
            var result = base.AddCard(card, invisibleAdd, overrideSpace);
            if (result)
            {
                card.DestroyCard();
                RemoveCard(card, invisibleRemove: true);
            }
            return result;
        }
    }
}
