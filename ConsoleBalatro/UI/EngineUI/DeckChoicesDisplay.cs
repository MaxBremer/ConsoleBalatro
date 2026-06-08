using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.UI.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public class DeckChoicesDisplay : TableDisplayEntity
    {
        public DeckChoicesDisplay(int xLoc, int yLoc) : base()
        {
            this.xLoc = xLoc;
            this.yLoc = yLoc;
        }

        public override void PreDisplaySetup()
        {
            BuildDeckChoiceTable();
            base.PreDisplaySetup();
        }

        private void BuildDeckChoiceTable()
        {
            MyTable = new();
            MyTable.Columns.Add(new TableColumn { Header = "Id", Width = 3 });
            MyTable.Columns.Add(new TableColumn { Header = "Name", Width = 10 });
            MyTable.Columns.Add(new TableColumn { Header = "MaxStake", Width = 10 });
            MyTable.Columns.Add(new TableColumn { Header = "CurStake", Width = 10 });
            MyTable.Columns.Add(new TableColumn { Header = "Desc", Width = 90 });
            MyTable.HeaderBottomBorder = true;
            MyTable.StretchMode = TableStretchMode.EvenColumns;
            MyTable.Width = EngineDisplayConstants.DECK_CHOICE_DISPLAY_WIDTH;
            int ct = 0;
            MyTable.Rows = DeckDb.DeckDBNames.Select(x => { ct++; return BuildDeckRow(ct, x); }).ToList();
        }

        private TableRow BuildDeckRow(int num, string deckDbName)
        {
            var rowVals = new List<string>();

            rowVals.Add(num.ToString());
            rowVals.Add(deckDbName);
            rowVals.Add("TEMP");
            rowVals.Add("TEMP");
            rowVals.Add(DeckDb.DeckData[deckDbName](null).DescriptionBuilder(null));//TODO: Gross. Yuck.
            
            return new TableRow(rowVals);
        }
    }
}
