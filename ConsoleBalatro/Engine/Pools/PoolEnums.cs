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
        Pack,
        Voucher,
        PlayingCard,
    }

    public enum GenerationSource
    {
        Market,
        GenericJoker,
        Shop,
        Pack,
        SoulCard,
        JudgementCard,
        Tag,
        UncommonTag,
        RareTag,
        RiffRaffJoker,
        HighPriestessCard,
        EmperorCard,
        PurpleSeal,
        WraithCard,
        TopUpTag,
    }
}
