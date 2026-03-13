using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Tags
{
    public enum TagType
    {
        NONE,
        NEGATIVE,
        HOLO,
        FOIL,
        POLYCHROME,
        MEGA_JOKER,
        MEGA_ARCANA,
        MEGA_PLANET,
        MEGA_STANDARD,
        SPECTRAL,
        TOP_UP, //2 common jokers
        DOUBLE_MONEY, //max 40 dollars
        INVESTMENT, //after next boss 25 dollars
        UNCOMMON,
        RARE,
        HANDY, //1$ per hand played this run
        GARBAGE, //1$ per discard this run
        ORBITAL,//ups given hand type by 3
        VOUCHER,
        BOSS_REROLL,
        COUPON,//next shop first items free
        DOUBLE_TAG,//add extra copy of next non-double tag
        JUGGLE, //next round +3 hand size
        REROLLS, //next shop rerolls start at 0
        SPEED, //5$ per skipped blind
    }
}
