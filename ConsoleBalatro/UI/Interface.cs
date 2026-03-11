using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI
{
    public class Interface
    {
        private List<DisplayEntity> Entities = new();
        public string DisplayBackground = ".";
        public const string ClearTileIndicator = "`";
        public int Width;
        public int Height;
        public static int Display_Width;
        public static int Display_Height;
        public string[,] Display;

        public Interface(int w, int h)
        {
            Width = w;
            Height = h;
            Display_Height = h;
            Display_Width = w;
            Display = new string[Height, Width];
        }

        public void Draw()
        {
            BuildDisplay();

            DisplayDisplay();
        }

        public void AddEntity(DisplayEntity entity)
        {
            entity.MyInterface = this;
            Entities.Add(entity);
        }

        public void RemoveEntity(DisplayEntity entity)
        {
            entity.MyInterface = null;
            Entities.Remove(entity);
        }

        public void ClearEntities()
        {
            foreach (var ent in Entities)
            {
                ent.MyInterface = null;
            }
            Entities.Clear();
        }

        public void BuildDisplay()
        {
            ResetToBackground();
            Entities = Entities.OrderBy(e => e.zSortOrder).ToList();
            foreach (var e in Entities.Where(x => x.Visible))
            {
                AddEntToDisplay(e);
            }
        }

        public void DisplayDisplay()
        {
            for (int i = 0; i < Height; i++)
            {
                var cur = "";
                for (int j = 0; j < Width; j++)
                {
                    cur += Display[i, j];
                }
                Console.WriteLine(cur);
            }
        }

        private void ResetToBackground()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Display[i, j] = DisplayBackground;
                }
            }
        }

        private void AddEntToDisplay(DisplayEntity ent)
        {
            ent.PreDisplaySetup();
            int xOffset = ent.xLoc;
            int yOffset = ent.yLoc;

            for (int i = 0; i < ent.Height; i++)
            {
                for (int j = 0; j < ent.Width; j++)
                {
                    int trueX = j + xOffset;
                    int trueY = i + yOffset;
                    if (trueX < Width && trueY < Height && ent.Sprite[i, j] != ClearTileIndicator)
                    {
                        Display[trueY, trueX] = ent.Sprite[i, j];
                    }
                }
            }
        }
    }
}
