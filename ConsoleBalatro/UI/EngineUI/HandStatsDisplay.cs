using ConsoleBalatro.Engine;
using ConsoleBalatro.UI.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public class HandStatsDisplay : TableDisplayEntity
    {
        public HandStatsDisplay(int xLoc, int yLoc) : base()
        {
            this.xLoc = xLoc;
            this.yLoc = yLoc;
        }

        public override void PreDisplaySetup()
        {
            BuildMyHandStatTable();
            base.PreDisplaySetup();
        }

        private void BuildMyHandStatTable()
        {
            //Hand, Chipx X Mult, Level, Num Times Played
            MyTable = new();
            MyTable.Columns.Add(new TableColumn { Header = "Hand", Width = 15 });
            MyTable.Columns.Add(new TableColumn { Header = "Chips", Width = 8 });
            MyTable.Columns.Add(new TableColumn { Header = " X ", Width = 5 });
            MyTable.Columns.Add(new TableColumn { Header = "Mult", Width = 8 });
            MyTable.Columns.Add(new TableColumn { Header = "Level", Width = 8 });
            MyTable.Columns.Add(new TableColumn { Header = "Times Played", Width = 4 });
            MyTable.HeaderBottomBorder = true;
            MyTable.StretchMode = TableStretchMode.EvenColumns;
            MyTable.Width = 60;
            MyTable.Rows = ScoreHandler.CurrentHandStats.Keys.Select(x => BuildHandRow(x)).ToList();

        }



        private TableRow BuildHandRow(PlayedHandType handType)
        {
            var rowVals = new List<string>();

            rowVals.Add(handType.ToString());
            var curStats = ScoreHandler.CurrentHandStats[handType];
            rowVals.Add(curStats.Item1.ToString());
            rowVals.Add(" X ");
            rowVals.Add(curStats.Item2.ToString());
            rowVals.Add(ScoreHandler.HandLevels[handType].ToString());
            rowVals.Add(ScoreHandler.HandNumTimesPlayed[handType].ToString());

            return new TableRow(rowVals);
        }
    }
}
