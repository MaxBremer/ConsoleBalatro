using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools
{
    public enum ItemPool
    {
        Joker,
        Tarot,
        Planet,
        Spectral,
        PlayingCard,
    }

    public enum GenerationSource
    {
        Shop,
        Pack,
        SoulCard,
        JudgementCard,
        TopUpTag,
        RiffRaffJoker,
    }
}
