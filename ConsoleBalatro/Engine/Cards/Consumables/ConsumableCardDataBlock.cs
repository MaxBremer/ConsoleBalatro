using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Consumables
{
    public class ConsumableCardDataBlock
    {
        public string ConsumableName;
        public string DBName;
        public ConsumableType Type;
        public BuyItemType BuyType;
        public PlayedHandType PlanetHandType;
        public Func<EventContext, string> DescriptionBuilder;

        public Card MyCard;

        public Dictionary<string, JokerData> DataDict = new();

        public Action<EngineEventArgs> Use = _ => { };
        public Func<EngineEventArgs?, bool> IsActivatable = _ => true;

        public void ActivateConsumable(EngineEventArgs evArgs)
        {
            //NOTE: this is here just in case global consumable wrapping needed.
            if (IsActivatable(evArgs))
            {
                Use(evArgs);
            }
        }

        public void CopyDataDictTo(ConsumableCardDataBlock target)
        {
            target.DataDict.Clear();
            foreach (var kvp in DataDict)
            {
                target.DataDict[kvp.Key] = kvp.Value;
            }
        }
    }

    public enum ConsumableType
    {
        TAROT,
        PLANET,
        SPECTRAL,
    }
}
