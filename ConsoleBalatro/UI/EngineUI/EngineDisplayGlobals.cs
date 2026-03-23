using ConsoleBalatro.Engine.Cards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public static class EngineDisplayGlobals
    {
        public const bool OVERRIDE_ANIMATIONS = true;

        public static Dictionary<Edition, string> EditionBorderChars = new()
        {
            {Edition.FOIL, "^" },
            {Edition.HOLOGRAPHIC, "w" },
            {Edition.NEGATIVE, "_" },
            {Edition.POLYCHROME, "z" },

        };

        public static Dictionary<Enhancement, Func<string, string>> EnhancementModifiers = new()
        {
            { Enhancement.MULT, x => setCharsAt(x, 19, "MMM") },
            { Enhancement.BONUSCHIPS, x => setCharsAt(x, 19, "^^^") },
            { Enhancement.LUCKY, x => setCharsAt(x, 19, "LLL") },
            { Enhancement.WILD, x => setCharAt(x, 15, '*') },
            { Enhancement.GOLD, x => setCharsAt(x, 19, "GGG") },
            { Enhancement.GLASS, x => setCharsAt(x, 19, "GLA") },
            { Enhancement.STEEL, x => setCharsAt(x, 19, "STE") },
            { Enhancement.STONE, x => setCharsAt(x, 13, "STO") },
        };

        public static Dictionary<Seal, Func<string, string>> SealModifiers = new()
        {
            { Seal.RED, x => setCharAt(x, 9, 'r') },
            { Seal.BLUE, x => setCharAt(x, 9, 'b') },
            { Seal.PURPLE, x => setCharAt(x, 9, 'p') },
            { Seal.GOLD, x => setCharAt(x, 9, 'g') },
        };

        public static Dictionary<Sticker, Func<string, string>> StickerModifiers = new()
        {
            { Sticker.ETERNAL, x => setCharAt(x, 10, 'E') },
            { Sticker.PERISHABLE, x => setCharAt(x, 10, 'P') },
            { Sticker.RENTAL, x => setCharAt(x, 10, 'R') },
        };

        public static string setCharAt(string baseS, int ind, char x)
        {
            var cArr = baseS.ToCharArray();
            cArr[ind] = x;
            return new string(cArr);
        }

        public static string setCharsAt(string baseS, int startInd, List<char> charsToInsert)
        {
            var ret = baseS;
            for (int i = startInd; i < startInd + charsToInsert.Count; i++)
            {
                ret = setCharAt(ret, i, charsToInsert[i - startInd]);
            }
            return ret;
        }

        public static string setCharsAt(string baseS, int startInd, string charsInsert)
        {
            return setCharsAt(baseS, startInd, charsInsert.ToCharArray().ToList());
        }
    }
}
