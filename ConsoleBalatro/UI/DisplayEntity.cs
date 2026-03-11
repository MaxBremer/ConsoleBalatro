using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI
{
    public class DisplayEntity
    {
        public DisplayEntity(int h, int w)
        {
            Sprite = new string[h, w];
        }

        public string[,] Sprite { get; set; }
        public int xLoc { get; set; } = 0;
        public int yLoc { get; set; } = 0;
        public int zSortOrder { get; set; } = 0;
        public int Height => Sprite.GetLength(0);
        public int Width => Sprite.GetLength(1);

        public bool Visible = true;
        public Interface MyInterface;

        public void ImportFromString(string contents, string lineSeparator)
        {
            var lines = contents.Split(lineSeparator);
            Sprite = new string[lines.Length, lines[0].Length];
            for (int i = 0; i < lines.Count(); i++)
            {
                var cArr = lines[i].ToCharArray().Select(x => x.ToString()).ToList();
                for (int j = 0; j < cArr.Count; j++)
                {
                    Sprite[i, j] = cArr[j];
                }
            }
        }

        public void InsertOtherEntity(int xposlocal, int yposlocal, DisplayEntity otherEnt)
        {
            for (int i = 0; i < otherEnt.Height; i++)
            {
                for (int j = 0; j < otherEnt.Width; j++)
                {
                    var xToSet = j + xposlocal;
                    var yToSet = i + yposlocal;
                    if (yToSet < Height && xToSet < Width)
                    {
                        Sprite[yToSet, xToSet] = otherEnt.Sprite[i, j];
                    }
                }
            }
        }

        public void InsertOtherStringDirect(int xposlocal, int yposlocal, string toInsert)
        {
            for (int i = 0; i < toInsert.Length; i++)
            {
                if (xposlocal + i < Width)
                    Sprite[yposlocal, xposlocal + i] = toInsert[i].ToString();
            }
        }

        public void FillWith(string toFillWith)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Sprite[i, j] = toFillWith;
                }
            }
        }

        public void FillWithClear()
        {
            FillWith(Interface.ClearTileIndicator);
        }

        public virtual void PreDisplaySetup() { }
    }
}
