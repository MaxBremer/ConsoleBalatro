using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Events.Args
{
    public class EngineOddsEstablishedForPackArgs : EngineEventArgs
    {
        public Dictionary<BuyItemType, int> Odds { get; set; }
        public PackType PackBeingOpened { get; set; }
        public Cards.Consumables.ConsumableManager.PackData PackDataBeingOpened { get; set; }
        public List<Card> CardsForceAdded { get; set; } = new(); 
        
    }
}
