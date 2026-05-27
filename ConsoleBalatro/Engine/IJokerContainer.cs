using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    internal interface IJokerContainer
    {
        void AddJokerEffs(Card jokerCard);
        void RemoveJokerEffs(Card jokerCard);
    }
}
