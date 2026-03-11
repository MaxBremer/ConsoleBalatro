using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI
{
    public class PanelDisplayEntity : DisplayEntity
    {
        public string SideChar = "|";
        public string TopChar = "-";
        public PanelDisplayEntity(int h, int w) : base(h, w)
        {
            Sprite = new string[h, w];
        }

        public bool ClearBg = false;

        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();
            //Set up side outline
            FillWith(ClearBg ? Interface.ClearTileIndicator : " ");
            for (int i = 0; i < Height; i++)
            {
                Sprite[i, 0] = SideChar;
                Sprite[i, Width - 1] = SideChar;
            }
            //Set up top outline
            for (int i = 0; i < Width; i++)
            {
                Sprite[0, i] = TopChar;
                Sprite[Height - 1, i] = TopChar;
            }
        }
    }
}
